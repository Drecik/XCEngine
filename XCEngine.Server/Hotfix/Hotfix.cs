using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text.Json.Nodes;

namespace XCEngine.Server
{
    /// <summary>
    /// 热更支持
    /// </summary>
    public static class Hotfix
    {
        /// <summary>
        /// 是否正在热重载
        /// </summary>
        public static bool Reloading = false;

        /// <summary>
        /// 热更Assembly加载器
        /// </summary>
        private class HotfixAssemblyLoadContext : AssemblyLoadContext
        {
            public HotfixAssemblyLoadContext() : base(true)
            {
            }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                // 处理热更Dll的依赖
                var assembly = _modelDllList.FirstOrDefault(e => e.FullName == assemblyName.FullName);
                if (assembly != null)
                {
                    return assembly;
                }
                return null;
            }
        }

        private static List<Assembly> _modelDllList = new List<Assembly>();
        private static HotfixAssemblyLoadContext _modelHotfixDllLoader;

        public static IEnumerable<Assembly> ModelDllList => _modelDllList;
        public static IEnumerable<Assembly> ModelHotfixDllList => _modelHotfixDllLoader.Assemblies;

        /// <summary>
        /// 初始化热更
        /// </summary>
        public static void Initialize()
        {
            // 热更信号绑定
            PosixSignalRegistration.Create(PosixSignal.SIGHUP, OnSignalHup);

            LoadModelDlls();
            LoadModelHotfixDlls();
        }

        /// <summary>
        /// 加载所有Model Dll（不支持热更）
        /// </summary>
        static void LoadModelDlls()
        {
            var dllPathArr = ServerConfig.GetConfig("ModelDllPathList", new JsonArray()).AsArray();
            var pdbPathArr = ServerConfig.GetConfig("ModelPdbPathList", new JsonArray()).AsArray();
            for (int i = 0; i < dllPathArr.Count; i++)
            {
                var dllPath = dllPathArr[i].ToString();
                var pdbPath = pdbPathArr[i].ToString();
                var assemblyBytes = File.ReadAllBytes(dllPath);
                var symbolAssemblyBytes = File.ReadAllBytes(pdbPath);

                using var ms = new MemoryStream(assemblyBytes);
                using var symbolMs = new MemoryStream(symbolAssemblyBytes);
                var assembly = AssemblyLoadContext.Default.LoadFromStream(ms, symbolMs);
                _modelDllList.Add(assembly);
            }
        }

        /// <summary>
        /// 加载所有Model Hotfix Dll（支持热更）
        /// </summary>
        static void LoadModelHotfixDlls()
        {
            _modelHotfixDllLoader?.Unload();
            _modelHotfixDllLoader = new HotfixAssemblyLoadContext();

            var dllPathArr = ServerConfig.GetConfig("ModelHotfixDllPathList", new JsonArray()).AsArray();
            var pdbPathArr = ServerConfig.GetConfig("ModelHotfixPdbPathList", new JsonArray()).AsArray();
            for (int i = 0; i < dllPathArr.Count; i++)
            {
                var dllPath = dllPathArr[i].ToString();
                var pdbPath = pdbPathArr[i].ToString();
                var assemblyBytes = File.ReadAllBytes(dllPath);
                var symbolAssemblyBytes = File.ReadAllBytes(pdbPath);

                using var ms = new MemoryStream(assemblyBytes);
                using var symbolMs = new MemoryStream(symbolAssemblyBytes);
                _modelHotfixDllLoader.LoadFromStream(ms, symbolMs);
            }
        }

        /// <summary>
        /// 热更
        /// </summary>
        public static void Reload()
        {
            Log.Info("Do Reload Begin....");

            Reloading = true;

            Thread.Sleep(1000); // 等待正在执行的actor执行完

            LoadModelHotfixDlls();

            Actor.ReInitialize();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Reloading = false;

            Log.Info("Do Reload End....");
        }

        public static void OnSignalHup(PosixSignalContext context)
        {
            Log.Info("Received SIGHUP...");
            Reload();
        }
    }
}
