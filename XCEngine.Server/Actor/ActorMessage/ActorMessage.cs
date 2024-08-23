namespace XCEngine.Server
{
    /// <summary>
    /// Actor消息结构
    /// </summary>
    public class ActorMessage
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public enum EMessageType
        {
            /// <summary>
            /// 创建
            /// </summary>
            Create,

            /// <summary>
            /// 销毁
            /// </summary>
            Destroy,

            /// <summary>
            /// Send
            /// </summary>
            Send,

            /// <summary>
            /// Call
            /// </summary>
            Call,

            /// <summary>
            /// await回调
            /// </summary>
            AwaitCallback,

            /// <summary>
            /// 定时器消息
            /// </summary>
            Timer
        }
        public EMessageType MessageType;

        /// <summary>
        /// 发送消息方
        /// </summary>
        public int From;

        /// <summary>
        /// 消息Id
        /// </summary>
        public string MessageId;

        /// <summary>
        /// 消息数据
        /// </summary>
        public object MessageData;

        /// <summary>
        /// 消息SessionId
        /// </summary>
        public int SessionId;
    }
}
