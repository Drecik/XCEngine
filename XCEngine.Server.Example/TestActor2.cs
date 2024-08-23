internal class TestActor2
{
}

[ActorMessageHandler(typeof(TestActor2))]
internal class TestActor2MessageHandler : IActorMessageHandler
{
    public async void OnMessage(object actor, ActorMessage actorMessage)
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

        if (actorMessage.MessageType == ActorMessage.EMessageType.Call)
        {
            if (actorMessage.MessageId == "TestCall")
            {
                var (arg1, arg2, arg3) = Actor.MessageSerializer.Deserialize<int, string, double>(actorMessage.MessageData as byte[]);
                Log.Info($"Receive call, {arg1}, {arg2}, {arg3}");

                await Task.Delay(1000);
                Actor.Return(Actor.MessageSerializer.Serialize(1));
            }
        }
    }
}