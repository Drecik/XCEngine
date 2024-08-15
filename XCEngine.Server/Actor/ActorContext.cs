namespace XCEngine.Server
{
    /// <summary>
    /// Actor上下文，存放一个Actor必要的数据
    /// </summary>
    internal class ActorContext
    {
        /// <summary>
        /// Actor Id
        /// </summary>
        public int ActorId { get; set; }

        /// <summary>
        /// Actor实例对象
        /// </summary>
        public object ActorObject { get; set; }

        /// <summary>
        /// 消息队列
        /// </summary>
        public ActorMessageQueue ActorMessageQueue { get; set; }

        /// <summary>
        /// 消息派发回调
        /// </summary>
        public Action<object, ActorMessage> MessageHandler { get; set; }
    }
}
