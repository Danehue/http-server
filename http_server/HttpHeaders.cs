using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace http_server
{
    internal class HttpHeaders
    {
        // Header keys
        public const string ContentType = "Content-Type";
        public const string ContentLength = "Content-Length";
        public const string UserAgent = "User-Agent";

        // Header values
        public const string TextPlain = "text/plain";
        public const string OctetStream = "application/octet-stream";

        public static Dictionary<string, string> GetHeaders(string ContentTypeValue, int BodyLength)
        {
            return new Dictionary<string, string>
            {
                {ContentType, ContentTypeValue},
                {ContentLength, BodyLength.ToString() ?? "0"} // Dictionary value could be null
            };
        } 
    }
}