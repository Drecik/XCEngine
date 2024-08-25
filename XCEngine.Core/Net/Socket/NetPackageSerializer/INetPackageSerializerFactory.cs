namespace XCEngine.Core
{
    /// <summary>
    /// 网络包序列化创建接口
    /// </summary>
    public interface INetPackageSerializerFactory
    {
        /// <summary>
        /// 创建网络包序列化
        /// </summary>
        /// <param name="netPackageFactory">网络包创建工厂</param>
        /// <returns></returns>
        INetPackageSerializer CreateNetPackageSerializer();
    }
}
