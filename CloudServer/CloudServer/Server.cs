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
        private const string DOWNLOAD_FOLDER = "DownloadFolder";
        private const string UPLOAD_FILE = "UploadFile";
        private const string DELETE_ITEM = "DeleteItem";
        private const string CREATE_FOLDER = "CreateFolder";
        private const string REGISTRATION = "Registration";
        private const string LOGIN = "Login";
        private const string GET_LOG = "GetLog";
        Dictionary<long, CommandWork> users;
        private long counter;
        

        public Server()
        {
            users = new Dictionary<long, CommandWork>();
            counter = 0;
            using (var context = new DropBoxContext())
            {
                context.Users.ToList();
            }
        }

        public void BeginToDo(Socket sok)
        {
            try
            {
                Console.WriteLine("Сервер работает");

                sok.Listen(1);
                while (true)
                {
                    CommandWork user = new CommandWork();
                    user.MySocket = sok.Accept();
                    users.Add(counter, user);
                    ClientJoin(counter);
                    counter++;
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
                for(int i = 0; i < users.Count; i++)
                {
                    users[i].Exit();
                }
            }
        }

        public Task ClientJoin(long sokIndx)
        {
            return Task.Run(() =>
            {
                try
                {
                    while (true)
                    {

                        if (users.Count > 0)
                        {
                            if (users.ContainsKey(sokIndx))
                            {
                                users[sokIndx].GetCommand();

                                if (users[sokIndx].NewCommand.Command == START)
                                {
                                    users[sokIndx].Start();
                                    users[sokIndx].NewCommand = null;
                                }
                                else if (users[sokIndx].NewCommand.Command == "4")
                                {
                                    users[sokIndx].Exit();
                                    users.Remove(sokIndx);
                                    Console.WriteLine("Пока");

                                }




                                else if (users[sokIndx].NewCommand.Command == GET_KEY)
                                {
                                    users[sokIndx].GetKey();
                                    users[sokIndx].NewCommand = null;

                                }

                                else if (users[sokIndx].NewCommand.Command == GET_FILES)
                                {
                                    users[sokIndx].GetFiles();
                                    users[sokIndx].NewCommand = null;
                                }

                                else if (users[sokIndx].NewCommand.Command == DOWNLOAD_FILE || users[sokIndx].NewCommand.Command==DOWNLOAD_FOLDER)
                                {
                                    users[sokIndx].DownloadFile();
                                    users[sokIndx].NewCommand = null;
                                }

                                else if (users[sokIndx].NewCommand.Command == UPLOAD_FILE)
                                {
                                    users[sokIndx].UploadFile();
                                    users[sokIndx].NewCommand = null;
                                }

                                else if (users[sokIndx].NewCommand.Command == DELETE_ITEM)
                                {
                                    users[sokIndx].DeleteFile();
                                    users[sokIndx].NewCommand = null;
                                }

                                else if (users[sokIndx].NewCommand.Command == CREATE_FOLDER)
                                {
                                    users[sokIndx].CreateFolder();
                                    users[sokIndx].NewCommand = null;
                                }

                                else if (users[sokIndx].NewCommand.Command == REGISTRATION)
                                {
                                    users[sokIndx].Registration();
                                    users[sokIndx].NewCommand = null;
                                }

                                else if (users[sokIndx].NewCommand.Command == LOGIN)
                                {
                                    users[sokIndx].Login();
                                    users[sokIndx].NewCommand = null;
                                }
                                else if (users[sokIndx].NewCommand.Command == GET_LOG)
                                {
                                    users[sokIndx].GetLog();
                                    users[sokIndx].NewCommand = null;
                                }



                                else
                                {
                                }
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
    }
}
