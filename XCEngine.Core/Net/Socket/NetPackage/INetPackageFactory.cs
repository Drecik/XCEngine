namespace XCEngine.Core
{
    /// <summary>
    /// 网络包创建接口
    /// </summary>
    public interface INetPackageFactory
    {
        /// <summary>
        /// 创建网络包
        /// </summary>
        /// <returns></returns>
        INetPackage CreateNetPackage();
    }
}
