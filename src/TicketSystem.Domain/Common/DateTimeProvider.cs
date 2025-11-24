namespace TicketSystem.Domain.Common;

/// <summary>
/// Provides centralized DateTime management for the application
/// Uses Turkey timezone (UTC+3)
/// </summary>
public static class DateTimeProvider
{
    private static readonly TimeZoneInfo TurkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

    /// <summary>
    /// Gets current date and time in Turkey timezone (UTC+3)
    /// </summary>
    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TurkeyTimeZone);

    /// <summary>
    /// Converts UTC DateTime to Turkey timezone
    /// </summary>
    public static DateTime ToTurkeyTime(DateTime utcDateTime)
    {
        if (utcDateTime.Kind != DateTimeKind.Utc)
        {
            utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        }
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TurkeyTimeZone);
    }

    /// <summary>
    /// Converts Turkey time to UTC
    /// </summary>
    public static DateTime ToUtc(DateTime turkeyDateTime)
    {
        return TimeZoneInfo.ConvertTimeToUtc(turkeyDateTime, TurkeyTimeZone);
    }
}
