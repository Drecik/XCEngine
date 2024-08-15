namespace XCEngine.Core
{
    /// <summary>
    /// Id生成者接口
    /// </summary>
    public interface IIdGenerator
    {
        /// <summary>
        /// 生成Id（除0以外的任何整数都可能返回）
        /// 同一个IdGenerator对象生成的Id唯一
        /// </summary>
        /// <returns></returns>
        int GenerateId();

        /// <summary>
        /// 返还这个Id，Id用完之后返还这个Id，可以再次被生成使用
        /// </summary>
        /// <param name="id"></param>
        void ReturnId(int id);

        /// <summary>
        /// 生成全局唯一Id
        /// </summary>
        /// <returns></returns>
        string GenerateUuid();
    }
}
