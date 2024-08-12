using System.Text;

namespace XCEngine.Core
{
    /// <summary>
    /// 日志等级
    /// </summary>
    public enum ELogLevel
    {
        /// <summary>
        /// 错误
        /// </summary>
        Error,

        /// <summary>
        /// 异常
        /// </summary>
        Exception,

        /// <summary>
        /// 警告
        /// </summary>
        Warning,

        /// <summary>
        /// 信息
        /// </summary>
        Info,

        /// <summary>
        /// 调试
        /// </summary>
        Debug
    }

    /// <summary>
    /// 日志输出类
    /// </summary>
    public static class Log
    {
        private static ThreadLocal<StringBuilder> _strinbBuilder = new(() => new StringBuilder());

        /// <summary>
        /// 日志打印实现设置
        /// </summary>
        public static Action<ELogLevel, string> LogWithLevel = (logLevel, text) =>
        {
            _strinbBuilder.Value.Clear();
            AppendLogPrefix(_strinbBuilder.Value, logLevel);
            _strinbBuilder.Value.Append(text);
            Console.WriteLine(text);
        };

        /// <summary>
        /// 输出Debug日志
        /// </summary>
        /// <param name="text"></param>
        public static void Debug(string text)
        {
            LogWithLevel(ELogLevel.Debug, text);
        }

        /// <summary>
        /// 输出信息日志
        /// </summary>
        /// <param name="text"></param>
        public static void Info(string text)
        {
            LogWithLevel(ELogLevel.Info, text);
        }

        /// <summary>
        /// 输出警告日志
        /// </summary>
        /// <param name="text"></param>
        public static void Warning(string text)
        {
            LogWithLevel(ELogLevel.Warning, text);
        }

        /// <summary>
        /// 输出异常日志
        /// </summary>
        /// <param name="ex"></param>
        public static void Exception(Exception ex)
        {
            Exception("", ex);
        }

        /// <summary>
        /// 输出异常日志
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ex"></param>
        public static void Exception(string text, Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(text).Append(ex.ToString()).Append("\n").Append(ex.StackTrace);
            LogWithLevel(ELogLevel.Exception, sb.ToString());
        }

        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="text"></param>
        public static void Error(string text)
        {
            LogWithLevel(ELogLevel.Error, text);
        }

        /// <summary>
        /// Append日志通用前缀
        /// </summary>
        /// <param name="sb">日志字符串Builder</param>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        static void AppendLogPrefix(StringBuilder sb, ELogLevel logLevel)
        {
            sb.Append("[").Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")).Append("][").Append(logLevel).Append("] ");
        }
    }
}
