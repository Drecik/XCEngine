internal class SocketServerActor
{
    public int ListenFd = 0;
    public Dictionary<int, int> ConnectionDict = new();
}

internal class SocketServerActorMessageDispatcher : CommonActorMessageDispatcher<SocketServerActor>
{
}

[ActorMessageHandler(typeof(SocketServerActor))]
internal static class SocketServerActorMessageHandler
{
    [ActorMessageHandlerMethod(nameof(OnCreate))]
    internal static void OnCreate(this SocketServerActor self)
    {
        self.ListenFd = TcpSocket.Listen("127.0.0.1", 9999, 100);
        if (self.ListenFd == 0)
        {
            Log.Error("Listen failed");
            return;
        }

        TcpSocket.StartAccept(self.ListenFd);
    }

    [ActorMessageHandlerMethod(nameof(OnDestroy))]
    internal static void OnDestroy(this SocketServerActor self)
    {
        TcpSocket.Close(self.ListenFd);
    }

    [ActorMessageHandlerMethod(nameof(OnAccept))]
    internal static void OnAccept(this SocketServerActor self, int listenFd, int newFd)
    {
        Log.Info($"{listenFd}, new connection, {newFd}");
        TcpSocket.StartReceive(newFd, new DefaultNetPackageSerializerFactory(new DefaultNetPackageFactory()));
    }

    [ActorMessageHandlerMethod(nameof(OnReceive))]
    internal static void OnReceive(this SocketServerActor self, int fd, INetPackage package)
    {
        Log.Info($"{fd}, Receive package");
    }

    [ActorMessageHandlerMethod(nameof(OnClose))]
    internal static void OnClose(this SocketServerActor self, int fd)
    {
        Log.Info($"{fd}, Close");
    }
}