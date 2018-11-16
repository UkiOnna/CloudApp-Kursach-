using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ClientCloud
{
    public class ClientWork
    {
        private const string START = "start";
        private const string GET_KEY = "GetKey";
        private const string GET_FILES = "fileList";
        private const string DOWNLOAD_FILE = "DownloadFile";
        private const string DOWNLOAD_FOLDER = "DownloadFolder";
        private const string UPLOAD_FILE = "UploadFile";
        private const string DELETE_ITEM = "DeleteItem";
        private const string CREATE_FOLDER = "CreateFolder";
        private const string REGISTRATION = "Registration";
        private const string LOGIN = "Login";
        private const string GET_LOG = "GetLog";
        private const string EXIT = "Exit";

        private Socket chatSocket;
        private IPEndPoint endPoint;
        public bool IsConnect { get; set; }
        public string Name { get; set; }
        public bool? IsKey { get; set; }
        public bool? IsRegistration { get; set; }
        public bool? IsLogin { get; set; }
        public Dictionary<string, string> fileList { get; set; }
        public List<string> LogList { get; set; }
        private bool isStrings;
        public string downloadSuccess { get; set; }
        public string refreshSuccess { get; set; }
        public bool isWorking { get; set; }
        public bool isLogWindowOpen { get; set; }
        public bool isOperationDone { get; set; }
        private string operationMessage;

        public ClientWork(string ip)
        {
            isWorking = false;
            chatSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endPoint = new IPEndPoint(IPAddress.Parse(ip), 3535);
            IsKey = null;
            IsRegistration = null;
            IsLogin = null;
            isStrings = true;
            downloadSuccess = null;
            refreshSuccess = null;
            isLogWindowOpen = false;
            isOperationDone = false;
            try
            {
                chatSocket.Connect(endPoint);
                Console.WriteLine("Вы вошли в чат!");
                IsConnect = true;
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public Task ThrowCommand(string command, string key, string file)
        {

            return Task.Run(() =>
            {
                try
                {
                    string serialized = JsonConvert.SerializeObject(new Message { Command = command, Key = key, FileWay = file });
                    chatSocket.Send(Encoding.Default.GetBytes(serialized));
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                }

            });
        }


        public Task SendCommand(string messg, string key, string file = "")
        {
            downloadSuccess = null;
            refreshSuccess = null;
            IsKey = null;
            IsRegistration = null;
            IsLogin = null;
            isOperationDone = false;
            return Task.Run(async () =>
            {
                await ThrowCommand(messg, key, file);
            });
        }


        public void CloseConnect()
        {
            Task task = ThrowCommand("Exit", "", "");
            task.Wait();
            IsConnect = false;
            chatSocket.Close();
        }

        public async void GetCommand()
        {
            await GetAnswer();
        }

        public Task GetAnswer()
        {
            return Task.Run(() =>
            {
                try
                {
                    while (IsConnect)
                    {
                        int bytes;
                        byte[] buffer = new byte[10000];

                        List<string> answer = new List<string>();
                        Dictionary<string, string> fileWays = new Dictionary<string, string>();

                        do
                        {
                            bytes = chatSocket.Receive(buffer);
                            if (isStrings)
                            {
                                answer = ConvertList.ByteArrayToList(buffer);
                            }
                            if (answer[0] == "false")
                            {
                                isStrings = false;
                                fileWays = ConvertList.ByteArrayToFileWays(buffer);
                            }
                        }
                        while (chatSocket.Available > 0);



                        isStrings = true;

                        if (answer.First() == GET_KEY + " false")
                        {
                            IsKey = false;
                            MessageBox.Show("Вы ввели неверный ключ");

                        }

                        else if (answer.First() == GET_KEY + " true")
                        {
                            IsKey = true;

                        }

                        else if (answer.First() == DOWNLOAD_FILE || answer.First() == DOWNLOAD_FOLDER)
                        {
                            downloadSuccess = answer[1];
                            MessageBox.Show(answer[1]);
                        }

                        else if (answer.First() == UPLOAD_FILE)
                        {
                            downloadSuccess = answer[1];
                            isOperationDone = true;
                            operationMessage = answer[1];
                        }

                        else if (answer.First() == UPLOAD_FILE + " false")
                        {
                            downloadSuccess = answer[1];
                            MessageBox.Show(answer[1]);
                        }

                        else if (answer.First() == DELETE_ITEM)
                        {
                            downloadSuccess = answer[1];
                            isOperationDone = true;
                            operationMessage = answer[1];
                        }

                        else if (answer.First() == DELETE_ITEM + " false")
                        {
                            downloadSuccess = answer[1];
                            MessageBox.Show(answer[1]);
                        }

                        else if (answer.First() == CREATE_FOLDER)
                        {
                            downloadSuccess = answer[1];
                            isOperationDone = true;
                            operationMessage = answer[1];
                        }

                        else if (answer.First() == CREATE_FOLDER + " false")
                        {
                            downloadSuccess = answer[1];
                            MessageBox.Show(answer[1]);
                        }

                        else if (answer.First() == GET_LOG + " true")
                        {
                            downloadSuccess = answer[1];
                            isLogWindowOpen = true;
                            LogList = answer;
                        }

                        else if (answer.First() == GET_LOG + " false")
                        {
                            downloadSuccess = answer[1];
                            isLogWindowOpen = true;
                            LogList = answer;
                        }

                        else if (answer.First() == REGISTRATION + " true")
                        {
                            IsRegistration = true;
                            MessageBox.Show(answer[1]);
                        }

                        else if (answer.First() == REGISTRATION + " false")
                        {
                            IsRegistration = false;
                            MessageBox.Show(answer[1]);
                        }

                        else if (answer.First() == LOGIN + " false")
                        {
                            IsLogin = false;
                            downloadSuccess = answer[1];
                            MessageBox.Show(answer[1]);
                        }

                        else if (answer.First() == LOGIN + " true")
                        {
                            SendCommand("GetFiles", "");
                        }



                        else if (fileWays.First().Key == GET_FILES)
                        {
                            refreshSuccess = "ha";
                            IsLogin = true;
                            fileList = fileWays;
                            if (isOperationDone)
                            {
                                MessageBox.Show(operationMessage);
                            }
                        }
                    }
                }

                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }
    }
}
