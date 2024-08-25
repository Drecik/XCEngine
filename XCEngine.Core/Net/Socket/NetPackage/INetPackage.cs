namespace XCEngine.Core
{
    /// <summary>
    /// 网络包接口
    /// </summary>
    public interface INetPackage
    {
        /// <summary>
        /// 序列化后的长度
        /// </summary>
        int SerializedSize { get; }

        /// <summary>
        /// 序列化数据
        /// </summary>
        /// <param name="writer"></param>
        void Serialize(BinaryWriter writer);

        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="bodySize"></param>
        void Deserialize(BinaryReader reader, int bodySize);
    }
}
