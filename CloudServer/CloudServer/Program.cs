using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CloudServer
{
    //"127.0.0.1"
    class Program
    {
        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("10.2.3.90"), 3535);
            serverSocket.Bind(endPoint);
            Server server = new Server();
            server.BeginToDo(serverSocket);
        }
    }
}
