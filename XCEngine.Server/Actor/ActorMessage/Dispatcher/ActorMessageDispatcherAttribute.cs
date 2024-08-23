namespace XCEngine.Server
{
    /// <summary>
    /// ActorMessage消息派发器属性
    /// </summary>
    public class ActorMessageDispatcherAttribute : Attribute
    {
        public Type ActorType;

        public ActorMessageDispatcherAttribute(Type actorType)
        {
            ActorType = actorType;
        }
    }
}
