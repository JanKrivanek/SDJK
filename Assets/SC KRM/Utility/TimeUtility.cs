using System.Globalization;
using System;
using SCKRM.Renderer;

namespace SCKRM
{
    public static class TimeUtility
    {
        public const double yearToDay = ((365 * 3) + 366) / 4d;
        public const double monthToDay = yearToDay / 12d;
        public const int weekPerDay = 7;

        public const long timeSpanTicksPerYear = (long)(TimeSpan.TicksPerDay * yearToDay);
        public const long timeSpanTicksPerMonth = (long)(TimeSpan.TicksPerDay * monthToDay);
        public const long timeSpanTicksPerWeek = TimeSpan.TicksPerDay * weekPerDay;

        public static int GetYears(this TimeSpan timeSpan) => (int)(timeSpan.Ticks / timeSpanTicksPerYear);
        public static double GetTotalYears(this TimeSpan timeSpan) => timeSpan.Ticks / timeSpanTicksPerYear;

        public static int GetMonths(this TimeSpan timeSpan) => (int)(timeSpan.Ticks / timeSpanTicksPerMonth);
        public static double GetTotalMonths(this TimeSpan timeSpan) => timeSpan.Ticks / timeSpanTicksPerMonth;

        public static int GetWeeks(this TimeSpan timeSpan) => (int)(timeSpan.Ticks / timeSpanTicksPerWeek);
        public static double GetTotalWeeks(this TimeSpan timeSpan) => timeSpan.Ticks / timeSpanTicksPerWeek;



        #region To Time
        /// <summary>
        /// (second = 70) = "1:10"
        /// </summary>
        /// <param name="timeSpan">시간</param>
        /// <param name="minuteAlwayShow">분 단위 항상 표시</param>
        /// <param name="hourAlwayShow">시간, 분 단위 항상 표시</param>
        /// <param name="dayAlwayShow">하루, 시간, 분 단위 항상 표시</param>
        /// <returns></returns>
        public static string ToTime(this int second, bool minuteAlwayShow = false, bool hourAlwayShow = false, bool dayAlwayShow = false) => (second < 0 ? "-" : "") + ToString(TimeSpan.FromSeconds(second), false, minuteAlwayShow, hourAlwayShow, dayAlwayShow);

        /// <summary>
        /// (second = 70.1f) = "1:10.1"
        /// </summary>
        /// <param name="second">초</param>
        /// <param name="decimalShow">소수 표시</param>
        /// <param name="minuteAlwayShow">분 단위 항상 표시</param>
        /// <param name="hourAlwayShow">시간, 분 단위 항상 표시</param>
        /// <param name="dayAlwayShow">하루, 시간, 분 단위 항상 표시</param>
        /// <returns></returns>
        [WikiIgnore]
        public static string ToTime(this float second, bool decimalShow = true, bool minuteAlwayShow = false, bool hourAlwayShow = false, bool dayAlwayShow = false)
        {
            if (!double.IsNormal(second))
                return "--:--";

            return (second < 0 ? "-" : "") + ToString(TimeSpan.FromSeconds(second), decimalShow, minuteAlwayShow, hourAlwayShow, dayAlwayShow);
        }

        /// <summary>
        /// (second = 70.1f) = "1:10.1"
        /// </summary>
        /// <param name="second">초</param>
        /// <param name="decimalShow">소수 표시</param>
        /// <param name="minuteAlwayShow">분 단위 항상 표시</param>
        /// <param name="hourAlwayShow">시간, 분 단위 항상 표시</param>
        /// <param name="dayAlwayShow">하루, 시간, 분 단위 항상 표시</param>
        /// <returns></returns>
        [WikiIgnore]
        public static string ToTime(this double second, bool decimalShow = true, bool minuteAlwayShow = false, bool hourAlwayShow = false, bool dayAlwayShow = false)
        {
            if (!double.IsNormal(second))
                return "--:--";

            return (second < 0 ? "-" : "") + ToString(TimeSpan.FromSeconds(second), decimalShow, minuteAlwayShow, hourAlwayShow, dayAlwayShow);
        }
        #endregion

        /// <summary>
        /// (second = 70.1) = "1:10.1"
        /// </summary>
        /// <param name="second">초</param>
        /// <param name="decimalShow">소수 표시</param>
        /// <param name="minuteAlwayShow">분 단위 항상 표시</param>
        /// <param name="hourAlwayShow">시간, 분 단위 항상 표시</param>
        /// <param name="dayAlwayShow">하루, 시간, 분 단위 항상 표시</param>
        /// <returns></returns>
        
        public static string ToString(this TimeSpan timeSpan, bool decimalShow = true, bool minuteAlwayShow = false, bool hourAlwayShow = false, bool dayAlwayShow = false)
        {
            try
            {
                double second = timeSpan.TotalSeconds;
                double secondAbs = second.Abs();
                if (decimalShow)
                {
                    if (second <= TimeSpan.MinValue.TotalSeconds || secondAbs <= double.NegativeInfinity)
                        return TimeSpan.MinValue.ToString(@"d\:hh\:mm\:ss\.ff");
                    else if (second >= TimeSpan.MaxValue.TotalSeconds || secondAbs >= double.PositiveInfinity)
                        return TimeSpan.MaxValue.ToString(@"d\:hh\:mm\:ss\.ff");
                    else if (secondAbs >= 86400 || dayAlwayShow)
                        return timeSpan.ToString(@"d\:hh\:mm\:ss\.ff");
                    else if (secondAbs >= 3600 || hourAlwayShow)
                        return timeSpan.ToString(@"h\:mm\:ss\.ff");
                    else if (secondAbs >= 60 || minuteAlwayShow)
                        return timeSpan.ToString(@"m\:ss\.ff");
                    else
                        return timeSpan.ToString(@"s\.ff");
                }
                else
                {
                    if (second <= TimeSpan.MinValue.TotalSeconds)
                        return TimeSpan.MinValue.ToString(@"d\:hh\:mm\:ss");
                    else if (second >= TimeSpan.MaxValue.TotalSeconds)
                        return TimeSpan.MaxValue.ToString(@"d\:h\:mm\:ss");
                    else if (secondAbs >= 86400 || dayAlwayShow)
                        return timeSpan.ToString(@"d\:hh\:mm\:ss");
                    else if (secondAbs >= 3600 || hourAlwayShow)
                        return timeSpan.ToString(@"h\:mm\:ss");
                    else if (secondAbs >= 60 || minuteAlwayShow)
                        return timeSpan.ToString(@"m\:ss");
                    else
                        return timeSpan.ToString(@"s");
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return "--:--";
            }
        }



        public static NameSpacePathReplacePair ToRelativeString(this TimeSpan timeSpan, int digits = 2)
        {
            try
            {
                NameSpacePathReplacePair nameSpacePathReplacePair = new NameSpacePathReplacePair();
                ReplaceOldNewPair replaceOldNewPair = new ReplaceOldNewPair("%value%", "");

                nameSpacePathReplacePair.nameSpace = "sc-krm";
                nameSpacePathReplacePair.path = "gui.";

                string isAgo = "later";
                if (timeSpan < TimeSpan.Zero)
                {
                    timeSpan = -timeSpan;
                    isAgo = "ago";
                }

                nameSpacePathReplacePair.path += isAgo + ".";

                switch (timeSpan.Ticks)
                {
                    case >= timeSpanTicksPerYear:
                        nameSpacePathReplacePair.path += "years";
                        replaceOldNewPair.replaceNew = timeSpan.GetTotalYears().Floor(digits).ToString("F" + digits);
                        break;
                    case >= timeSpanTicksPerMonth:
                        nameSpacePathReplacePair.path += "months";
                        replaceOldNewPair.replaceNew = timeSpan.GetTotalMonths().Floor(digits).ToString("F" + digits);
                        break;
                    case >= timeSpanTicksPerWeek:
                        nameSpacePathReplacePair.path += "weeks";
                        replaceOldNewPair.replaceNew = timeSpan.GetTotalWeeks().Floor(digits).ToString("F" + digits);
                        break;
                    case >= TimeSpan.TicksPerDay:
                        nameSpacePathReplacePair.path += "days";
                        replaceOldNewPair.replaceNew = timeSpan.TotalDays.Floor(digits).ToString("F" + digits);
                        break;
                    case >= TimeSpan.TicksPerHour:
                        nameSpacePathReplacePair.path += "hours";
                        replaceOldNewPair.replaceNew = timeSpan.TotalHours.Floor(digits).ToString("F" + digits);
                        break;
                    case >= TimeSpan.TicksPerMinute:
                        nameSpacePathReplacePair.path += "minutes";
                        replaceOldNewPair.replaceNew = timeSpan.TotalMinutes.Floor(digits).ToString("F" + digits);
                        break;
                    case >= TimeSpan.TicksPerSecond:
                        nameSpacePathReplacePair.path += "seconds";
                        replaceOldNewPair.replaceNew = timeSpan.TotalSeconds.Floor(digits).ToString("F" + digits);
                        break;
                    case >= TimeSpan.TicksPerMillisecond:
                        nameSpacePathReplacePair.path += "milliseconds";
                        replaceOldNewPair.replaceNew = timeSpan.TotalMilliseconds.Floor(digits).ToString("F" + digits);
                        break;
                    default:
                        return "";
                }

                nameSpacePathReplacePair.replace = new ReplaceOldNewPair[] { replaceOldNewPair };
                return nameSpacePathReplacePair;
            }
            catch (Exception)
            {
                return "";
            }
        }

        static readonly KoreanLunisolarCalendar klc = new KoreanLunisolarCalendar();
        public static UnlimitedDateTime ToLunarDate(this DateTime dateTime, out bool isLeapMonth)
        {
            int year = klc.GetYear(dateTime);
            int month = klc.GetMonth(dateTime);
            int day = klc.GetDayOfMonth(dateTime);
            int hour = klc.GetHour(dateTime);
            int minute = klc.GetMinute(dateTime);
            int second = klc.GetSecond(dateTime);
            int millisecond = (int)klc.GetMilliseconds(dateTime);

            isLeapMonth = false;

            //1년이 12이상이면 윤달이 있음..
            if (klc.GetMonthsInYear(year) > 12)
            {
                //년도의 윤달이 몇월인지?
                int leapMonth = klc.GetLeapMonth(year);

                //달이 윤월보다 같거나 크면 -1을 함 즉 윤8은->9 이기때문
                if (month >= leapMonth)
                {
                    isLeapMonth = true;
                    month--;
                }
            }

            return new UnlimitedDateTime(year, month, day, hour, minute, second, millisecond);
        }

        public static DateTime ToSolarDate(this DateTime dateTime, bool isLeapMonth = false)
        {
            if (klc.GetMonthsInYear(dateTime.Year) > 12)
            {
                int leapMonth = klc.GetLeapMonth(dateTime.Year);

                if (dateTime.Month > leapMonth - 1)
                    dateTime = klc.AddMonths(dateTime, 1);
                else if (dateTime.Month == leapMonth - 1 && isLeapMonth)
                    dateTime = klc.AddMonths(dateTime, 1);
            }

            return klc.ToDateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
        }
    }

    public struct UnlimitedDateTime
    {
        public UnlimitedDateTime(int year, int month, int day) : this()
        {
            this.year = year;
            this.month = month;
            this.day = day;
        }

        public UnlimitedDateTime(int year, int month, int day, int hour, int minute, int second) : this(year, month, day)
        {
            this.hour = hour;
            this.minute = minute;
            this.second = second;
        }

        public UnlimitedDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond) : this(year, month, day, hour, minute, second) => this.millisecond = millisecond;

        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }
        public int second { get; set; }
        public int millisecond { get; set; }

        public static explicit operator DateTime(UnlimitedDateTime dateTime) => new DateTime(dateTime.year, dateTime.month, dateTime.day, dateTime.hour, dateTime.minute, dateTime.second, dateTime.minute);
        public static implicit operator UnlimitedDateTime(DateTime dateTime) => new UnlimitedDateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Minute);

        public override string ToString() => $"{year}-{month}-{day} {hour}:{minute}:{second}";
    }
}
