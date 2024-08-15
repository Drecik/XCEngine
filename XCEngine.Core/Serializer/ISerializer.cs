namespace XCEngine.Core
{
    /// <summary>
    /// 序列化接口
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        byte[] Serialize(object obj);

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        T Deserialize<T>(byte[] data);
        T Deserialize<T>(ReadOnlySpan<byte> data);

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        object Deserialize(Type type, byte[] data);
        object Deserialize(Type type, ReadOnlySpan<byte> data);
    }
}
