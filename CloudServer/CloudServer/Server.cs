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

namespace CloudServer
{
    public class Server
    {
        private string key;
        private List<Socket> sockets = new List<Socket>();
        private bool isKeyRight = false;
        public Server()
        {

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
                            Console.WriteLine("Завершение");
                        }

                        else if (newMessage.Command == "GetKey")
                        {
                            Console.WriteLine("Получен ключ");
                            key = newMessage.Key;

                            var task = Task.Run(dfd);
                            if (isKeyRight == false)
                            {
                                newMessage.Key = "false";
                                sockets[0].Send(Encoding.Default.GetBytes(newMessage.Command + " " + newMessage.Key));
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        public async Task dfd()
        {
            using (var dropBox = new DropboxClient(key))
            {
                var id = await dropBox.Users.GetCurrentAccountAsync();
                Console.WriteLine(id.Name.DisplayName);
                isKeyRight = true;
                //Здесь в базу данных айди логин пользователя
            }
        }

    }
}
