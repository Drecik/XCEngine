namespace XCEngine.Core
{
    /// <summary>
    /// 默认网络包创建工厂
    /// </summary>
    public class DefaultNetPackageFactory : INetPackageFactory
    {
        public INetPackage CreateNetPackage()
        {
            return new DefaultNetPackage();
        }
    }
}
