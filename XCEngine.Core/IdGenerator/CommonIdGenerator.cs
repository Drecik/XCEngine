namespace XCEngine.Core
{
    /// <summary>
    /// 通用Id生成器
    /// </summary>
    public class CommonIdGenerator : IIdGenerator
    {
        private static HashSet<int> _usedIds = new HashSet<int>();
        private static int _idGenerator = 0;

        public int GenerateId()
        {
            ++_idGenerator;
            if (_idGenerator == 0)
            {
                // 0预留
                ++_idGenerator;
            }

            while (_usedIds.Contains(_idGenerator))
            {
                ++_idGenerator;
            }

            _usedIds.Add(_idGenerator);
            return _idGenerator;
        }

        public void ReturnId(int id)
        {
            _usedIds.Remove(id);
        }

        public string GenerateUuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
