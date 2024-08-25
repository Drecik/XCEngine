namespace XCEngine.Core
{
    /// <summary>
    /// 网络包序列化接口
    /// </summary>
    public interface INetPackageSerializer
    {
        /// <summary>
        /// 反序列化包
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        List<INetPackage> Deserialize(byte[] data, int length);

        /// <summary>
        /// 序列化包
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        byte[] Serialize(INetPackage package);
    }
}
