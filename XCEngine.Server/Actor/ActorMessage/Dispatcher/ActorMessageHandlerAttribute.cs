namespace XCEngine.Server
{
    /// <summary>
    /// ActorMessage消息处理句柄属性
    /// </summary>
    public class ActorMessageHandlerAttribute : Attribute
    {
        public Type ActorType;

        public ActorMessageHandlerAttribute(Type actorType)
        {
            ActorType = actorType;
        }
    }
}
