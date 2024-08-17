internal class TestActor1
{
}

[ActorMessageHandler(typeof(TestActor1))]
internal class TestActor1MessageHandler : IActorMessageHandler
{
    public void OnMessage(object actor, ActorMessage actorMessage)
    {
        if (actorMessage.MessageType == ActorMessage.EMessageType.Create)
        {
            var (arg1, arg2, arg3) = Actor.MessageSerializer.Deserialize<int, string, double>(actorMessage.MessageData as byte[]);
            Log.Info($"Started, {arg1}, {arg2}, {arg3}");
            Log.Info("Exit self");
            Actor.Exit();
        }

        if (actorMessage.MessageType == ActorMessage.EMessageType.Destroy)
        {
            Log.Info("Destroy");
        }
    }
}