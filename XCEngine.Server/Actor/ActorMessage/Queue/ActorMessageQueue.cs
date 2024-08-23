namespace XCEngine.Server
{
    /// <summary>
    /// 单个Actor的消息队列
    /// </summary>
    internal class ActorMessageQueue
    {
        public int ActorId;

        /// <summary>
        /// 是否在全局队列中
        /// </summary>
        private bool _inGlobalQueue = false;

        /// <summary>
        /// Actor消息队列
        /// </summary>
        private Queue<ActorMessage> _messageQueue = new();

        /// <summary>
        /// 消息数量
        /// </summary>
        public int Count => _messageQueue.Count;

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="message"></param>
        public void PushMessage(ActorMessage message)
        {
            lock (_messageQueue)
            {
                _messageQueue.Enqueue(message);

                if (_inGlobalQueue == false)
                {
                    _inGlobalQueue = true;
                    GlobalActorMessageQueue.Push(this);
                }
            }
        }

        /// <summary>
        /// 取一个消息
        /// </summary>
        /// <returns></returns>
        public ActorMessage PopMessage()
        {
            lock (_messageQueue)
            {
                if (_messageQueue.Count == 0)
                {
                    _inGlobalQueue = false;
                    return null;
                }

                return _messageQueue.Dequeue();
            }
        }
    }
}
