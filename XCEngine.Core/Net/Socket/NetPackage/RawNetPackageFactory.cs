namespace XCEngine.Core
{
    /// <summary>
    /// 原始网络包创建工厂
    /// </summary>
    public class RawNetPackageFactory : INetPackageFactory
    {
        public INetPackage CreateNetPackage()
        {
            return new RawNetPackage();
        }
    }
}
