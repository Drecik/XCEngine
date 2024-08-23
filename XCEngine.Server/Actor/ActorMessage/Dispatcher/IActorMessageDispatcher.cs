namespace XCEngine.Server
{
    /// <summary>
    /// Actor消息派发起，用来接收处理Actor的消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IActorMessageDispatcher
    {
        public void OnMessage(object actor, ActorMessage actorMessage);
    }
}
