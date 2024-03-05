using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace http_server
{
    internal abstract class HttpMessage
    {
        public string RawStringMessage { get; }
        protected string StartLine { get; set; }
        public string? Body { get; set; }
        protected Dictionary<string, string>? Headers { set; get; } = new Dictionary<string,string>();
        private readonly string CRLF = "\r\n";
        public HttpMessage(string RawStringMessage)
        {
            if (string.IsNullOrEmpty(RawStringMessage))
            {
                throw new ArgumentException($"'{nameof(RawStringMessage)}' cannot be NULL or whitespace.", nameof(RawStringMessage));
            }

            this.RawStringMessage = RawStringMessage;

            string[] MessageLines = RawStringMessage.Split(CRLF);
            Debug.Assert(MessageLines.Length > 0, "HTTP message should have at least a Start Line!");
            StartLine = MessageLines[0];
            ProcessStartLine(StartLine);
        }
        public HttpMessage() { }

        protected abstract void ProcessStartLine(string startLine);

        public override string ToString() => $"{StartLine}{CRLF}" +
                                             $"{string.Join(CRLF,Headers.Select(kvp => $"{kvp.Key}:{kvp.Value}"))}" +
                                             $"{CRLF}{CRLF}" +
                                             $"{Body}";
        
    }
}
