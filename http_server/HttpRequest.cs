using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace http_server
{
    internal class HttpRequest : HttpMessage
    {
        public string? Method { get; private set; }
        public string? RequestUri { get; private set; }
        public string? HttpVersion { get; private set; }

        public HttpRequest(string RawStringMessage)
            : base(RawStringMessage)
        {
        }


        // RequestLine = Method SP Request-URI SP HTTP-Version CRLF
        protected override void ProcessStartLine(string RequestLine)
        {
            if (string.IsNullOrWhiteSpace(RequestLine))
            {
                throw new ArgumentException($"\"{nameof(RequestLine)}\" cannot be NULL or whitespace.", nameof(RequestLine));
            }

            string[] SplittedRequestLine = RequestLine.Split(' ');
            Method = SplittedRequestLine[0] ?? throw new ArgumentNullException($"{Method} cannot be null");
            RequestUri = SplittedRequestLine[1] ?? throw new ArgumentNullException($"{RequestUri} cannot be null");
            HttpVersion = SplittedRequestLine[2] ?? throw new ArgumentNullException($"{HttpVersion} cannot be null");
        }


    }
}
