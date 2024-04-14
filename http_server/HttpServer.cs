using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
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
            "POST",
            "HEAD",
        };
        private readonly Encoding DefaultEncoding = Encoding.ASCII;
        private readonly string ServerHttpVersion = "HTTP/1.1";
        public string? Dir { get; }
        public HttpServer(IPAddress Ip, ushort Port, ILogger Logger, string? Dir)
            : base(Ip, Port, Logger)
        {
            this.Dir = Dir;
        }

        // RFC 2616
        // Request-Line = Method SP Request-URI SP HTTP-Version CRLF
        // Status-Line  = HTTP-Version SP Status-Code SP Reason-Phrase CRLF
        protected override async Task ProcessRequestAsync(Socket socket)
        {
            try
            {
                byte[] RequestBuff = new byte[MaxRecievedBytes];
                int RecievedBytes = await socket.ReceiveAsync(RequestBuff, SocketFlags.None);
                RequestBuff = RequestBuff[0..RecievedBytes];

                Logger.LogInformation($"Thread {Thread.CurrentThread.Name} {Thread.CurrentThread.ManagedThreadId} processing");
                string RequestString = DefaultEncoding.GetString(RequestBuff);
                Logger.LogInformation($"{nameof(RequestString)}: {RequestString}");

                HttpRequest ParsedString = new HttpRequest(RequestString);
                if (!SuportedMethods.Contains(ParsedString.Method))
                {
                    RequestBuff = DefaultEncoding.GetBytes(HttpResponce.NotImplemented(ServerHttpVersion).ToString());
                    await Send(RequestBuff, socket);
                    return;
                }
                switch (ParsedString.RequestUri.ToLowerInvariant())
                {
                    case Routes.Base:
                        await Send(DefaultEncoding.GetBytes(HttpResponce.Ok(ServerHttpVersion).ToString()), socket);
                        break;
                    case var uri when uri.StartsWith(Routes.Echo):
                        await Echo(ParsedString);
                        break;
                    case var uri when uri.StartsWith(Routes.UserAgent):
                        await UserAgent(ParsedString);
                        break;
                    case var uri when uri.StartsWith(Routes.Files):
                        await Files(ParsedString);
                        break;
                    default:
                        await Send(DefaultEncoding.GetBytes(HttpResponce.NotFound(ServerHttpVersion).ToString()), socket);
                        break;
                }
               
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

                socket.Close();
            }

            async Task Echo(HttpRequest ParsedString)
            {
                string Body = ParsedString.RequestUri[Routes.Echo.Length..];
                int BodyLength = Body?.Length ?? 0;
                byte[] RequestBuff = DefaultEncoding.GetBytes(HttpResponce.Ok(ServerHttpVersion, HttpHeaders.GetHeaders(HttpHeaders.TextPlain, BodyLength), Body)
                    .ToString());
                await Send(RequestBuff, socket);
                return;
            }

            async Task UserAgent(HttpRequest ParsedString)
            {
                string Body = ParsedString.Headers[HttpHeaders.UserAgent];
                int BodyLength = Body?.Length ?? 0;
                byte[] RequestBuff = DefaultEncoding.GetBytes(HttpResponce.Ok(ServerHttpVersion, HttpHeaders.GetHeaders(HttpHeaders.TextPlain, BodyLength), Body)
                    .ToString());
                await Send(RequestBuff, socket);
                return;
            }

            async Task Files(HttpRequest ParsedString)
            {
                if (Dir is null)
                {
                    Logger.LogError("Dir is null, can't handle files");
                    await Send(DefaultEncoding.GetBytes(HttpResponce.BadRequest(ServerHttpVersion).ToString()), socket);
                    return;
                }
                string[]? SplittedUri = ParsedString.RequestUri?.Split(Routes.Files);
                if (SplittedUri is null || SplittedUri.Length < 2)
                {
                    Logger.LogError("Expected a file name");
                    return;
                }
                string TargetFile = Path.Combine(Dir, SplittedUri[1]);
                if (ParsedString.IsGet)
                {
                    if (File.Exists(TargetFile))
                    {
                        string FileContents = await File.ReadAllTextAsync(TargetFile);
                        int ContentLength = FileContents?.Length ?? 0;
                        byte[] FileResponce = DefaultEncoding.GetBytes(HttpResponce.Ok(ServerHttpVersion, HttpHeaders.GetHeaders(HttpHeaders.OctetStream, ContentLength), FileContents)
                            .ToString());
                        await Send(FileResponce, socket);
                    }
                    else
                    {
                        // 404 not foud
                        await Send(DefaultEncoding.GetBytes(HttpResponce.NotFound(ServerHttpVersion).ToString()), socket);
                    }
                }
                else if (ParsedString.IsPost)
                {
                    await File.WriteAllTextAsync(TargetFile, ParsedString.Body);
                    await Send(DefaultEncoding.GetBytes(HttpResponce.Created(ServerHttpVersion).ToString()), socket);
                }
                else
                {
                    throw new Exception($"The method {ParsedString.Method} is not sopported");
                }
                
            }
        }

        protected override Task Send(byte[] Responce, Socket socket)
        {
            return socket.SendAsync(Responce, SocketFlags.None);
        }

    }
}
