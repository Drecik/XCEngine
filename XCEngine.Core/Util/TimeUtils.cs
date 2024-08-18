namespace XCEngine.Core
{
    /// <summary>
    /// 时间助手函数
    /// </summary>
    public class TimeUtils
    {
        /// <summary>
        /// 分钟秒数
        /// </summary>
        public const long MinuteSeconds = 60;

        /// <summary>
        /// 小时秒数
        /// </summary>
        public const long HourSeconds = 60 * MinuteSeconds;

        /// <summary>
        /// 天秒数
        /// </summary>
        public const long DaySeconds = 24 * HourSeconds;

        /// <summary>
        /// 周秒数
        /// </summary>
        public const long WeekSeconds = 7 * DaySeconds;

        /// <summary>
        /// 月秒数
        /// </summary>
        public const long MonthSeconds = 30 * DaySeconds;

        private static DateTime _utcStartTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);

        /// <summary>
        /// 秒级时间戳
        /// </summary>
        /// <returns></returns>
        public static long Now()
        {
            return NowMs() / 1000;
        }

        /// <summary>
        /// 毫秒级别时间戳
        /// </summary>
        /// <returns></returns>
        public static long NowMs()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 当前时间
        /// </summary>
        /// <returns></returns>
        public static DateTime NowDateTime()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// DateTime转秒级时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToUnixStamp(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - _utcStartTime).TotalSeconds;
        }

        /// <summary>
        /// DateTime转毫秒级别时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToUnixStampMs(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - _utcStartTime).TotalMilliseconds;
        }

        /// <summary>
        /// 秒级时间戳转DateTime
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(long ts)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(ts);
            return dateTimeOffset.DateTime;
        }

        /// <summary>
        /// 毫秒级时间戳转DateTime
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static DateTime ToDateTimeMs(long ts)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(ts);
            return dateTimeOffset.DateTime;
        }

        /// <summary>
        /// 秒级时间戳转Utc DateTime
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static DateTime ToUtcDateTime(long ts)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(ts);
            return dateTimeOffset.UtcDateTime;
        }

        /// <summary>
        /// 毫秒级时间戳转Utc DateTime
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static DateTime ToUtcDateTimeMs(long ts)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(ts);
            return dateTimeOffset.UtcDateTime;
        }

        /// <summary>
        /// 今天0点时间
        /// </summary>
        /// <returns></returns>
        public static long TimeOfToday()
        {
            return ToUnixStamp(DateTime.Now.Date);
        }

        /// <summary>
        /// 明天0点时间
        /// </summary>
        /// <returns></returns>
        public static long TimeOfTomorrow()
        {
            return TimeOfToday() + DaySeconds;
        }

        /// <summary>
        /// 本周开始(周一)0点时间
        /// </summary>
        /// <returns></returns>
        public static long TimeOfWeek()
        {
            var dt = DateTime.Now;
            int diff = dt.DayOfWeek - DayOfWeek.Sunday;
            return ToUnixStamp(dt.Date.AddDays(-diff)) + DaySeconds;
        }

        /// <summary>
        /// 下周开始（周一）0点时间
        /// </summary>
        /// <returns></returns>
        public static long TimeOfNextWeek()
        {
            return TimeOfWeek() + WeekSeconds;
        }

        /// <summary>
        /// 本月开始（1号）0点时间
        /// </summary>
        /// <returns></returns>
        public static long TimeOfMonth()
        {
            var dt = DateTime.Now;
            int diff = dt.Day - 1;
            return ToUnixStamp(dt.Date.AddDays(-diff));
        }

        /// <summary>
        /// 下月开始（1号）0点时间
        /// </summary>
        /// <returns></returns>
        public static long TimeOfNextMonth()
        {
            var dt = DateTime.Now;
            int count = GetMonthDayCount(dt.Month);

            return ToUnixStamp(dt.Date.AddDays(count - dt.Day + 1));
        }

        /// <summary>
        /// 获取月份天数
        /// </summary>
        /// <param name="month">月份</param>
        /// <param name="year">年份</param>
        /// <returns></returns>
        private static int[] _monthDayCountArr = { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        public static int GetMonthDayCount(int month = 0, int year = 0)
        {
            if (month == 0)
            {
                month = ToDateTimeMs(NowMs()).Month;
                year = ToDateTimeMs(NowMs()).Year;
            }
            else if (year == 0)
            {
                year = ToDateTimeMs(NowMs()).Year;
            }

            if (month == 2 && IsLeapYear(year))
            {
                return 29;
            }
            return _monthDayCountArr[month];
        }

        /// <summary>
        /// 是否是闰年
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static bool IsLeapYear(int year = 0)
        {
            if (year == 0)
            {
                year = ToDateTimeMs(NowMs()).Year;
            }

            return year % 400 == 0 || (year % 4 == 0 && year % 100 != 0);
        }
    }
}
