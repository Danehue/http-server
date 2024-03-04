using http_server;
using System.Net;
using System.Net.Sockets;
using System.Text;

var Server = new HttpServer(IPAddress.Any, 4221, null);
Console.WriteLine(Server.ToString());

//Console.WriteLine("Init");

//TcpListener server = new TcpListener(IPAddress.Any, 4221);

//server.Start();

//using (Socket socket = server.AcceptSocket())
//{
//    byte[] RequestBuff = new byte[1024];
//    _ = socket.Receive(RequestBuff);
//    string RequestString = Encoding.ASCII.GetString(RequestBuff);

//    string HttpPath = ExtractHttpPath(RequestString);

//    byte[] ResponceBuff = new byte[1024];
//    if (HttpPath.StartsWith("/echo") || HttpPath.StartsWith('/'))
//    {
//        string HttpRouth = ExtractHttpRouth(HttpPath);
//        ResponceBuff = Encoding.ASCII.GetBytes($"HTTP/1.1 200 OK\r\nConstext-Type: text/plain\r\nConstent-Length: {HttpRouth.Length}\r\n\r\n{HttpRouth}\r\n");
//    }
//    else
//    {
//        ResponceBuff = Encoding.ASCII.GetBytes("HTTP/1.1 404 Not Found\r\n\r\n");
//    }
//    socket.Send(ResponceBuff);
//    // Console.WriteLine(Encoding.ASCII.GetString(ResponceBuff));

//}

string ExtractHttpRouth(string HttpPath)
{
    string[] HttpRouth = HttpPath.Split('/');
    string Routh = "";
    if (HttpRouth.Length == 2)
    {
        Routh = HttpRouth[1];
    }
    else
    {
        Routh = HttpRouth[2];
    }
    return Routh;
}

string ExtractHttpPath(string RequestString)
{
    string[] RequestSplited = RequestString.Split('\n');
    var RequestLine = RequestSplited[0].Split(' ');
    string Path = RequestLine[1];
    return Path;
}