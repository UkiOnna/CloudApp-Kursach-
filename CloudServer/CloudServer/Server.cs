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
using System.Data.Entity.Infrastructure;

namespace CloudServer
{
    public class Server
    {
        private const string START = "start";
        private const string GET_KEY = "GetKey";
        private const string GET_FILES = "GetFiles";
        private const string DOWNLOAD_FILE = "DownloadFile";
        private const string UPLOAD_FILE = "UploadFile";
        private const string DELETE_ITEM = "DeleteItem";
        private const string CREATE_FOLDER = "CreateFolder";
        private const string REGISTRATION = "Registration";
        private const string LOGIN = "Login";
        private string key;
        private List<Socket> sockets = new List<Socket>();
        private string name;
        private Dictionary<string, string> fileWays;
        private string toFileSave;
        private string fromFileDownload;
        private string itemNeedToDelete;
        private string folderName;
        private List<string> answer;
        private object locker = new object();
        public Server()
        {
            fileWays = new Dictionary<string, string>();
            answer = new List<string>();
        }

        public void BeginToDo(Socket sok)
        {
            try
            {
                Console.WriteLine("Сервер работает");

                sok.Listen(1);
                while (true)
                {
                    sockets.Add(sok.Accept());
                    int socketIndex = sockets.Count - 1;

                    ClientJoin(socketIndex);
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

                            if (newMessage.Command == START)
                            {
                                lock (locker)
                                {
                                    Console.WriteLine("Присоединено");
                                    newMessage = null;
                                }
                            }
                            else if (newMessage.Command == "4")
                            {
                                lock (locker)
                                {
                                    Console.WriteLine("Пока");
                                    sockets[sokIndx].Shutdown(SocketShutdown.Both);
                                    sockets.RemoveAt(sokIndx);
                                    newMessage = null;
                                }
                            }




                            else if (newMessage.Command == GET_KEY)
                            {
                                lock (locker)
                                {
                                    key = newMessage.Key;

                                    try
                                    {
                                        var task = Task.Run(CheckKey);
                                        task.Wait();
                                        Console.WriteLine("Получен ключ");
                                        newMessage.Key = "GetKey true";
                                        newMessage.Command = name;
                                        answer.Add(newMessage.Key);
                                        answer.Add(newMessage.Command);
                                        sockets[sokIndx].Send(ConvertList.ListToByteArray(answer));
                                        answer.Clear();
                                        newMessage = null;
                                    }
                                    catch (AggregateException)
                                    {
                                        Console.WriteLine("Неверный ключ");
                                        newMessage.Key = "false";
                                        answer.Add(newMessage.Command + " " + newMessage.Key);
                                        sockets[sokIndx].Send(ConvertList.ListToByteArray(answer));
                                        answer.Clear();
                                        newMessage = null;
                                    }
                                }

                            }

                            else if (newMessage.Command == GET_FILES)
                            {
                                lock (locker)
                                {
                                    DropBoxFacade facade = new DropBoxFacade(key);

                                    fileWays.Add("fileList", "");
                                    var task = facade.LoadAll();
                                    task.Wait();
                                    DisplayAll(facade.Folders.First(), 0);
                                    sockets[sokIndx].Send(ConvertList.FileWaysToByteArray(fileWays));
                                    fileWays.Clear();
                                    newMessage = null;
                                }
                            }

                            else if (newMessage.Command == DOWNLOAD_FILE)
                            {
                                lock (locker)
                                {
                                    try
                                    {
                                        fromFileDownload = newMessage.FileWay;
                                        toFileSave = newMessage.Key;
                                        var task = Task.Run(DownloadFile);
                                        task.Wait();
                                        answer.Add(DOWNLOAD_FILE);
                                        answer.Add("Файл успешно скачан");
                                        sockets[sokIndx].Send(ConvertList.ListToByteArray(answer));
                                        answer.Clear();
                                        newMessage = null;
                                    }
                                    catch (AggregateException ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                        newMessage.Key = "false";
                                        answer.Add(DOWNLOAD_FILE);
                                        answer.Add("При скачивании поизошла ошибка");
                                        sockets[sokIndx].Send(ConvertList.ListToByteArray(answer));
                                        answer.Clear();
                                        newMessage = null;
                                    }
                                }
                            }

                            else if (newMessage.Command == UPLOAD_FILE)
                            {
                                lock (locker)
                                {
                                    try
                                    {
                                        fromFileDownload = newMessage.Key;
                                        toFileSave = newMessage.FileWay;
                                        var task = Task.Run(UploadFile);
                                        task.Wait();
                                        answer.Add(UPLOAD_FILE);
                                        answer.Add("Файл успешно загружен");
                                        sockets[sokIndx].Send(ConvertList.ListToByteArray(answer));
                                        answer.Clear();
                                        newMessage = null;
                                    }
                                    catch (AggregateException)
                                    {
                                        Console.WriteLine("Ошибка при загрузке,возможно вы выбрали не папку а файл для загрузки файла");
                                        newMessage.Key = "false";
                                        answer.Add(UPLOAD_FILE);
                                        answer.Add("Ошибка при загрузке,возможно вы выбрали не паку для загрузки файла");
                                        sockets[sokIndx].Send(ConvertList.ListToByteArray(answer));
                                        answer.Clear();
                                        newMessage = null;
                                    }
                                }
                            }

                            else if (newMessage.Command == DELETE_ITEM)
                            {
                                lock (locker)
                                {
                                    try
                                    {
                                        itemNeedToDelete = newMessage.Key;
                                        var task = Task.Run(DeleteItem);
                                        task.Wait();
                                        answer.Add(DELETE_ITEM);
                                        answer.Add("Файл удален");
                                        sockets[sokIndx].Send(ConvertList.ListToByteArray(answer));
                                        answer.Clear();
                                        newMessage = null;

                                    }
                                    catch (AggregateException)
                                    {
                                        newMessage.Key = "false";
                                        answer.Add(DELETE_ITEM);
                                        answer.Add("Ошибка при удалении");
                                        sockets[sokIndx].Send(ConvertList.ListToByteArray(answer));
                                        answer.Clear();
                                        newMessage = null;
                                    }
                                }
                            }

                            else if (newMessage.Command == CREATE_FOLDER)
                            {
                                lock (locker)
                                {
                                    try
                                    {
                                        folderName = newMessage.Key;
                                        var task = Task.Run(CreateFolder);
                                        task.Wait();
                                        answer.Add(CREATE_FOLDER);
                                        answer.Add("Папка создана");
                                        sockets[sokIndx].Send(ConvertList.ListToByteArray(answer));
                                        answer.Clear();
                                        newMessage = null;

                                    }
                                    catch (AggregateException)
                                    {
                                        newMessage.Key = "false";
                                        answer.Add(CREATE_FOLDER);
                                        answer.Add("Ошибка при создании");
                                        sockets[sokIndx].Send(ConvertList.ListToByteArray(answer));
                                        answer.Clear();
                                        newMessage = null;
                                    }
                                }
                            }

                            else if (newMessage.Command == REGISTRATION)
                            {
                                lock (locker)
                                {
                                    try
                                    {

                                        using (var context = new DropBoxContext())
                                        {
                                            if (context.Users.Any(item => item.Key == key)) throw new Exception();

                                            User user = new User { Login = newMessage.Key, Key = key, Password = newMessage.FileWay };
                                            context.Users.Add(user);
                                            context.SaveChanges();
                                            answer.Add(REGISTRATION + " " + "true");
                                            answer.Add("Регистрация прошла успешно");
                                            sockets[sokIndx].Send(ConvertList.ListToByteArray(answer));
                                            answer.Clear();

                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                        answer.Add(REGISTRATION + " " + "false");
                                        answer.Add("Ошибка при регистрации пользователя");
                                        sockets[sokIndx].Send(ConvertList.ListToByteArray(answer));
                                        answer.Clear();
                                    }
                                }
                            }

                            else if (newMessage.Command == LOGIN)
                            {
                                lock (locker)
                                {
                                    try
                                    {
                                        bool isLoginSuccess = false;
                                        using (var context = new DropBoxContext())
                                        {
                                            foreach (var item in context.Users)
                                            {
                                                if (item.Login == newMessage.Key && item.Password == newMessage.FileWay)
                                                {
                                                    isLoginSuccess = true;
                                                    key = item.Key;
                                                }
                                            }
                                        }

                                        if (isLoginSuccess)
                                        {
                                            answer.Add(LOGIN + " " + "true");
                                            sockets[sokIndx].Send(ConvertList.ListToByteArray(answer));
                                            answer.Clear();
                                            newMessage = null;
                                        }
                                        else
                                        {
                                            throw new Exception();
                                        }


                                    }
                                    catch (Exception)
                                    {

                                        answer.Add(LOGIN + " " + "false");
                                        answer.Add("Вы ввели нправильный логин или пароль");
                                        sockets[sokIndx].Send(ConvertList.ListToByteArray(answer));
                                        answer.Clear();
                                        newMessage = null;
                                    }
                                }
                            }



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
                name = id.Name.DisplayName;
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
                    fileWays.Add(new String(' ', offset) + "[Folder]" + element.Name, element.FullName);
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
                using (var mem = new MemoryStream(File.ReadAllBytes(file)))
                {
                    var updated = dbx.Files.UploadAsync(toFileSave, WriteMode.Overwrite.Instance, body: mem);
                    updated.Wait();
                }
            }
        }

        public async Task DeleteItem()
        {
            using (var dbx = new DropboxClient(key))
            {
                var updated = dbx.Files.DeleteV2Async(itemNeedToDelete, null);
                updated.Wait();
            }
        }

        public async Task CreateFolder()
        {
            using (var dbx = new DropboxClient(key))
            {
                var updated = dbx.Files.CreateFolderV2Async(folderName, true);
                updated.Wait();
            }
        }

    }
}
