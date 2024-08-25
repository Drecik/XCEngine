namespace XCEngine.Core
{
    /// <summary>
    /// 回车包序列化工厂
    /// </summary>
    public class LineNetPackageSerializerFactory : INetPackageSerializerFactory
    {
        private INetPackageFactory _netPackageFactory;

        public LineNetPackageSerializerFactory(INetPackageFactory netPackageFactory)
        {
            _netPackageFactory = netPackageFactory;
        }

        public INetPackageSerializer CreateNetPackageSerializer()
        {
            return new LineNetPackageSerializer(_netPackageFactory);
        }
    }
}
