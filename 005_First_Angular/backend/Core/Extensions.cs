namespace TankMuseum.Core;

public static class Extensions
{
    public static ZonedDateTime ToLocalDateTime(this Instant instant) => instant.InZone(Const.TimeZone);

    public static Instant ToStartOfDayInstant(this LocalDate date)
    {
        var dateTime = date.AtStartOfDayInZone(Const.TimeZone);

        return dateTime.ToInstant();
    }
}
