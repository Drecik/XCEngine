internal class Bootstrap
{
    public int Actor1Id;
    public int Actor2Id;
    public int Actor3Id;
}

[ActorMessageDispatcher(typeof(Bootstrap))]
internal class BootstrapMessageHandler : IActorMessageDispatcher
{
    public async void OnMessage(object actor, ActorMessage actorMessage)
    {
        Bootstrap bootstrap = (Bootstrap)actor;
        if (actorMessage.MessageType == ActorMessage.EMessageType.Create)
        {
            Log.Info($"Started");

            bootstrap.Actor1Id = Actor.Start<TestActor1, int, string, double>(1, "1", 2.0);
            bootstrap.Actor2Id = Actor.Start<TestActor2>();
            bootstrap.Actor3Id = Actor.Start<TestActor3>();

            XC.Delay(1000, "Delay1");
            XC.Tick(10000, 10000, "Tick");
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
                Actor.Send(bootstrap.Actor3Id, "TestSend", 1, "1", 2.0);

                XC.Delay(1000, "Delay2", null);
            }
            else if (actorMessage.MessageId == "Delay2")
            {
                Log.Info("On delay2");
                XC.Delay(1000, "Delay3", null);
            }
            else if (actorMessage.MessageId == "Delay3")
            {
                int ret1 = await Actor.Call<int, int, string, double>(bootstrap.Actor2Id, "TestCall", 1, "1", 2.0);
                int ret2 = await Actor.Call<int, int, string, double>(bootstrap.Actor3Id, "TestCall", 1, "1", 2.0);
                Log.Info($"Call return, {ret1}, {ret2}");
                Actor.Kill(bootstrap.Actor2Id);
                Actor.Kill(bootstrap.Actor3Id);
            }
            else if (actorMessage.MessageId == "Tick")
            {
                Log.Info("On tick");
            }
        }
    }
}