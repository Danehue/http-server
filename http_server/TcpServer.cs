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
        public bool ShouldStop { get; private set; } = false;
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
            while (!ShouldStop)
            {
                Socket socket = Server.AcceptSocket();
                _ = Task.Run(async () => await ProcessRequestAsync(socket));
            }
        }

        public void Stop()
        {
            ShouldStop = true;
        }

        protected abstract Task ProcessRequestAsync(Socket socket);

        protected abstract Task Send(byte[] RequestBuff, Socket socket);
    }
}







