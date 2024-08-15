namespace XCEngine.Server
{
    /// <summary>
    /// ActorMessage处理句柄
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
