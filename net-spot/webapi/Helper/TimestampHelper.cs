namespace webapi.Helper
{
    public static class TimestampHelper
    {
        private static readonly long _minTimestamp;

        private static readonly long _maxTimestamp;

        //
        // 摘要:
        //     转为Unix时间戳
        //
        // 参数:
        //   dateTime:
        public static long ToUnixTimeMilliseconds(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Local);
            }

            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        }

        //
        // 摘要:
        //     Unix时间戳转为DateTimeOffset
        //
        // 参数:
        //   timestamp:
        public static DateTimeOffset FromUnixTimeMilliseconds(long timestamp)
        {
            if (timestamp < _minTimestamp)
            {
                timestamp = _minTimestamp;
            }
            else if (timestamp > _maxTimestamp)
            {
                timestamp = _maxTimestamp;
            }

            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
        }

        static TimestampHelper()
        {
            DateTimeOffset minValue = DateTimeOffset.MinValue;
            _minTimestamp = minValue.ToUnixTimeMilliseconds();
            minValue = DateTimeOffset.MaxValue;
            _maxTimestamp = minValue.ToUnixTimeMilliseconds();
        }


        public static long GetTimesMorning(long timestamp)
        {
            // 这里可以根据你的需求进行日期操作
            // 以下示例将时间戳转换为 DateTime，并将时间设置为当天的凌晨，然后再转换回时间戳
            DateTime dateTime = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
            DateTime morningDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc);
            long morningTimestamp = morningDateTime.Ticks / TimeSpan.TicksPerMillisecond;

            return morningTimestamp;
        }
    }
}
