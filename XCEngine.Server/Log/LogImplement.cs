using System.Diagnostics;
using System.Text;

namespace XCEngine.Server
{
    internal static class LogImplement
    {
        private static ThreadLocal<StringBuilder> _strinbBuilder = new ThreadLocal<StringBuilder>(() => new StringBuilder());

        public static void LogWithLevel(ELogLevel logLevel, string text, int skipFrames)
        {
            _strinbBuilder.Value.Clear();
            AppendLogPrefix(_strinbBuilder.Value, logLevel, skipFrames);
            _strinbBuilder.Value.Append(text);
            if (logLevel == ELogLevel.Error || logLevel == ELogLevel.Warning)
            {
                _strinbBuilder.Value.Append("\n").Append(new StackTrace().ToString());
            }
            Console.WriteLine(_strinbBuilder.Value.ToString());
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
            sb.Append("[").Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")).Append("]")
                .Append("[").Append(logLevel).Append("]")
                .Append("[").Append(Actor.Self()).Append("]")
                .Append("[").Append(fileName).Append(":").Append(stackFrame.GetFileLineNumber()).Append("] ");
        }
    }
}
