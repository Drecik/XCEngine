namespace XCEngine.Server
{
    /// <summary>
    /// Actor上下文
    /// </summary>
    internal class ActorContext
    {
        public int ActorId { get; set; }
        public object ActorObject { get; set; }

        private Queue<ActorMessage> _messageQueue = new();

        #region Message处理

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="message"></param>
        public void PushMessage(ActorMessage message)
        {
            lock (_messageQueue)
            {
                _messageQueue.Enqueue(message);
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
                    return null;
                }

                return _messageQueue.Dequeue();
            }
        }

        #endregion
    }
}
