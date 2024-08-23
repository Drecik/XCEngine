namespace XCEngine.Server
{
    class WorkThreadContext
    {
        /// <summary>
        /// 线程权重，用来决定一次性执行Actor多少个消息
        /// </summary>
        public int Weight = 0;

        /// <summary>
        /// 执行线程
        /// </summary>
        public Thread Thread = null;
    }

    /// <summary>
    /// 工作线程
    /// </summary>
    internal static class WorkThread
    {
        /// <summary>
        /// 是否停止
        /// </summary>
        private static bool _stop = false;

        /// <summary>
        /// 线程同步变量
        /// </summary>
        private static object _lock = new();

        /// <summary>
        /// 工作线程数组
        /// </summary>
        private static List<WorkThreadContext> _workThreadContextList = new();

        /// <summary>
        /// 睡眠的线程数
        /// </summary>
        private static int _sleepCount = 0;

        /// <summary>
        /// 线程权重
        /// </summary>
        private static int[] WEIGHTS = {
            -1, -1, -1, -1, 0, 0, 0, 0,
            1, 1, 1, 1, 1, 1, 1, 1,
            2, 2, 2, 2, 2, 2, 2, 2,
            3, 3, 3, 3, 3, 3, 3, 3
        };

        /// <summary>
        /// 启动工作线程
        /// </summary>
        /// <param name="count">工作线程数量</param>
        public static void Start(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var worker = new WorkThreadContext();
                worker.Weight = i < WEIGHTS.Length ? WEIGHTS[i] : 0;
                worker.Thread = new Thread(() => ThreadRunner(worker));
                worker.Thread.Name = $"WorkThread[{i}:D2]";
                worker.Thread.Start();
                _workThreadContextList.Add(worker);
            }
        }

        /// <summary>
        /// 运行线程
        /// </summary>
        private static void ThreadRunner(WorkThreadContext workThreadContext)
        {
            // 这一块结构仿照的是skynet
            ActorMessageQueue queue = null;
            while (_stop == false)
            {
                queue = DispatchActorMessage(workThreadContext, queue);
                if (queue == null)
                {
                    lock (_lock)
                    {
                        ++_sleepCount;
                        if (_stop == false)
                        {
                            Monitor.Wait(_lock);
                        }
                        --_sleepCount;
                    }
                }
            }
        }

        private static ActorMessageQueue DispatchActorMessage(WorkThreadContext workThreadContext, ActorMessageQueue queue)
        {
            if (queue == null)
            {
                queue = GlobalActorMessageQueue.Pop();
                if (queue == null)
                {
                    return null;
                }
            }

            var actorContext = Actor.GetActorContext(queue.ActorId);
            if (actorContext == null)
            {
                // TODO：暂时直接删除，skynet会向发送方回复消息
                return GlobalActorMessageQueue.Pop();
            }

            int n = 1;
            for (int i = 0; i < n; ++i)
            {
                ActorMessage message = queue.PopMessage();
                if (message == null)
                {
                    return GlobalActorMessageQueue.Pop();
                }
                else if (i == 0 && workThreadContext.Weight >= 0)
                {
                    // 这一块抄的skynet
                    n = queue.Count;
                    n >>= workThreadContext.Weight;
                }

                // TODO：每次new感觉不太好，看看能不能优化
                System.Threading.SynchronizationContext.SetSynchronizationContext(new SynchronizationContext(queue.ActorId, message.SessionId));

                Actor.DispatchMessage(actorContext, message);
            }

            var nextQueue = GlobalActorMessageQueue.Pop();
            if (nextQueue != null)
            {
                // 让出线程给下一个actor，自己重新push回全局队列
                GlobalActorMessageQueue.Push(queue);
                queue = nextQueue;
            }

            return queue;
        }

        /// <summary>
        /// 唤醒一个线程
        /// </summary>
        public static void WakeUp()
        {
            if (_sleepCount >= 1)
            {
                lock (_lock)
                {
                    Monitor.Pulse(_lock);
                }
            }
        }

        /// <summary>
        /// 停止所有工作线程
        /// </summary>
        public static void Stop()
        {
            lock (_lock)
            {
                _stop = true;
                Monitor.PulseAll(_lock);
            }

            foreach (var woker in _workThreadContextList)
            {
                woker.Thread?.Join();
            }

            _workThreadContextList.Clear();
        }
    }
}
