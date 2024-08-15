namespace XCEngine.Server
{
    internal static class TimerThread
    {
        /// <summary>
        /// 是否停止
        /// </summary>
        private static bool _stop = false;

        /// <summary>
        /// 内部线程
        /// </summary>
        private static Thread _thread = null;

        /// <summary>
        /// 启动线程
        /// </summary>
        public static void Start()
        {
            if (_thread != null)
            {
                Log.Error($"TimerThread already started.");
                return;
            }

            _stop = false;
            _thread = new Thread(() => Run());
            _thread.Name = $"TimerThread";
            _thread.Start();
        }

        static void Run()
        {
            while (_stop == false)
            {
                Thread.Sleep(2);
                WorkThread.WakeUp();
            }
        }

        /// <summary>
        /// 停止线程
        /// </summary>
        public static void Stop()
        {
            _stop = true;
            _thread?.Join();
        }
    }
}
