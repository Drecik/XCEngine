using System.Runtime.InteropServices;

namespace XCEngine.Server
{
    public static class XC
    {
        /// <summary>
        /// 是否停止
        /// </summary>
        private static bool _stop = false;

        private static int _bootstrapActorId = 0;

        public static void Start(string[] args)
        {
            PosixSignalRegistration.Create(PosixSignal.SIGHUP, OnSignalStop);

            Log.LogWithLevel = LogImplement.LogWithLevel;

            // 读取配置
            if (ServerConfig.LoadConfig(args[0]) == false)
            {
                Log.Error("Load Server Config Failed!");
                Environment.Exit(1);
            }

            if (false == Actor.Initialize())
            {
                Log.Error("Actor intialize failed");
                Environment.Exit(1);
            }

            // 启动定时器线程
            TimerThread.Start();

            // 启动工作线程
            WorkThread.Start(ServerConfig.WorkThreadCount);

            // 启动成功，创建启动Actor
            var bootstrapType = CommonUtils.GetType(ServerConfig.GetConfig("BootstrapType", string.Empty));
            if (bootstrapType == null)
            {
                Log.Error("Invalid bootstrap type");
                Environment.Exit(1);
            }
            _bootstrapActorId = Actor.Start(bootstrapType);

            LoopUntilStop();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public static void Stop()
        {
            _stop = true;
        }

        static void OnSignalStop(PosixSignalContext context)
        {
            Log.Info("OnSignalStop");
            context.Cancel = true;
            Stop();
        }

        static void LoopUntilStop()
        {
            while (!_stop)
            {
                Thread.Sleep(1000);
            }

            BeginStop();
        }

        static void BeginStop()
        {
            Log.Info("========= Begin stop ===========");

            Log.Info("========= Stop bootstrap actor =========");
            Actor.Send(_bootstrapActorId, "BeginStop");

            do
            {
                Log.Info("Waiting bootstrap actor stop...");
                Thread.Sleep(5000);
            }
            while (Actor.GetActorContext(_bootstrapActorId) != null);


            while (Actor.GetActorContextCount() > 0)
            {
                Log.Info($"Waiting all actor etop... now: {Actor.GetActorContextCount()}");
                Thread.Sleep(5000);
            }

            Log.Info("======== Stop timer thread ========");
            TimerThread.Stop();

            Log.Info("======== Stop work thread ========");
            WorkThread.Stop();

            Log.Info("========= Stop =========");
        }

        #region Timer

        private static Core.Timer _timer = new Core.Timer();

        internal static void UpdateTimer()
        {
            lock (_timer)
            {
                _timer.Update();
            }
        }

        /// <summary>
        /// 延迟执行
        /// </summary>
        /// <param name="delay">延迟毫秒数</param>
        /// <param name="messageId">执行的消息id</param>
        /// <param name="userData">用户数据</param>
        public static void Delay(long delay, string messageId, object userData = null)
        {
            lock (_timer)
            {
                var actorId = Actor.ActorId.Value;
                _timer.AddTask(TimeUtils.NowMs() + delay, 0, () =>
                {
                    Actor.PushMessage(actorId, new ActorMessage()
                    {
                        From = 0,
                        To = actorId,
                        MessageType = ActorMessage.EMessageType.Timer,
                        MessageId = messageId,
                        MessageData = userData
                    });
                });
            }
        }

        /// <summary>
        /// 重复执行
        /// </summary>
        /// <param name="firstDelay">第一次执行延时的毫秒数</param>
        /// <param name="interval">每次执行间隔</param>
        /// <param name="messageId">执行的</param>
        /// <param name="userData">用户数据</param>
        public static void Tick(long firstDelay, int interval, string messageId, object userData = null)
        {
            lock (_timer)
            {
                var actorId = Actor.ActorId.Value;
                _timer.AddTask(TimeUtils.NowMs() + firstDelay, interval, () =>
                {
                    Actor.PushMessage(actorId, new ActorMessage()
                    {
                        From = 0,
                        To = actorId,
                        MessageType = ActorMessage.EMessageType.Timer,
                        MessageId = messageId,
                        MessageData = userData
                    });
                });
            }
        }

        /// <summary>
        /// 取消定时器
        /// </summary>
        /// <param name="timerId"></param>
        public static void Cancel(int timerId)
        {
            lock (_timer)
            {
                _timer.CancelTask(timerId);
            }
        }

        #endregion
    }
}