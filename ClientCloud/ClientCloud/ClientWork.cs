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
        public bool IsKey { get; set; }
        public List<string> fileList { get; set; }
        public ClientWork()
        {
            chatSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3535);
            IsKey = false;
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

        public Task ThrowLetter(string command,string key)
        {
            return Task.Run(() =>
            {
                try
                {
                    string serialized = JsonConvert.SerializeObject(new Message { Command = command, Key = key });
                    chatSocket.Send(Encoding.Default.GetBytes(serialized));
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }


        public Task SendMessage(string messg,string key)
        {
            return Task.Run(async () => {
                await ThrowLetter(messg,key);
            });
        }


        public void CloseConnect()
        {
           Task task= ThrowLetter("4","");
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
                        do
                        {
                            bytes = chatSocket.Receive(buffer);
                            //answer.Append(Encoding.Default.GetString(buffer, 0, bytes));
                           answer=ConvertList.ByteArrayToList(buffer);
                        }
                        while (chatSocket.Available > 0);

                        if (answer.First() == "GetKey false")
                        {
                            MessageBox.Show("Вы ввели неверный ключ");
                        }
                        else if(answer.First()=="GetKey true")
                        {
                            IsKey = true;
                            MessageBox.Show("Ключ прошел проверку!");
                            Name = answer[1];
                        }
                        else if (answer.First()=="fileList")
                        {
                            fileList = answer;
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
