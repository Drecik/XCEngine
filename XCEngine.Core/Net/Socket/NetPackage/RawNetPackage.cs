namespace XCEngine.Core
{
    /// <summary>
    /// 原始数据包，不做任何处理
    /// </summary>
    public class RawNetPackage : INetPackage
    {
        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data;

        public int SerializedSize => Data.Length;

        public void Deserialize(BinaryReader reader, int bodySize)
        {
            Data = new byte[bodySize];
            reader.Read(Data, 0, bodySize);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Data);
        }
    }
}
