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

        public ClientWork()
        {
            isWorking = false;
            chatSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3535);
            IsKey = null;
            IsRegistration = null;
            IsLogin = null;
            isStrings = true;
            downloadSuccess = null;
            refreshSuccess = null;
            isLogWindowOpen = false;
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

        public Task ThrowLetter(string command, string key, string file)
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


        public Task SendMessage(string messg, string key, string file = "")
        {
            downloadSuccess = null;
            refreshSuccess = null;
            IsKey = null;
            IsRegistration = null;
            IsLogin = null;
            return Task.Run(async () =>
            {
                await ThrowLetter(messg, key, file);
            });
        }


        public void CloseConnect()
        {
            Task task = ThrowLetter("4", "", "");
            task.Wait();
            IsConnect = false;
            chatSocket.Close();
        }

        public async void GetMessage()
        {
            await GetLetter();
        }

        public Task GetLetter()
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

                        if (answer.First() == "GetKey false")
                        {
                            IsKey = false;
                            MessageBox.Show("Вы ввели неверный ключ");

                        }

                        else if (answer.First() == "GetKey true")
                        {
                            IsKey = true;

                        }

                        else if (answer.First() == "DownloadFile")
                        {
                            downloadSuccess = answer[1];
                            MessageBox.Show(answer[1]);

                        }

                        else if (answer.First() == "UploadFile")
                        {
                            downloadSuccess = answer[1];
                            MessageBox.Show(answer[1]);

                        }

                        else if (answer.First() == "DeleteItem")
                        {
                            downloadSuccess = answer[1];
                            MessageBox.Show(answer[1]);
                        }
                        else if (answer.First() == "CreateFolder")
                        {
                            downloadSuccess = answer[1];
                            MessageBox.Show(answer[1]);
                        }

                        else if (answer.First() == "GetLog true")
                        {
                            downloadSuccess = answer[1];
                            isLogWindowOpen = true;
                            LogList = answer;
                        }

                        else if (answer.First() == "GetLog false")
                        {
                            downloadSuccess = answer[1];
                            MessageBox.Show(answer[1]);
                        }

                        else if (answer.First() == "Registration true")
                        {
                            IsRegistration = true;
                            MessageBox.Show(answer[1]);
                        }

                        else if (answer.First() == "Registration false")
                        {
                            IsRegistration = false;
                            MessageBox.Show(answer[1]);
                        }

                        else if (answer.First() == "Login false")
                        {
                            IsLogin = false;
                            downloadSuccess = answer[1];
                            MessageBox.Show(answer[1]);
                        }

                        else if (answer.First() == "Login true")
                        {
                            SendMessage("GetFiles", "");
                        }



                        else if (fileWays.First().Key == "fileList")
                        {
                            refreshSuccess = "ha";
                            IsLogin = true;
                            fileList = fileWays;
                        }

                    }
                }

                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }
    }
}
