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
        private string name;
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

                            try
                            {
                                var task = Task.Run(dfd);
                                task.Wait();
                                Console.WriteLine("Получен ключ");
                                newMessage.Key = "";
                                newMessage.Command = name;
                                sockets[0].Send(Encoding.Default.GetBytes(newMessage.Command + "" + newMessage.Key));
                            }
                            catch (System.AggregateException)
                            {
                                Console.WriteLine("Неверный ключ");
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
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch(ArgumentOutOfRangeException ex)
                {
                    
                }
            });
        }

        public async Task dfd()
        {
            using (var dropBox = new DropboxClient(key))
            {
                var id = await dropBox.Users.GetCurrentAccountAsync();
                Console.WriteLine(id.Name.DisplayName);
                name=id.Name.DisplayName;
                //Здесь в базу данных айди логин пользователя
            }
        }

    }
}
