using System.Diagnostics;
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
        private static ThreadLocal<StringBuilder> _strinbBuilder = new ThreadLocal<StringBuilder>(() => new StringBuilder());
        private static int _logOpenFlag = 0;

        static Log()
        {
            // 默认开启所有日志
            SetLogLevel(ELogLevel.Debug);
        }

        /// <summary>
        /// 开启日志
        /// </summary>
        /// <param name="logLevel">日志等级</param>
        public static void EnableLogLevel(ELogLevel logLevel)
        {
            _logOpenFlag = _logOpenFlag | (1 << (int)logLevel);
        }

        /// <summary>
        /// 关闭日志
        /// </summary>
        /// <param name="logLevel">日志等级</param>
        public static void DisableLogLevel(ELogLevel logLevel)
        {
            _logOpenFlag = _logOpenFlag | (1 << (int)logLevel);
        }

        /// <summary>
        /// 日志是否开启
        /// </summary>
        /// <param name="logLevel">日志等级</param>
        /// <returns></returns>
        public static bool IsLogEnabled(ELogLevel logLevel)
        {
            return (_logOpenFlag & (1 << (int)logLevel)) != 0;
        }

        /// <summary>
        /// 设置日志等级
        ///     小于该等级的日志会被开启
        /// </summary>
        /// <param name="logLevel">日志等级</param>
        public static void SetLogLevel(ELogLevel logLevel)
        {
            foreach (var level in Enum.GetValues(typeof(ELogLevel)))
            {
                if ((ELogLevel)level <= logLevel)
                {
                    EnableLogLevel((ELogLevel)level);
                }
                else
                {
                    DisableLogLevel((ELogLevel)level);
                }
            }
        }

        /// <summary>
        /// 日志打印实现设置
        /// </summary>
        public static Action<ELogLevel, string, int> LogWithLevel = (logLevel, text, skipFrames) =>
        {
            _strinbBuilder.Value.Clear();
            AppendLogPrefix(_strinbBuilder.Value, logLevel, skipFrames);
            _strinbBuilder.Value.Append(text);
            Console.WriteLine(_strinbBuilder.Value.ToString());
        };

        /// <summary>
        /// 输出Debug日志
        /// </summary>
        /// <param name="text"></param>
        public static void Debug(string text, int skipFrames = 0)
        {
            if (IsLogEnabled(ELogLevel.Debug) == false)
            {
                return;
            }
            LogWithLevel(ELogLevel.Debug, text, skipFrames);
        }

        /// <summary>
        /// 输出信息日志
        /// </summary>
        /// <param name="text"></param>
        public static void Info(string text, int skipFrames = 0)
        {
            if (IsLogEnabled(ELogLevel.Info) == false)
            {
                return;
            }
            LogWithLevel(ELogLevel.Info, text, skipFrames);
        }

        /// <summary>
        /// 输出警告日志
        /// </summary>
        /// <param name="text"></param>
        public static void Warning(string text, int skipFrames = 0)
        {
            if (IsLogEnabled(ELogLevel.Warning) == false)
            {
                return;
            }
            LogWithLevel(ELogLevel.Warning, text, skipFrames);
        }

        /// <summary>
        /// 输出异常日志
        /// </summary>
        /// <param name="ex"></param>
        public static void Exception(Exception ex, int skipFrames = 0)
        {
            Exception(string.Empty, ex, skipFrames);
        }

        /// <summary>
        /// 输出异常日志
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ex"></param>
        public static void Exception(string text, Exception ex, int skipFrames = 0)
        {
            if (IsLogEnabled(ELogLevel.Exception) == false)
            {
                return;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(text).Append(ex.ToString()).Append("\n").Append(ex.StackTrace);
            LogWithLevel(ELogLevel.Exception, sb.ToString(), skipFrames);
        }

        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="text"></param>
        public static void Error(string text, int skipFrames = 0)
        {
            if (IsLogEnabled(ELogLevel.Error) == false)
            {
                return;
            }
            LogWithLevel(ELogLevel.Error, text, skipFrames);
        }

        /// <summary>
        /// 获取日志通用前缀
        /// </summary>
        /// <param name="sb">日志字符串Builder</param>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        static void AppendLogPrefix(StringBuilder sb, ELogLevel logLevel, int skipFrames)
        {
            var stackTrace = new StackTrace(2 + skipFrames, true);
            var stackFrame = stackTrace.GetFrame(0);
            var fileName = Path.GetFileName(stackFrame.GetFileName());
            sb.Append("[").Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")).Append("][").Append(logLevel).Append("][").Append(fileName).Append(":").Append(stackFrame.GetFileLineNumber()).Append("]");
        }
    }
}
