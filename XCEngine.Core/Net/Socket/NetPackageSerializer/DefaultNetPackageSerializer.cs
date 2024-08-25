using System.Net;

namespace XCEngine.Core
{
    /// <summary>
    /// 默认网络包序列化，Body长度（手动指定长度大小） + Body数据
    /// </summary>
    public class DefaultNetPackageSerializer : INetPackageSerializer
    {
        private INetPackageFactory _netPackageFactory;

        private MemoryStream _receiveStream;
        private BinaryReader _receiveReader;

        /// <summary>
        /// 包体长度变量大小
        /// </summary>
        private int _bodyLengthValueSize;

        public DefaultNetPackageSerializer(INetPackageFactory netPackageFactory, int bodyLengthValueSize)
        {
            _netPackageFactory = netPackageFactory;

            _receiveStream = new MemoryStream();
            _receiveReader = new BinaryReader(_receiveStream);
            _bodyLengthValueSize = bodyLengthValueSize;
        }

        public List<INetPackage> Deserialize(byte[] data, int length)
        {
            _receiveStream.Write(data, 0, length);
            _receiveStream.Seek(0, SeekOrigin.Begin);

            List<INetPackage> result = new List<INetPackage>();
            while (true)
            {
                //Log.Info($"-------------Position: {_receiveStream.Position}, Length: {_receiveStream.Length}");
                // 小于包体大小的长度
                if (_receiveStream.Length - _receiveStream.Position < 4)
                {
                    //Log.Info($"Break");
                    break;
                }

                int bodyLength = ReadBodyLength(_receiveReader);
                //Log.Info($"Body lenght: {bodyLength}");

                // 小于包体大小
                if (_receiveStream.Length - _receiveStream.Position < bodyLength)
                {
                    //Log.Info($"-------------Break2: Position: {_receiveStream.Position}, Length: {_receiveStream.Length}");
                    // 恢复到读之前的位置
                    _receiveStream.Seek(-_bodyLengthValueSize, SeekOrigin.Current);
                    break;
                }

                INetPackage package = _netPackageFactory.CreateNetPackage();
                package.Deserialize(_receiveReader, bodyLength);
                //Log.Info($"------------------- {(package as DefaultBabuNetPackage).ProtoId}, Cost Length: {package.SerializedSize}");
                result.Add(package);
            }

            // 把剩余的内容提前
            if (_receiveStream.Position != _receiveStream.Length)
            {
                //Log.Info($"-----1: Position: {_receiveStream.Position}, Length: {_receiveStream.Length}");
                byte[] leftData = _receiveReader.ReadBytes((int)(_receiveStream.Length - _receiveStream.Position));
                _receiveStream.SetLength(0);
                _receiveStream.Seek(0, SeekOrigin.Begin);
                _receiveStream.Write(leftData, 0, leftData.Length);
            }
            else
            {
                //Log.Info($"-----2: Position: {_receiveStream.Position}, Length: {_receiveStream.Length}");
                _receiveStream.SetLength(0);
                _receiveStream.Seek(0, SeekOrigin.Begin);
            }
            return result;
        }

        public byte[] Serialize(INetPackage package)
        {
            byte[] data = new byte[_bodyLengthValueSize + package.SerializedSize];
            BinaryWriter writer = new BinaryWriter(new MemoryStream(data));

            if (_bodyLengthValueSize == 2)
            {
                writer.Write(IPAddress.HostToNetworkOrder((short)package.SerializedSize));
            }
            else
            {
                writer.Write(IPAddress.HostToNetworkOrder(package.SerializedSize));
            }
            package.Serialize(writer);
            return data;
        }

        int ReadBodyLength(BinaryReader reader)
        {
            if (_bodyLengthValueSize == 2)
            {
                return IPAddress.NetworkToHostOrder(reader.ReadInt16());
            }
            else
            {
                return IPAddress.NetworkToHostOrder(reader.ReadInt32());
            }
        }
    }
}
