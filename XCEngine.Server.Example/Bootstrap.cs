using XCEngine.Core;

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
            Log.Info("Started");

            Actor.Exit();
        }
        
        if (actorMessage.MessageType == ActorMessage.EMessageType.Destroy)
        {
            Log.Info("Destroy");
        }
    }
}