using System.Collections.Concurrent;

namespace XCEngine.Core
{
    /// <summary>
    /// 对象池
    /// </summary>
    /// <typeparam name="T">对象池内部类型</typeparam>
    public class ObjectPool<T>
    {
        private ConcurrentBag<T> _objects;
        private Func<T> _objectGenerator;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="objectGenerator">对象创建函数</param>
        public ObjectPool(Func<T> objectGenerator)
        {
            _objects = new ConcurrentBag<T>();
            _objectGenerator = objectGenerator;
        }

        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            T item;
            if (_objects.TryTake(out item))
                return item;
            return _objectGenerator();
        }

        /// <summary>
        /// 返回对象到对象池
        /// </summary>
        /// <param name="item">返回的对象</param>
        public void Return(T item)
        {
            _objects.Add(item);
        }
    }
}
