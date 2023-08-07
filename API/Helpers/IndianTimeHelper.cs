namespace API.Helpers
{
    public class IndianTimeHelper
    {
        public static DateTime GetIndianLocalTime()
        {
            string timeZoneId = "India Standard Time";
            TimeZoneInfo indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            DateTime utcNow = DateTime.UtcNow;
            DateTime indianLocalTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, indianTimeZone);
            return indianLocalTime;
        }
    }
}
