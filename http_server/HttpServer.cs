using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
//using http_server;

namespace http_server
{
    internal class HttpServer : TcpServer
    {
        private readonly string[] SuportedMethods =
        {
            "GET",
            "HEAD"
        };
        private readonly Encoding DefaultEncoding = Encoding.ASCII;
        private readonly string ServerHttpVersion = "HTTP/1.1";
        public HttpServer(IPAddress Ip, ushort Port, ILogger Logger)
            : base(Ip, Port, Logger)
        {
        }

        // RFC 2616
        // Request-Line = Method SP Request-URI SP HTTP-Version CRLF
        // Status-Line  = HTTP-Version SP Status-Code SP Reason-Phrase CRLF
        protected override byte[] ProcessRequest(byte[] RequestBytes, Socket socket)
        {
            byte[] ResponceBytes = [];
            try
            {
                Logger.LogInformation($"Thread {Thread.CurrentThread.Name} {Thread.CurrentThread.ManagedThreadId} processing");
                string RequestString = DefaultEncoding.GetString(RequestBytes);
                Logger.LogInformation($"{nameof(RequestString)}: {RequestString}");

                HttpRequest ParsedString = new HttpRequest(RequestString);
                if (!SuportedMethods.Contains(ParsedString.Method))
                {
                    ResponceBytes = DefaultEncoding.GetBytes(HttpResponce.NotImplemented(ServerHttpVersion).ToString());
                    return ResponceBytes;
                }
                if (ParsedString.RequestUri == "/")
                {
                    ResponceBytes = DefaultEncoding.GetBytes(HttpResponce.Ok(ServerHttpVersion).ToString());
                    return ResponceBytes;
                }
                if (ParsedString.RequestUri.ToLowerInvariant().StartsWith(Routes.Echo))
                {
                    var Body = HandleEcho(ParsedString.RequestUri);
                    ResponceBytes = DefaultEncoding.GetBytes(HttpResponce.Ok(
                        ServerHttpVersion,
                        new Dictionary<string, string>
                        {
                        {HttpHeaders.ContentType, HttpHeaders.TextPlain},
                        {HttpHeaders.ContentLength, Body?.Length.ToString() ?? "0"} // Dictionary value could be null
                        },
                        Body).ToString());
                    return ResponceBytes;
                }
                if (ParsedString.RequestUri.ToLowerInvariant().StartsWith(Routes.UserAgent))
                {
                    var Body = ParsedString.Headers[HttpHeaders.UserAgent];
                    ResponceBytes = DefaultEncoding.GetBytes(HttpResponce.Ok(
                        ServerHttpVersion,
                        new Dictionary<string, string>
                        {
                        {HttpHeaders.ContentType, HttpHeaders.TextPlain},
                        {HttpHeaders.ContentLength, Body?.Length.ToString() ?? "0"} // Dictionary value could be null
                        },
                        Body).ToString());
                    return ResponceBytes;
                }
                ResponceBytes = DefaultEncoding.GetBytes(HttpResponce.NotFound(ServerHttpVersion).ToString());
                return ResponceBytes;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Send(ResponceBytes, socket);
                socket.Close();
            }

        }

        protected override void Send(byte[] Responce, Socket socket)
        {
            socket.Send(Responce);
        }

        private static string HandleEcho(string RequestUri)
        {
            var ToEcho = RequestUri[Routes.Echo.Length..];
            return ToEcho;
        }

    }
}
