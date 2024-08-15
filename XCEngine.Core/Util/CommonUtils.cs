namespace XCEngine.Core
{
    /// <summary>
    /// 通用助手函数
    /// </summary>
    public class CommonUtils
    {
        /// <summary>
        /// 交换两个对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }

        /// <summary>
        /// 获取指定类名的Type
        /// </summary>
        /// <param name="typeFullName">类名全称（包含命名空间）</param>
        /// <returns></returns>
        private static Dictionary<string, Type> _typeDict = new Dictionary<string, Type>();
        public static Type GetType(string typeFullName)
        {
            if (_typeDict.TryGetValue(typeFullName, out var type))
            {
                return type;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeFullName);
                if (type != null)
                {
                    _typeDict[typeFullName] = type;
                    return type;
                }
            }
            return null;
        }

        /// <summary>
        /// 同C system函数，调用系统命令，会阻塞线程
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static int System(string fileName, string args)
        {
            Log.Info($"Do system fileName: {fileName}, args: {args}");
            var process = global::System.Diagnostics.Process.Start(fileName, args);
            process.WaitForExit();
            return process.ExitCode;
        }
    }
}
