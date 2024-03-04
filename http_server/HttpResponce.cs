using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace http_server
{
    internal class HttpResponce : HttpMessage
    {
        public string? HttpVersion { get; private set; }
        public HttpStatusCode? StatusCode { get; private set; }
        public string? ReasonPhrase { get; private set; }

        public HttpResponce(string RawStringMessage)
            : base(RawStringMessage)
        {

        }
        public HttpResponce(string HttpVersion, HttpStatusCode StatusCode)
        {
            this.HttpVersion = HttpVersion;
            this.StatusCode = StatusCode;
            ReasonPhrase = Enum.GetName(StatusCode);
        }


        // StatusLine = HTTP-Version SP Status-Code SP Reason-Phrase CRLF
        protected override void ProcessStartLine(string StatusLine)
        {
            if (string.IsNullOrWhiteSpace(StatusLine))
            {
                throw new ArgumentException($"\"{nameof(StatusLine)}\" cannot be NULL or whitespace.", nameof(StatusLine));
            }

            string[] SplittedStatusLine = StatusLine.Split(' ');

            HttpVersion = SplittedStatusLine[0] ?? throw new ArgumentNullException($"{HttpVersion} cannot be null");
            StatusCode = Enum.TryParse(SplittedStatusLine[1], out HttpStatusCode Parsed) ? Parsed : throw new ArgumentNullException($"{StatusCode} cannot be null");
            ReasonPhrase = SplittedStatusLine[2] ?? throw new ArgumentNullException($"{ReasonPhrase} cannot be null");
        }


        public static HttpResponce NotImplemented(string HttpVersion)
        {
            return new HttpResponce(HttpVersion, HttpStatusCode.NotImplementes);
        }
    }
}
