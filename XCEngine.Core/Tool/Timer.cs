namespace XCEngine.Core
{
    /// <summary>
    /// 定时器实现封装，线程不安全
    /// </summary>
    public class Timer
    {
        private IIdGenerator _idGenerator = new CommonIdGenerator();
        private Dictionary<int, TimerInfo> _timerDict = new();
        private PriorityQueue<TimerInfo, long> _timerQueue = new();
        private List<TimerInfo> _executeTargets = new List<TimerInfo>();

        private class TimerInfo
        {
            public int Id;
            public long NextTime;
            public int IntervalTime;
            public bool Canceled;
            public Action Callback;
        }

        /// <summary>
        /// 增加定时器任务
        /// </summary>
        /// <param name="startTime">开始执行时间</param>
        /// <param name="intervalTime">开始执行之后每隔多长时间重复执行一次，不重复执行可以传0</param>
        /// <param name="callback">任务回调函数</param>
        /// <returns>定时器任务Id</returns>
        public int AddTask(long startTime, int intervalTime, Action callback)
        {
            TimerInfo timerInfo = new TimerInfo();
            timerInfo.Id = _idGenerator.GenerateId();
            timerInfo.NextTime = startTime;
            timerInfo.IntervalTime = intervalTime;
            timerInfo.Callback = callback;

            _timerDict.Add(timerInfo.Id, timerInfo);
            _timerQueue.Enqueue(timerInfo, startTime);
            return timerInfo.Id;
        }

        /// <summary>
        /// 取消定时器任务
        /// </summary>
        /// <param name="timerId">定时器任务Id</param>
        public void CancelTask(int timerId)
        {
            if (_timerDict.TryGetValue(timerId, out var timerInfo))
            {
                timerInfo.Canceled = true;
            }
        }

        /// <summary>
        /// Update函数，由外部驱动
        /// </summary>
        public void Update()
        {
            long now = TimeUtils.NowMs();
            while (_timerQueue.Count > 0 && now >= _timerQueue.Peek().NextTime)
            {
                TimerInfo timerInfo = _timerQueue.Dequeue();
                if (timerInfo.Canceled == false)
                {
                    _executeTargets.Add(timerInfo);
                }
                else
                {
                    _timerDict.Remove(timerInfo.Id);
                    _idGenerator.ReturnId(timerInfo.Id);
                }
            }

            for (int i = 0; i < _executeTargets.Count; ++i)
            {
                TimerInfo timerInfo = _executeTargets[i];
                timerInfo.Callback();
                if (timerInfo.IntervalTime != 0)
                {
                    timerInfo.NextTime = timerInfo.NextTime + timerInfo.IntervalTime;
                    _timerQueue.Enqueue(timerInfo, timerInfo.NextTime);
                }
                else
                {
                    _timerDict.Remove(timerInfo.Id);
                    _idGenerator.ReturnId(timerInfo.Id);
                }
            }

            _executeTargets.Clear();
        }
    }
}
