using System.Net;
using System.Text;

[UseCommonActorMessageDispatcher]
internal class HttpServerActor
{
    public int ServerFd = 0;
}


[ActorMessageHandler(typeof(HttpServerActor))]
internal static class HttpServerActorMessageHandler
{
    [ActorMessageHandlerMethod(nameof(OnCreate))]
    public static void OnCreate(this HttpServerActor self, string host, int port)
    {
        self.ServerFd = TcpSocket.ListenHttp(host, port);
        if (self.ServerFd == 0)
        {
            Log.Error("Listen failed");
            return;
        }

        TcpSocket.StartAccept(self.ServerFd);
    }

    [ActorMessageHandlerMethod(nameof(OnAccept))]
    public static void OnAccept(this HttpServerActor self, int fd, HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        Log.Info($"Receive request: {request.RawUrl}");

        string responseString = "<html><body><h1>Hello, World with Http!</h1></body></html>";
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);

        // 设置响应头
        response.ContentLength64 = buffer.Length;
        response.ContentType = "text/html";

        // 发送响应
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }

    [ActorMessageHandlerMethod(nameof(OnError))]
    public static void OnError(this SocketServerActor self, int fd, int errorId, string errorDesc)
    {
        Log.Info($"{fd}, Error, {errorId}, {errorDesc}");
    }
}