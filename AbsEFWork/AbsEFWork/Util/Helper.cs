namespace AbsEFWork.Util
{
    public static class Helper
    {
        internal static double GetLongTimeStamp(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        internal static DateTime FromLongTimeStamp(string timeStamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(timeStamp)).DateTime.ToLocalTime();
        }
    }
}
