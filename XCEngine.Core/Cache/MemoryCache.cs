using System.Text;

namespace XCEngine.Core
{
    /// <summary>
    /// 内存缓存，不会落地，对象销毁掉数据也会消失
    /// </summary>
    public class MemoryCache
    {
        private object _lock = new object();
        private Dictionary<string, object> _cache = new Dictionary<string, object>();

        /// <summary>
        /// 设置缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key, object value)
        {
            lock (_lock)
            {
                _cache[key] = value;
            }
        }

        /// <summary>
        /// 删除缓存对象
        /// </summary>
        /// <param name="key"></param>
        public void DeleteValue(string key)
        {
            lock (_lock)
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            lock (_lock)
            {
                object ret;
                if (_cache.TryGetValue(key, out ret))
                {
                    return (T)ret;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// 获取缓存里面的所有数据（用来调试）
        /// </summary>
        /// <returns></returns>
        public new string ToString()
        {
            lock (_lock)
            {
                StringBuilder builder = new StringBuilder();
                foreach (KeyValuePair<string, object> iter in _cache)
                {
                    builder.Append(iter.Key);
                    builder.Append(':');
                    builder.Append(iter.Value.ToString());
                    builder.Append('\n');
                }
                return builder.ToString();
            }
        }
    }
}
