namespace XCEngine.Server
{
    /// <summary>
    /// 工作线程上下文，存储当前线程运行时的情况
    /// </summary>
    internal static class WorkThreadContext
    {
        /// <summary>
        /// 当前工作线程Id
        /// </summary>
        public static ThreadLocal<int> WorkId = new();

        /// <summary>
        /// 当前运行的Actor Id
        /// </summary>
        private static ThreadLocal<int> ActorId = new();
    }
}
