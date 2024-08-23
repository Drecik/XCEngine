namespace XCEngine.Core
{
    /// <summary>
    /// 通用Id生成器
    /// </summary>
    public class CommonIdGenerator : IIdGenerator
    {
        private HashSet<int> _usedIds = new HashSet<int>();
        private int _idGenerator = 0;

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

            if (_usedIds.Count > 100000)
            {
                Log.Warning($"Id used overlay, now: {_usedIds.Count}");
            }

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
