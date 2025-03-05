namespace FinTrack.API.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToTimestampLong(this DateTime utc)
        {
            return (long)(new DateTimeOffset(utc) - DateTimeOffset.UnixEpoch).TotalSeconds;
        }
    }
}
