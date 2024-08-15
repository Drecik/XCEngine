namespace XCEngine.Core
{
    /// <summary>
    /// MeoryPack序列化
    /// </summary>
    public class MemoryPackSerializer : ISerializer
    {
        public virtual byte[] Serialize(object obj)
        {
            MemoryPack.MemoryPackSerializer.Serialize(1);
            MemoryPack.MemoryPackSerializer.Serialize(new List<int>() { 1 });
            return MemoryPack.MemoryPackSerializer.Serialize(obj);
        }

        public virtual T Deserialize<T>(byte[] data)
        {
            return MemoryPack.MemoryPackSerializer.Deserialize<T>(data);
        }

        public virtual T Deserialize<T>(ReadOnlySpan<byte> data)
        {
            return MemoryPack.MemoryPackSerializer.Deserialize<T>(data);
        }

        public virtual object Deserialize(Type type, byte[] data)
        {
            return MemoryPack.MemoryPackSerializer.Deserialize(type, data);
        }

        public virtual object Deserialize(Type type, ReadOnlySpan<byte> data)
        {
            return MemoryPack.MemoryPackSerializer.Deserialize(type, data);
        }
    }
}
