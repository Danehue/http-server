using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace http_server
{
    [AttributeUsage(AttributeTargets.Field)]
    public class HttpStatusCodeAttribute(string StatusCode) : Attribute
    {
        public string StatusCode { get; } = StatusCode;
    }
    public enum HttpStatusCode
    {
        [HttpStatusCode("OK")]
        OK = 200,

        [HttpStatusCode("Not Found")]
        NotFound = 404,

        [HttpStatusCode("Not Implemented")]
        NotImplemented = 501,

        [HttpStatusCode("Bad Request")]
        BadRequest = 400
    }
    public static class HttpStatusCodeExtension
    {
        public static string GetHttpStatusCodeName(HttpStatusCode statusCode)
        {
            var FieldInfo = typeof(HttpStatusCode).GetField(statusCode.ToString());
            var attribute = FieldInfo.GetCustomAttribute<HttpStatusCodeAttribute>();
            return attribute?.StatusCode ?? statusCode.ToString();
        }
    }

}
