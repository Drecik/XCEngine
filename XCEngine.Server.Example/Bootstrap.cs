using XCEngine.Core;

internal class Bootstrap
{

}

[ActorMessageHandler(typeof(Bootstrap))]
internal class BootstrapMessageHandler : IActorMessageHandler
{
    public void OnMessage(object actor, ActorMessage actorMessage)
    {
        Log.Info("Started");
    }
}