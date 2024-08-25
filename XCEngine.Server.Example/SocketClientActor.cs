internal class SocketClientActor
{
}

internal class SocketClientActorMessageDispatcher : CommonActorMessageDispatcher<SocketServerActor>
{
}

[ActorMessageHandler(typeof(SocketClientActor))]
internal static class SocketClientActorMessageHandler
{
    
}