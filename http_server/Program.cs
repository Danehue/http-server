using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Init");

TcpListener server = new TcpListener(IPAddress.Any, 4221);

server.Start();

using (Socket socket = server.AcceptSocket())
{
    byte[] RequestBuff = new byte[1024];
    _ = socket.Receive(RequestBuff);
    string RequestString = Encoding.ASCII.GetString(RequestBuff);

    string HttpPath = ExtractHttpPath(RequestString);

    byte[] ResponceBuff = new byte[1024];
    if (HttpPath == "/")
    {
        ResponceBuff = Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\n\r\n");
    } 
    else
    {
        ResponceBuff = Encoding.ASCII.GetBytes("HTTP/1.1 404 Not Found\r\n\r\n");
    }
    socket.Send(ResponceBuff);
    // Console.WriteLine(Encoding.ASCII.GetString(ResponceBuff));

}

string ExtractHttpPath(string RequestString)
{
    string[] RequestSplited = RequestString.Split('\n');
    var RequestLine = RequestSplited[0].Split(' ');
    var Path = RequestLine[1];
    return Path;
}