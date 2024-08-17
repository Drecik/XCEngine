internal class TestActor2
{
}

[ActorMessageHandler(typeof(TestActor2))]
internal class TestActor2MessageHandler : IActorMessageHandler
{
    public void OnMessage(object actor, ActorMessage actorMessage)
    {
        if (actorMessage.MessageType == ActorMessage.EMessageType.Create)
        {
            Log.Info("Started");
        }

        if (actorMessage.MessageType == ActorMessage.EMessageType.Destroy)
        {
            Log.Info("Destroy");
        }

        if (actorMessage.MessageType == ActorMessage.EMessageType.Send)
        {
            if (actorMessage.MessageId == "TestSend")
            {
                var (arg1, arg2, arg3) = Actor.MessageSerializer.Deserialize<int, string, double>(actorMessage.MessageData as byte[]);
                Log.Info($"Receive send, {arg1}, {arg2}, {arg3}");
            }
        }
    }
}