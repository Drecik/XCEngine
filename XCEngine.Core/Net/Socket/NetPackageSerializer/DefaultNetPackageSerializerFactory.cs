using System;

namespace XCEngine.Core
{
    /// <summary>
    /// 默认包序列化工厂
    /// </summary>
    public class DefaultNetPackageSerializerFactory : INetPackageSerializerFactory
    {
        /// <summary>
        /// 包体长度变量大小
        /// </summary>
        private int _bodyLengthValueSize;

        private INetPackageFactory _netPackageFactory;

        public DefaultNetPackageSerializerFactory(INetPackageFactory netPackageFactory, int bodyLengthValueSize = 4)
        {
            _netPackageFactory = netPackageFactory;
            _bodyLengthValueSize = bodyLengthValueSize;

            if (_bodyLengthValueSize != 2 && _bodyLengthValueSize != 4)
            {
                throw new Exception("Invalid bodyLengthValueSize, need 2 or 4");
            }
        }

        public INetPackageSerializer CreateNetPackageSerializer()
        {
            return new DefaultNetPackageSerializer(_netPackageFactory, _bodyLengthValueSize);
        }
    }
}
