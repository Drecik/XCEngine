using System.Collections.Concurrent;

namespace XCEngine.Server
{
    /// <summary>
    // 全局消息消息队列，工作线程从这里取消息进行处理
    /// </summary>
    internal static class GlobalActorMessageQueue
    {
        /// <summary>
        /// 工作线程的竞争锁
        /// </summary>
        public static object LockMutex = new();

        private static ConcurrentQueue<ActorMessageQueue> _globalQueue = new();

        /// <summary>
        /// 将Acotr消息队列加入到全局队列中
        /// </summary>
        /// <param name="queue"></param>
        public static void Push(ActorMessageQueue queue)
        {
            _globalQueue.Enqueue(queue);
        }

        /// <summary>
        /// 从全局队列中取出Actor消息队列
        /// </summary>
        /// <returns></returns>
        public static ActorMessageQueue Pop()
        {
            _globalQueue.TryDequeue(out var queue);
            return queue;
        }
    }
}
