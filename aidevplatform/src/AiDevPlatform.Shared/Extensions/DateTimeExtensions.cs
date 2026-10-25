namespace AiDevPlatform.Shared.Extensions;

public static class DateTimeExtensions
{
    public static DateTime StartOfDay(this DateTime dateTime)
    {
        return dateTime.Date;
    }

    public static DateTime EndOfDay(this DateTime dateTime)
    {
        return dateTime.Date.AddDays(1).AddTicks(-1);
    }

    public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        var diff = (7 + (dateTime.DayOfWeek - startOfWeek)) % 7;
        return dateTime.AddDays(-1 * diff).Date;
    }

    public static DateTime EndOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        return dateTime.StartOfWeek(startOfWeek).AddDays(6).EndOfDay();
    }

    public static DateTime StartOfMonth(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, 1);
    }

    public static DateTime EndOfMonth(this DateTime dateTime)
    {
        return dateTime.StartOfMonth().AddMonths(1).AddDays(-1).EndOfDay();
    }

    public static DateTime StartOfYear(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, 1, 1);
    }

    public static DateTime EndOfYear(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, 12, 31).EndOfDay();
    }

    public static DateTime StartOfQuarter(this DateTime dateTime)
    {
        var quarter = (dateTime.Month - 1) / 3 + 1;
        var startMonth = (quarter - 1) * 3 + 1;
        return new DateTime(dateTime.Year, startMonth, 1);
    }

    public static DateTime EndOfQuarter(this DateTime dateTime)
    {
        return dateTime.StartOfQuarter().AddMonths(3).AddDays(-1).EndOfDay();
    }

    public static int GetQuarter(this DateTime dateTime)
    {
        return (dateTime.Month - 1) / 3 + 1;
    }

    public static int GetWeekOfYear(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        var startOfYear = new DateTime(dateTime.Year, 1, 1);
        var startOfFirstWeek = startOfYear.StartOfWeek(startOfWeek);

        if (startOfFirstWeek > dateTime)
        {
            startOfYear = startOfYear.AddYears(-1);
            startOfFirstWeek = startOfYear.StartOfWeek(startOfWeek);
        }

        return (int)((dateTime - startOfFirstWeek).TotalDays / 7) + 1;
    }

    public static bool IsWeekend(this DateTime dateTime)
    {
        return dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;
    }

    public static bool IsWeekday(this DateTime dateTime)
    {
        return !dateTime.IsWeekend();
    }

    public static DateTime AddBusinessDays(this DateTime dateTime, int businessDays)
    {
        var direction = businessDays >= 0 ? 1 : -1;
        var absBusinessDays = Math.Abs(businessDays);
        var result = dateTime;

        while (absBusinessDays > 0)
        {
            result = result.AddDays(direction);
            if (result.IsWeekday())
                absBusinessDays--;
        }

        return result;
    }

    public static int GetBusinessDaysBetween(this DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            return -GetBusinessDaysBetween(endDate, startDate);

        var businessDays = 0;
        var currentDate = startDate.Date;

        while (currentDate <= endDate.Date)
        {
            if (currentDate.IsWeekday())
                businessDays++;
            currentDate = currentDate.AddDays(1);
        }

        return businessDays;
    }

    public static string ToRelativeTime(this DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalSeconds < 60)
            return "刚刚";

        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes}分钟前";

        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours}小时前";

        if (timeSpan.TotalDays < 7)
            return $"{(int)timeSpan.TotalDays}天前";

        if (timeSpan.TotalDays < 30)
            return $"{(int)(timeSpan.TotalDays / 7)}周前";

        if (timeSpan.TotalDays < 365)
            return $"{(int)(timeSpan.TotalDays / 30)}个月前";

        return $"{(int)(timeSpan.TotalDays / 365)}年前";
    }

    public static string ToChineseDateString(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy年MM月dd日");
    }

    public static string ToChineseDateTimeString(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy年MM月dd日 HH:mm:ss");
    }

    public static string ToChineseTimeString(this DateTime dateTime)
    {
        return dateTime.ToString("HH:mm:ss");
    }

    public static DateTime ToChinaTime(this DateTime dateTime)
    {
        var chinaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, chinaTimeZone);
    }

    public static DateTime ToUtcTime(this DateTime dateTime)
    {
        var chinaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
        return TimeZoneInfo.ConvertTimeToUtc(dateTime, chinaTimeZone);
    }

    public static bool IsSameDay(this DateTime dateTime1, DateTime dateTime2)
    {
        return dateTime1.Date == dateTime2.Date;
    }

    public static bool IsSameMonth(this DateTime dateTime1, DateTime dateTime2)
    {
        return dateTime1.Year == dateTime2.Year && dateTime1.Month == dateTime2.Month;
    }

    public static bool IsSameYear(this DateTime dateTime1, DateTime dateTime2)
    {
        return dateTime1.Year == dateTime2.Year;
    }

    public static int GetAge(this DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age))
            age--;

        return age;
    }

    public static bool IsLeapYear(this DateTime dateTime)
    {
        return DateTime.IsLeapYear(dateTime.Year);
    }

    public static int GetDaysInMonth(this DateTime dateTime)
    {
        return DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
    }

    public static int GetDaysInYear(this DateTime dateTime)
    {
        return dateTime.IsLeapYear() ? 366 : 365;
    }
}
