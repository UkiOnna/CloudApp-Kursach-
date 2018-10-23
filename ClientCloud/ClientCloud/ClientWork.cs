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
        public Dictionary<string,string> fileList { get; set; }
        private bool isStrings;
        public string downloadSuccess { get; set; }
        public string refreshSuccess { get; set; }
        public string uploadSuccess { get; set; }
        public bool isWorking { get; set; }
        public ClientWork()
        {
            isWorking = false;
            chatSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3535);
            IsKey = null;
            isStrings = true;
            downloadSuccess = null;
            uploadSuccess = null;
            refreshSuccess = null;
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

        public Task ThrowLetter(string command,string key,string file)
        {
            return Task.Run(() =>
            {
                try
                {
                    string serialized = JsonConvert.SerializeObject(new Message { Command = command, Key = key,FileWay=file });
                    chatSocket.Send(Encoding.Default.GetBytes(serialized));
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }


        public Task SendMessage(string messg,string key,string file="")
        {
            if (messg == "GetFiles")
            {
                refreshSuccess = null;
            }
            return Task.Run(async () => {
                await ThrowLetter(messg,key,file);
            });
        }


        public void CloseConnect()
        {
           Task task= ThrowLetter("4","","");
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
                        byte[] buffer = new byte[90000000];

                        List<string> answer = new List<string>();
                        Dictionary<string, string> fileWays = new Dictionary<string, string>();
                        do
                        {
                            bytes = chatSocket.Receive(buffer);
                            //answer.Append(Encoding.Default.GetString(buffer, 0, bytes));
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
                        else if(answer.First()=="GetKey true")
                        {
                            SendMessage("GetFiles", "");
                            Name = answer[1];
                        }
                        else if (answer.First() =="DownloadFile")
                        {
                            downloadSuccess = answer[1];
                            MessageBox.Show(answer[1]);
                            downloadSuccess = null;
                        }
                        else if (answer.First() == "UploadFile")
                        {
                            downloadSuccess = answer[1];
                            MessageBox.Show(answer[1]);
                            downloadSuccess = null;
                        }
                        else if (fileWays.First().Key=="fileList")
                        {
                           refreshSuccess = "ha";
                            fileList = fileWays;
                            IsKey = true;
                        }
                    }
                }
                
                catch(SocketException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }
    }
}
