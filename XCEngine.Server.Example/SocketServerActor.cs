using System;
using System.Linq;

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
    internal static void OnCreate(this SocketServerActor @this)
    {
        @this.ListenFd = TcpSocket.Listen("127.0.0.1", 9999, 100);
        if (@this.ListenFd == 0)
        {
            Log.Error("Listen failed");
            return;
        }

        TcpSocket.StartAccept(@this.ListenFd);
    }

    [ActorMessageHandlerMethod(nameof(OnDestroy))]
    internal static void OnDestroy(this SocketServerActor @this)
    {
        TcpSocket.Close(@this.ListenFd);
    }
}