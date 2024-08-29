[UseCommonActorMessageDispatcher]
internal class SocketServerActor
{
    public int ListenFd = 0;
    public Dictionary<int, int> ConnectionDict = new();
}


[ActorMessageHandler(typeof(SocketServerActor))]
internal static class SocketServerActorMessageHandler
{
    [ActorMessageHandlerMethod(nameof(OnCreate))]
    public static void OnCreate(this SocketServerActor self, string ip, int port, int backlog)
    {
        self.ListenFd = TcpSocket.Listen(ip, port, backlog);
        if (self.ListenFd == 0)
        {
            Log.Error("Listen failed");
            return;
        }

        TcpSocket.StartAccept(self.ListenFd);
    }

    [ActorMessageHandlerMethod(nameof(OnDestroy))]
    public static void OnDestroy(this SocketServerActor self)
    {
        TcpSocket.Close(self.ListenFd);
    }

    [ActorMessageHandlerMethod(nameof(OnAccept))]
    public static void OnAccept(this SocketServerActor self, int listenFd, int newFd)
    {
        Log.Info($"{listenFd}, new connection, {newFd}");
        TcpSocket.StartReceive(newFd, new DefaultNetPackageSerializerFactory(new DefaultNetPackageFactory()).CreateNetPackageSerializer());
    }

    [ActorMessageHandlerMethod(nameof(OnReceive))]
    public static void OnReceive(this SocketServerActor self, int fd, INetPackage package)
    {
        DefaultNetPackage netPackage = package as DefaultNetPackage;
        BinaryReader br = new BinaryReader(new MemoryStream(netPackage.Data));

        Log.Info($"{fd}, Receive package: {br.ReadString()}");

        netPackage = new DefaultNetPackage();

        var ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write("reply package...");
        netPackage.Data = ms.ToArray();
        TcpSocket.Send(fd, netPackage);
    }

    [ActorMessageHandlerMethod(nameof(OnClose))]
    public static void OnClose(this SocketServerActor self, int fd)
    {
        Log.Info($"{fd}, Close");
    }

    [ActorMessageHandlerMethod(nameof(OnError))]
    public static void OnError(this SocketServerActor self, int fd, int errorId, string errorDesc)
    {
        Log.Info($"{fd}, Error, {errorId}, {errorDesc}");
    }
}