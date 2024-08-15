using System.Text.Json.Nodes;

namespace XCEngine.Server
{
    /// <summary>
    /// 服务器配置
    /// </summary>
    public static class ServerConfig
    {
        /// <summary>
        /// 内部配置缓存
        /// </summary>
        private static MemoryCache _cache = new MemoryCache();

        /// <summary>
        /// 工作线程数量
        /// </summary>
        public static int WorkThreadCount => _cache.GetValue<int>("WorkThreadCount", 1);

        /// <summary>
        /// 是否开发版本
        /// </summary>
        public static bool IsDevelopment => _cache.GetValue<bool>("Development");

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="configPath">配置路径</param>
        /// <returns></returns>
        public static bool LoadConfig(string configPath)
        {
            try
            {
                string fileContent = File.ReadAllText(configPath);
                JsonObject configJson = JsonNode.Parse(fileContent).AsObject();
                SetConfigs(configJson);
                return true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return false;
            }
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="configName">配置名字</param>
        /// <param name="defaultValue">配置不存在的时候的默认值</param>
        /// <returns></returns>
        public static T GetConfig<T>(string configName, T defaultValue = default(T))
        {
            lock (_cache)
            {
                return _cache.GetValue<T>(configName, defaultValue);
            }
        }

        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="configName">配置名字</param>
        /// <param name="config">配置值</param>
        public static void SetConfig(string configName, object config)
        {
            lock (_cache)
            {
                _cache.SetValue(configName, config);
            }
        }

        /// <summary>
        /// 通过json设置一批配置
        /// </summary>
        /// <param name="configJson"></param>
        public static void SetConfigs(JsonObject configJson)
        {
            foreach (var iter in configJson)
            {
                if (iter.Value.AsValue().TryGetValue<bool>(out var b))
                {
                    SetConfig(iter.Key, b);
                }
                else if (iter.Value.AsValue().TryGetValue<int>(out var i))
                {
                    SetConfig(iter.Key, i);
                }
                else if (iter.Value.AsValue().TryGetValue<double>(out var d))
                {
                    SetConfig(iter.Key, d);
                }
                else if (iter.Value.AsValue().TryGetValue<string>(out var s))
                {
                    SetConfig(iter.Key, s);
                }
                else
                {
                    // 直接设置JsonNode
                    SetConfig(iter.Key, iter.Value);
                }
            }
        }
    }
}
