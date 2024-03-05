using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Text;
//using http_server;

namespace http_server
{
    internal abstract class TcpServer
    {
        public IPAddress Ip { get; }
        public ushort Port { get; }
        public ILogger Logger { get; }
        protected int MaxRecievedBytes { get; set; }

        public TcpServer(IPAddress Ip, ushort Port, ILogger Logger)
        {
            this.Ip = Ip;
            this.Port = Port;
            this.Logger = Logger;
            this.MaxRecievedBytes = 1024;
        }

        public void Start()
        {
            TcpListener Server = new TcpListener(Ip, Port);
            Server.Start();
            Logger.LogInformation($"Started server on {Ip}:{Port}");
            using (Socket socket = Server.AcceptSocket())
            {
                byte[] RequestBuff = new byte[MaxRecievedBytes];
                int RecievedBytes = socket.Receive(RequestBuff);
                byte[] Responce = ProcessRequest(RequestBuff);
                socket.Send(Responce);
            }
        }

        protected abstract byte[] ProcessRequest(byte[] RequestBuff);

    }
}







