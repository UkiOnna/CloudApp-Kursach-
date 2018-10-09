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


        public async void SendMessage(string messg,string key)
        {
            await ThrowLetter(messg,key);
        }


        public void CloseConnect()
        {
            ThrowLetter("4","");
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
                        byte[] buffer = new byte[1024];

                        StringBuilder stringBuilder = new StringBuilder();
                        do
                        {
                            bytes = chatSocket.Receive(buffer);
                            stringBuilder.Append(Encoding.Default.GetString(buffer, 0, bytes));
                        }
                        while (chatSocket.Available > 0);

                        if (stringBuilder.ToString() == "GetKey false")
                        {
                            MessageBox.Show("Вы ввели неверный ключ");
                        }
                        else
                        {
                            IsKey = true;
                            MessageBox.Show("Ключ прошел проверку!");
                            Name = stringBuilder.ToString();
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
