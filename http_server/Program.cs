using http_server;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

class Program
{
    static void Main(string[] args)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger Logger = factory.CreateLogger("Program");


        string? Dir = null;
        if(args is not null && args.Length > 0 )
        {
            Logger.LogInformation($"Received {args.Length} args: {string.Join(", ", args)}");
            if (args[0] == "--directory")
            {
                if(args.Length < 2)
                {
                    Logger.LogError($"Received {args[0]} but no directory was sent");
                }
                else
                {
                    Dir = args[1];
                }
            }
        }

        var Server = new HttpServer(IPAddress.Any, 4221, Logger, Dir);
        Server.Start();

    }
}
