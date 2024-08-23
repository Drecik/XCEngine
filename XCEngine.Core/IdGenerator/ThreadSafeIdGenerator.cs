namespace XCEngine.Core
{
    /// <summary>
    /// 线程安全的Id生成器
    /// </summary>
    public class ThreadSafeIdGenerator : IIdGenerator
    {
        private object _lock = new object();
        private IIdGenerator _idGenerator = new CommonIdGenerator();

        public int GenerateId()
        {
            lock (_lock)
            {
                return _idGenerator.GenerateId();
            }
        }

        public void ReturnId(int id)
        {
            lock (_lock)
            {
                _idGenerator.ReturnId(id);
            }
        }

        public string GenerateUuid()
        {
            lock (_lock)
            {
                return _idGenerator.GenerateUuid();
            }
        }
    }
}
