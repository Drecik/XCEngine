internal class Bootstrap
{
    public int SocketServer;
    public int SocketClient;
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

            bootstrap.SocketServer = Actor.Start(typeof(SocketServerActor), "127.0.0.1", 9999, 100);

            XC.Delay(1000, "Delay1");
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
                bootstrap.SocketClient = Actor.Start(typeof(SocketClientActor));

                XC.Delay(1000, "Delay2", null);
            }
            else if (actorMessage.MessageId == "Delay2")
            {
                Actor.Send(bootstrap.SocketClient, nameof(SocketClientActorMessageHandler.SendTestPackage));
                Log.Info("On delay2");
                XC.Delay(1000, "Delay3", null);
            }
            else if (actorMessage.MessageId == "Delay3")
            {
                Actor.Send(bootstrap.SocketClient, nameof(SocketClientActorMessageHandler.Disconnect));
            }
            else if (actorMessage.MessageId == "Tick")
            {
                Log.Info("On tick");
            }
        }
    }
}