using System.Net;

namespace XCEngine.Core
{
    /// <summary>
    /// 默认网络包实现
    /// </summary>
    public class DefaultNetPackage : INetPackage
    {
        /// <summary>
        /// 协议Id
        /// </summary>
        public int ProtoId;

        /// <summary>
        /// SessionId，用来处理回包的情况
        /// </summary>
        public uint SessionId;

        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// 序列化之后的大小
        /// </summary>
        public int SerializedSize { get => 4 + 4 + 4 + Data.Length; }

        /// <summary>
        /// 序列化数据
        /// </summary>
        /// <param name="writer"></param>
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(IPAddress.HostToNetworkOrder(ProtoId));
            writer.Write(IPAddress.HostToNetworkOrder((int)SessionId));
            writer.Write(IPAddress.HostToNetworkOrder(Data.Length));
            writer.Write(Data);
        }

        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="reader"></param>
        public void Deserialize(BinaryReader reader, int bodySize)
        {
            ProtoId = IPAddress.NetworkToHostOrder(reader.ReadInt32());
            SessionId = (uint)IPAddress.NetworkToHostOrder(reader.ReadInt32());
            int length = IPAddress.NetworkToHostOrder(reader.ReadInt32());
            Data = reader.ReadBytes(length);
        }
    }
}
