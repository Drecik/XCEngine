namespace XCEngine.Core
{
    /// <summary>
    /// 线程安全的懒加载模式单例模板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : class, new()
    {
        private static T _instance;
        static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new T();
                    }
                }
                return _instance;
            }
        }

        public static void Release()
        {
            _instance = null;
        }
    }
}
