namespace XCEngine.Server
{
    /// <summary>
    /// Actor消息句柄，用来处理Actor的消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ActorMessageHandle<T> where T : class
    {
        public abstract void OnMessage(T actor, ActorMessage actorMessage);
    }
}
