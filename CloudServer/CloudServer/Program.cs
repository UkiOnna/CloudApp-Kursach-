using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CloudServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(server.GetLocalIPAddress()), 3535);
            serverSocket.Bind(endPoint);
            
            server.BeginToDo(serverSocket);
        }
    }
}
