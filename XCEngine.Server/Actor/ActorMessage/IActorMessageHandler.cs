namespace XCEngine.Server
{
    /// <summary>
    /// Actor消息句柄，用来处理Actor的消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IActorMessageHandler
    {
        public void OnMessage(object actor, ActorMessage actorMessage);
    }
}
