namespace XCEngine.Server
{
    /// <summary>
    /// ActorMessage消息执行方法
    /// </summary>
    public class ActorMessageHandlerMethodAttribute : Attribute
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        public string MessageId;

        public ActorMessageHandlerMethodAttribute(string messageId)
        {
            MessageId = messageId;
        }
    }
}
