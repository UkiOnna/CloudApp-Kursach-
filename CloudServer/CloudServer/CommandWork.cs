using Dropbox.Api;
using Dropbox.Api.Files;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CloudServer
{

    public class CommandWork
    {
        private const string START = "start";
        private const string GET_KEY = "GetKey";
        private const string GET_FILES = "GetFiles";
        private const string DOWNLOAD_FILE = "DownloadFile";
        private const string DOWNLOAD_FOLDER = "DownloadFolder";
        private const string UPLOAD_FILE = "UploadFile";
        private const string DELETE_ITEM = "DeleteItem";
        private const string CREATE_FOLDER = "CreateFolder";
        private const string REGISTRATION = "Registration";
        private const string LOGIN = "Login";
        private const string GET_LOG = "GetLog";
        private string key;
        private Dictionary<string, string> fileWays;
        private string toFileSave;
        private string fromFileDownload;
        private string itemNeedToDelete;
        private string folderName;
        private string name;
        public Message NewCommand { get; set; }
        public Socket MySocket { get; set; }
        private List<string> answer;
        public bool IsIndexChange;

        public CommandWork()
        {
            IsIndexChange = false;
            fileWays = new Dictionary<string, string>();
            answer = new List<string>();
            using (var context = new DropBoxContext())
            {
                context.Users.ToList();
            }
        }

        public void Start()
        {
            Console.WriteLine("Присоединено с класса");
        }

        public void GetCommand()
        {
            int bytes;
            byte[] buffer = new byte[1024];
            StringBuilder stringBuilder = new StringBuilder();

            do
            {
                bytes = MySocket.Receive(buffer);
                stringBuilder.Append(Encoding.Default.GetString(buffer));
            }
            while (MySocket.Available > 0);


            NewCommand = JsonConvert.DeserializeObject<Message>(stringBuilder.ToString());
        }

        public void Exit()
        {
            MySocket.Shutdown(SocketShutdown.Both);
        }

        public void GetKey()
        {
            key = NewCommand.Key;

            try
            {
                var task = Task.Run(CheckKey);
                task.Wait();
                Console.WriteLine("Получен ключ");
                NewCommand.Key = "GetKey true";
                NewCommand.Command = name;
                answer.Add(NewCommand.Key);
                answer.Add(NewCommand.Command);
                MySocket.Send(ConvertList.ListToByteArray(answer));
                answer.Clear();
            }
            catch (AggregateException)
            {
                Console.WriteLine("Неверный ключ");
                NewCommand.Key = "false";
                answer.Add(NewCommand.Command + " " + NewCommand.Key);
                MySocket.Send(ConvertList.ListToByteArray(answer));
                answer.Clear();
            }
        }

        public void GetFiles()
        {
            DropBoxFacade facade = new DropBoxFacade(key);

            fileWays.Clear();
            fileWays.Add("fileList", "");
            var task = facade.LoadAll();
            task.Wait();
            DisplayAll(facade.Folders.First(), 0);
            MySocket.Send(ConvertList.FileWaysToByteArray(fileWays));

        }

        public void DownloadFile()
        {
            try
            {

                fromFileDownload = NewCommand.FileWay;
                toFileSave = NewCommand.Key;
                var task = Task.Run(DropBoxDownloadFile);
                task.Wait();
                answer.Add(DOWNLOAD_FILE);
                answer.Add("Файл успешно скачан");
                MySocket.Send(ConvertList.ListToByteArray(answer));
                answer.Clear();
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.Message);
                NewCommand.Key = "false";
                answer.Add(DOWNLOAD_FILE);
                answer.Add("При скачивании поизошла ошибка");
                MySocket.Send(ConvertList.ListToByteArray(answer));
                answer.Clear();
            }
        }

        public void UploadFile()
        {
            try
            {
                fromFileDownload = NewCommand.Key;
                toFileSave = NewCommand.FileWay;
                var task = Task.Run(DropBoxUploadFile);
                task.Wait();
                answer.Add(UPLOAD_FILE);
                answer.Add("Файл успешно загружен");
                using (var context = new DropBoxContext())
                {
                    FileInfo info = new FileInfo();
                    info.UserId = context.Users.ToList().Find(item => item.Key == key).Id;
                    info.FileWay = toFileSave;
                    string[] words = toFileSave.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    info.FileName = words.Last();
                    info.IsDeleted = false;
                    info.CreationDate = DateTime.Now;
                    info.DeletedDate = null;
                    context.FilesInfo.Add(info);
                    context.SaveChanges();
                }

                MySocket.Send(ConvertList.ListToByteArray(answer));
                answer.Clear();
            }
            catch (AggregateException)
            {
                Console.WriteLine("Ошибка при загрузке,возможно вы выбрали не папку а файл для загрузки файла");
                NewCommand.Key = "false";
                answer.Clear();
                answer.Add(UPLOAD_FILE + " false");
                answer.Add("Ошибка при загрузке,возможно вы выбрали не паку для загрузки файла");
                MySocket.Send(ConvertList.ListToByteArray(answer));
                answer.Clear();
            }
        }

        public void DeleteFile()
        {
            try
            {
                itemNeedToDelete = NewCommand.Key;
                var task = Task.Run(DeleteItem);
                task.Wait();
                answer.Add(DELETE_ITEM);
                answer.Add("Файл удален");
                using (var context = new DropBoxContext())
                {
                    if (context.FilesInfo.ToList().Count != 0)
                    {
                        int userId = context.Users.ToList().Find(item => item.Key == key).Id;
                        FileInfo info = context.FilesInfo.ToList().Find(item => item.UserId == userId && item.IsDeleted == false && item.FileWay == itemNeedToDelete);
                        if (info != null)
                        {
                            info.IsDeleted = true;
                            info.DeletedDate = DateTime.Now;
                            FileInfo oldInfo = context.FilesInfo.ToList().Find(item => item.UserId == userId && item.IsDeleted == false && item.FileWay == itemNeedToDelete);

                            int index = context.FilesInfo.ToList().IndexOf(oldInfo);
                            if (index != -1)
                                context.FilesInfo.ToList()[index] = info;

                            context.SaveChanges();
                        }
                    }
                }
                MySocket.Send(ConvertList.ListToByteArray(answer));
                answer.Clear();

            }
            catch (AggregateException)
            {
                NewCommand.Key = "false";
                answer.Add(DELETE_ITEM + " false");
                answer.Add("Ошибка при удалении");
                MySocket.Send(ConvertList.ListToByteArray(answer));
                answer.Clear();
            }
        }

        public void CreateFolder()
        {
            try
            {
                folderName = NewCommand.Key;
                var task = Task.Run(DropBoxCreateFolder);
                task.Wait();
                answer.Add(CREATE_FOLDER);
                answer.Add("Папка создана");
                MySocket.Send(ConvertList.ListToByteArray(answer));
                answer.Clear();

            }
            catch (AggregateException)
            {
                NewCommand.Key = "false";
                answer.Add(CREATE_FOLDER + " false");
                answer.Add("Ошибка при создании");
                MySocket.Send(ConvertList.ListToByteArray(answer));
                answer.Clear();
            }
        }

        public void Registration()
        {
            try
            {

                using (var context = new DropBoxContext())
                {
                    if (context.Users.Any(item => item.Key == key||item.Login==NewCommand.Key)) throw new Exception();

                    User user = new User { Login = NewCommand.Key, Key = key, Password = NewCommand.FileWay };
                    context.Users.Add(user);
                    context.SaveChanges();
                    answer.Add(REGISTRATION + " " + "true");
                    answer.Add("Регистрация прошла успешно");
                    MySocket.Send(ConvertList.ListToByteArray(answer));
                    answer.Clear();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                answer.Add(REGISTRATION + " " + "false");
                answer.Add("Пльзователь с таким логином или ключом уже присутствует");
                MySocket.Send(ConvertList.ListToByteArray(answer));
                answer.Clear();
            }
        }

        public void Login()
        {
            try
            {
                bool isLoginSuccess = false;
                using (var context = new DropBoxContext())
                {
                    foreach (var item in context.Users)
                    {
                        if (item.Login == NewCommand.Key && item.Password == NewCommand.FileWay)
                        {
                            isLoginSuccess = true;
                            key = item.Key;
                        }
                    }
                }

                if (isLoginSuccess)
                {
                    answer.Add(LOGIN + " " + "true");
                    MySocket.Send(ConvertList.ListToByteArray(answer));
                    answer.Clear();
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
                MySocket.Send(ConvertList.ListToByteArray(answer));
                answer.Clear();
            }
        }

        public void GetLog()
        {
            try
            {
                using (var context = new DropBoxContext())
                {
                    if (context.FilesInfo.ToList().Count != 0)
                    {
                        answer.Add(GET_LOG + " true");
                        foreach (var a in context.FilesInfo.ToList())
                        {
                            if (a.IsDeleted)
                            {
                                answer.Add(a.FileName + '-' + a.CreationDate.ToShortDateString() + " " + a.CreationDate.ToShortTimeString() + '-'
                                     + a.CreationDate.ToShortDateString() + " " + a.DeletedDate.Value.ToShortTimeString());
                            }

                            else
                            {
                                answer.Add(a.FileName + '-' + a.CreationDate.ToShortDateString() + " " + a.CreationDate.ToShortTimeString() + '-' + " ");
                            }
                        }
                        MySocket.Send(ConvertList.ListToByteArray(answer));
                        answer.Clear();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception)
            {
                answer.Add(GET_LOG + " " + "false");
                answer.Add(" " + "-" + " " + "-" + " ");
                MySocket.Send(ConvertList.ListToByteArray(answer));
                answer.Clear();
            }
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

        public async Task DropBoxDownloadFile()
        {
            using (var dbx = new DropboxClient(key))
            {

                string file = fromFileDownload;
                if (NewCommand.Command == DOWNLOAD_FOLDER)
                {
                    using (var response = await dbx.Files.DownloadZipAsync(file))
                    {
                        var s = response.GetContentAsByteArrayAsync();
                        s.Wait();
                        var d = s.Result;
                        toFileSave += ".rar";
                        File.WriteAllBytes(toFileSave, d);
                    }
                }
                else
                {
                    using (var response = await dbx.Files.DownloadAsync(file))
                    {
                        var s = response.GetContentAsByteArrayAsync();
                        s.Wait();
                        var d = s.Result;
                        File.WriteAllBytes(toFileSave, d);
                    }
                }
            }
        }

        public async Task DropBoxUploadFile()
        {
            using (var dbx = new DropboxClient(key))
            {
                string file = fromFileDownload;
                using (var mem = new MemoryStream(File.ReadAllBytes(file)))
                {
                    var updated = await dbx.Files.UploadAsync(toFileSave, WriteMode.Overwrite.Instance, body: mem);
                }
            }
        }

        public async Task DeleteItem()
        {
            using (var dbx = new DropboxClient(key))
            {
                var updated = await dbx.Files.DeleteV2Async(itemNeedToDelete, null);
            }
        }

        public async Task DropBoxCreateFolder()
        {
            using (var dbx = new DropboxClient(key))
            {
                var updated = await dbx.Files.CreateFolderV2Async(folderName, true);
            }
        }
    }
}
