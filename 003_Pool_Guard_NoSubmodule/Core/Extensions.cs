namespace PoolGuard.Core;

public static class Extensions
{
    public static ZonedDateTime ToLocalDateTime(this Instant self)
    {
        return self.InZone(Const.TimeZone);
    }
}