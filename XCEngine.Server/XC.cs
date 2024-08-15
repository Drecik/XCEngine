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
            _bootstrapActorId = Actor.Start(bootstrapType, null);

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
    }
}