namespace XCEngine.Server
{
    /// <summary>
    /// 同步上下文对象
    /// </summary>
    internal class SynchronizationContext : System.Threading.SynchronizationContext
    {
        /// <summary>
        /// Actor Id
        /// </summary>
        public int ActorId;

        /// <summary>
        /// 消息的Sesion Id
        /// </summary>
        public int SessionId;

        public SynchronizationContext(int actorId, int sessionId)
        {
            ActorId = actorId;
            SessionId = sessionId;
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            Actor.PushMessage(ActorId, new ActorMessage()
            {
                MessageType = ActorMessage.EMessageType.AwaitCallback,
                SessionId = SessionId,
                MessageData = new object[2] { d, state }
            });
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            Actor.PushMessage(ActorId, new ActorMessage()
            {
                MessageType = ActorMessage.EMessageType.AwaitCallback,
                SessionId = SessionId,
                MessageData = new object[2] { d, state }
            });
        }
    }
}