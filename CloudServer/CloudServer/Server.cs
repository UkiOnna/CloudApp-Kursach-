using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dropbox.Api;
using System.IO;
using Dropbox.Api.Files;

namespace CloudServer
{
    public class Server
    {
        private string key;
        private List<Socket> sockets = new List<Socket>();
        private string name;
        private Dictionary<string,string> fileWays;
        private string toFileSave;
        private string fromFileDownload;
        public Server()
        {
            fileWays = new Dictionary<string, string>();
        }

        public void BeginToDo(Socket sok)
        {
            try
            {
                Console.WriteLine("Сервер работает");


                sok.Listen(1);
                while (true)
                {
                    if (sockets.Count < 1)
                    {

                        sockets.Add(sok.Accept());
                        int socketIndex = sockets.Count - 1;

                        ClientJoin(socketIndex);
                    }
                    
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Poka");
                sok.Close();
                for (int i = 0; i < sockets.Count; i++)
                {
                    sockets[i].Shutdown(SocketShutdown.Both);
                }
            }
        }

        public Task ClientJoin(int sokIndx)
        {
            return Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        if (sockets.Count > 0)
                        {
                            int bytes;
                            byte[] buffer = new byte[1024];
                            StringBuilder stringBuilder = new StringBuilder();

                            do
                            {
                                bytes = sockets[sokIndx].Receive(buffer);
                                stringBuilder.Append(Encoding.Default.GetString(buffer));
                            }
                            while (sockets[sokIndx].Available > 0);


                            Message newMessage = JsonConvert.DeserializeObject<Message>(stringBuilder.ToString());
                            if (newMessage.Command == "start")
                            {
                                Console.WriteLine("Присоединено");
                            }
                            else if (newMessage.Command == "4")
                            {
                                Console.WriteLine("Пока");
                                sockets[sokIndx].Shutdown(SocketShutdown.Both);
                                sockets.RemoveAt(sokIndx);
                            }




                            else if (newMessage.Command == "GetKey")
                            {

                                key = newMessage.Key;
                                List<string> answer = new List<string>();

                                try
                                {
                                    var task = Task.Run(CheckKey);
                                    task.Wait();
                                    Console.WriteLine("Получен ключ");
                                    newMessage.Key = "GetKey true";
                                    newMessage.Command = name;
                                    answer.Add(newMessage.Key);
                                    answer.Add(newMessage.Command);
                                    sockets[0].Send(ConvertList.ListToByteArray(answer));
                                }
                                catch (System.AggregateException)
                                {
                                    Console.WriteLine("Неверный ключ");
                                    newMessage.Key = "false";
                                    answer.Add(newMessage.Command + " " + newMessage.Key);
                                    sockets[0].Send(ConvertList.ListToByteArray(answer));
                                }

                            }

                            else if (newMessage.Command == "GetFiles")
                            {
                                DropBoxFacade facade = new DropBoxFacade(key);

                                fileWays.Add("fileList", "");
                                var task = facade.LoadAll();
                                task.Wait();
                                DisplayAll(facade.Folders.First(), 0);
                                sockets[0].Send(ConvertList.FileWaysToByteArray(fileWays));
                                fileWays.Clear();
                            }

                            else if (newMessage.Command == "DownloadFile")
                            {
                                List<string> answer = new List<string>();
                                try
                                {
                                    fromFileDownload = newMessage.FileWay;
                                    toFileSave = newMessage.Key;
                                    var task = Task.Run(DownloadFile);
                                    task.Wait();
                                    answer.Add("DownloadFile");
                                    answer.Add("Файл успешно скачан");
                                    sockets[0].Send(ConvertList.ListToByteArray(answer));
                                }
                                catch(System.AggregateException)
                                {
                                    Console.WriteLine("Ошибка при скачивании");
                                    newMessage.Key = "false";
                                    answer.Add("DownloadFile");
                                    answer.Add("При скачивании поизошла ошибка");
                                    sockets[0].Send(ConvertList.ListToByteArray(answer));
                                }
                            }

                            else if (newMessage.Command == "UploadFile")
                            {
                                List<string> answer = new List<string>();
                                try
                                {
                                    fromFileDownload = newMessage.Key;
                                    toFileSave = newMessage.FileWay;
                                    var task = Task.Run(UploadFile);
                                    task.Wait();
                                    answer.Add("UploadFile");
                                    answer.Add("Файл успешно загружен");
                                    sockets[0].Send(ConvertList.ListToByteArray(answer));
                                }
                                catch (System.AggregateException)
                                {
                                    Console.WriteLine("Ошибка при загрузке,возможно вы выбрали не папку а файл для загрузки файла");
                                    newMessage.Key = "false";
                                    answer.Add("UploadFile");
                                    answer.Add("Ошибка при загрузке,возможно вы выбрали не паку для загрузки файла");
                                    sockets[0].Send(ConvertList.ListToByteArray(answer));
                                }
                            }

                            //else if(newMessage.Command=="GetKey")
                            //{
                            //    Console.WriteLine("Пользователь " + newMessage.Name + " отправил сообщение");
                            //    for (int i = 0; i < sockets.Count; i++)
                            //    {
                            //        if (i != sokIndx)
                            //        {
                            //            sockets[i].Send(Encoding.Default.GetBytes(newMessage.Name + ": " + newMessage.Letter));
                            //        }
                            //    }
                            //}

                            else
                            {
                            }

                        }
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentOutOfRangeException ex)
                {

                }
            });
        }

        public async Task CheckKey()
        {
            using (var dropBox = new DropboxClient(key))
            {
                var id = await dropBox.Users.GetCurrentAccountAsync();
                Console.WriteLine(id.Name.DisplayName);
                name=id.Name.DisplayName;
                //Здесь в базу данных айди логин пользователя
            }
        }


        public void DisplayAll(DropBoxFolder folder, int offset)
        {
            if (folder == null)
            {
                return;
            }

            foreach (var element in folder.Elements)
            {
                if (element.IsFile)
                {
                    fileWays.Add(new String(' ', offset) + "[File]" + element.Name, element.FullName);
                    Console.WriteLine(element.FullName);
                }
                else
                {
                    fileWays.Add(new String(' ', offset) + "[Folder]" + element.Name, element.FullName); ;
                    Console.WriteLine(element.FullName);
                }
                if (element.IsFolder)
                {
                    DisplayAll(element as DropBoxFolder, offset + 2);
                }
            }
        }

        public async Task DownloadFile()
        {
            using (var dbx = new DropboxClient(key))
            {
                string folder = "";
                string file = fromFileDownload;
                using (var response = await dbx.Files.DownloadAsync(file))
                {
                    var s = response.GetContentAsByteArrayAsync();
                    s.Wait();
                    var d = s.Result;
                    File.WriteAllBytes(toFileSave, d);
                }
            }
        }

        public async Task UploadFile()
        {
            using (var dbx = new DropboxClient(key))
            {
                string file = fromFileDownload;
                string folder = "/Public";
                string filename = "dummytextfile.txt";
                string url = "";
                using (var mem = new MemoryStream(File.ReadAllBytes(file)))
                {
                    var updated = dbx.Files.UploadAsync(toFileSave, WriteMode.Overwrite.Instance, body: mem);
                    updated.Wait();
                    var tx = dbx.Sharing.CreateSharedLinkWithSettingsAsync(toFileSave);
                    tx.Wait();
                    url = tx.Result.Url;
                }
                Console.Write(url);
            }
        }

    }
}
