﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        protected override byte[] ProcessRequest(byte[] RequestBytes)
        {
            string RequestString = DefaultEncoding.GetString(RequestBytes);
            Logger.LogInformation($"{nameof(RequestString)}: {RequestString}");

            HttpRequest ParsedString = new HttpRequest(RequestString); 
            if (!SuportedMethods.Contains(ParsedString.Method))
            {
                return DefaultEncoding.GetBytes(HttpResponce.NotImplemented(ServerHttpVersion).ToString());
            }
            if (ParsedString.RequestUri.ToLowerInvariant().StartsWith("/echo/"))
            {
                var Responce = HandleEcho(ParsedString.RequestUri);
                return DefaultEncoding.GetBytes(HttpResponce.Ok(
                    ServerHttpVersion,
                    new Dictionary<string, string>
                    {
                        {"Content-Type", "text/plain" },
                        {"Content-Length", Responce?.Length.ToString() ?? "0" } // Dictionary value could be null
                    },
                    Responce).ToString());
            }
            return DefaultEncoding.GetBytes(HttpResponce.NotFound(ServerHttpVersion).ToString());
        }

        private string HandleEcho(string RequestUri)
        {
            var ToEcho = RequestUri["/echo/".Length..];
            return ToEcho;
        }
    }
}