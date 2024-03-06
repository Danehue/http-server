using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace http_server
{
    internal abstract class HttpMessage
    {
        public string RawStringMessage { get; }
        protected string StartLine { get; set; }
        public string? Body { get; set; }
        public Dictionary<string, string> Headers { get; protected set; } = new Dictionary<string,string>();
        private readonly string CRLF = "\r\n";
        private readonly string AnyCharacter = "[^ ]*";

        public HttpMessage(string RawStringMessage)
        {
            if (string.IsNullOrEmpty(RawStringMessage))
            {
                throw new ArgumentException($"'{nameof(RawStringMessage)}' cannot be NULL or whitespace.", nameof(RawStringMessage));
            }

            this.RawStringMessage = RawStringMessage;

            string[] MessageLines = { };
            // Header fields can be extended over multiple lines by preceding each extra line with at least one SP or HT
            if (Regex.IsMatch(RawStringMessage, $@"{CRLF}{AnyCharacter}"))
            {
                // Split HttpMessage
                MessageLines = Regex.Split(RawStringMessage, $@"{CRLF}");
            }

            Debug.Assert(MessageLines.Length > 0, "HTTP message should have at least a Start Line.");
            StartLine = MessageLines[0];
            ProcessStartLine(StartLine);
            ProcessHeaders(MessageLines);

        }
        public HttpMessage() { }

        protected void ProcessHeaders(string[] StringMessage)
        {
            
            foreach (var line in StringMessage.Skip(1))
            {
                if (line == "") break;
                if (line.Contains(':'))
                {
                    string[] SplittedHeader = line.Split(": ");
                    Headers.Add(SplittedHeader[0], SplittedHeader[1].Trim());
                }
            }
        }

        protected abstract void ProcessStartLine(string StartLine);

        public override string ToString() => $"{StartLine}{CRLF}" +
                                             $"{string.Join(CRLF,Headers.Select(kvp => $"{kvp.Key}:{kvp.Value}"))}" +
                                             $"{CRLF}{CRLF}" +
                                             $"{Body}";
        
    }
}
