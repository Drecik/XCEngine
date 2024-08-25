using System;

namespace XCEngine.Core
{
    /// <summary>
    /// 回车包序列化，每个回车一个包
    /// </summary>
    internal class LineNetPackageSerializer : INetPackageSerializer
    {
        private INetPackageFactory _netPackageFactory;

        private MemoryStream _receiveStream;
        private BinaryReader _receiveReader;

        public LineNetPackageSerializer(INetPackageFactory netPackageFactory)
        {
            _netPackageFactory = netPackageFactory;

            _receiveStream = new MemoryStream();
            _receiveReader = new BinaryReader(_receiveStream);
        }

        public List<INetPackage> Deserialize(byte[] data, int length)
        {
            List<INetPackage> result = new List<INetPackage>();
            int lastIndex = 0;
            while (true)
            {
                var index = Array.IndexOf(data, (byte)10, lastIndex, length);
                if (index != -1)
                {
                    _receiveStream.Write(data, lastIndex, index - lastIndex);
                    var package = _netPackageFactory.CreateNetPackage();
                    _receiveStream.Seek(0, SeekOrigin.Begin);
                    package.Deserialize(_receiveReader, (int)_receiveStream.Length);

                    result.Add(package);
                    lastIndex = index + 1;

                    _receiveStream.SetLength(0);
                    _receiveStream.Seek(0, SeekOrigin.Begin);
                }
                else
                {
                    _receiveStream.Write(data, lastIndex, length - lastIndex);
                    break;
                }
            }

            return result;
        }

        public byte[] Serialize(INetPackage package)
        {
            byte[] data = new byte[package.SerializedSize + 1];
            BinaryWriter writer = new BinaryWriter(new MemoryStream(data));
            package.Serialize(writer);
            writer.Write('\n');
            return data;
        }
    }
}
