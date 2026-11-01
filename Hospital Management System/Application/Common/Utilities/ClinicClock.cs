namespace Hospital_Management_System.Application.Common.Utilities
{
    public static class ClinicClock
    {
        private static readonly TimeZoneInfo Zone = ResolveZone(); 

        public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zone); 

        public static DateOnly Today => DateOnly.FromDateTime(Now); 

        private static TimeZoneInfo ResolveZone()
        {
            foreach (var id in new[] { "Egypt Standard Time", "Africa/Cairo" }) 
            {
                try 
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(id);
                }
                catch (TimeZoneNotFoundException) { } 
                catch (InvalidTimeZoneException) { } 
            }

            return TimeZoneInfo.Utc; 
        }
    }
}
