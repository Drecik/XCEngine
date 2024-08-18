internal class Bootstrap
{
    public int Actor1Id;
    public int Actor2Id;
}

[ActorMessageHandler(typeof(Bootstrap))]
internal class BootstrapMessageHandler : IActorMessageHandler
{
    public void OnMessage(object actor, ActorMessage actorMessage)
    {
        Bootstrap bootstrap = (Bootstrap)actor;
        if (actorMessage.MessageType == ActorMessage.EMessageType.Create)
        {
            Log.Info($"Started");

            bootstrap.Actor1Id = Actor.Start<TestActor1, int, string, double>(1, "1", 2.0);
            bootstrap.Actor2Id = Actor.Start<TestActor2>();

            XC.Delay(1000, "Delay1");
            XC.Tick(1000, 1000, "Tick");
        }
        
        if (actorMessage.MessageType == ActorMessage.EMessageType.Destroy)
        {
            Log.Info("Destroy");
        }

        if (actorMessage.MessageType == ActorMessage.EMessageType.Timer)
        {
            if (actorMessage.MessageId == "Delay1")
            {
                Log.Info("On delay1");
                Actor.Send(bootstrap.Actor2Id, "TestSend", 1, "1", 2.0);

                XC.Delay(1000, "Delay2", null);
            }
            else if (actorMessage.MessageId == "Delay2")
            {
                Log.Info("On delay2");
                Actor.Kill(bootstrap.Actor2Id);
            }
            else if (actorMessage.MessageId == "Tick")
            {
                Log.Info("On tick");
            }
        }
    }
}