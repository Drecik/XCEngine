[UseCommonActorMessageDispatcher]
internal class SocketClientActor
{
    public int ConnectionFd;
}

[ActorMessageHandler(typeof(SocketClientActor))]
internal static class SocketClientActorMessageHandler
{
    [ActorMessageHandlerMethod(nameof(OnCreate))]
    public static async Task OnCreate(this SocketClientActor self)
    {
        self.ConnectionFd = await TcpSocket.Connect("127.0.0.1", 9999);
        if (self.ConnectionFd == 0)
        {
            Log.Error("Connect failed");
            return;
        }

        TcpSocket.StartReceive(self.ConnectionFd, new DefaultNetPackageSerializerFactory(new DefaultNetPackageFactory()).CreateNetPackageSerializer());
    }

    [ActorMessageHandlerMethod(nameof(OnReceive))]
    public static void OnReceive(this SocketClientActor self, int fd, INetPackage package)
    {
        DefaultNetPackage netPackage = package as DefaultNetPackage;
        BinaryReader br = new BinaryReader(new MemoryStream(netPackage.Data));

        Log.Info($"{fd}, Receive package: {br.ReadString()}");
    }

    [ActorMessageHandlerMethod(nameof(OnClose))]
    public static void OnClose(this SocketClientActor self, int fd)
    {
        Log.Info($"{fd}, Close");
    }

    [ActorMessageHandlerMethod(nameof(SendTestPackage))]
    public static void SendTestPackage(this SocketClientActor self)
    {
        DefaultNetPackage netPackage = new DefaultNetPackage();
        netPackage.ProtoId = 100;
        netPackage.SessionId = 0;

        var ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write("send package...");
        netPackage.Data = ms.ToArray();
        TcpSocket.Send(self.ConnectionFd, netPackage);
    }

    [ActorMessageHandlerMethod(nameof(Disconnect))]
    public static void Disconnect(this SocketClientActor self)
    {
        TcpSocket.Close(self.ConnectionFd);
    }

    [ActorMessageHandlerMethod(nameof(OnError))]
    public static void OnError(this SocketClientActor self, int fd, int errorId, string errorDesc)
    {
        Log.Info($"{fd}, Error, {errorId}, {errorDesc}");
    }
}