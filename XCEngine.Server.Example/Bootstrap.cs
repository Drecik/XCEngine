internal class Bootstrap
{

}

[ActorMessageHandler(typeof(Bootstrap))]
internal class BootstrapMessageHandler : IActorMessageHandler
{
    public void OnMessage(object actor, ActorMessage actorMessage)
    {
        if (actorMessage.MessageType == ActorMessage.EMessageType.Create)
        {
            Log.Info($"Started");

            int actor1 = Actor.Start<TestActor1, int, string, double>(1, "1", 2.0);
            int actor2 = Actor.Start<TestActor2>();

            Actor.Send(actor2, "TestSend", 1, "1", 2.0);
            Actor.Kill(actor2);
        }
        
        if (actorMessage.MessageType == ActorMessage.EMessageType.Destroy)
        {
            Log.Info("Destroy");
        }
    }
}