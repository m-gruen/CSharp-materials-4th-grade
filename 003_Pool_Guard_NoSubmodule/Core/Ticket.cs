namespace PoolGuard.Core;

public sealed class Ticket
{
    private readonly List<AccessEvent> _accessEvents = [];
    public Guid Id { get; init; }
    public Instant ValidFrom { get; init; }
    public Instant ValidTo { get; init; }
    public required string PersonName { get; init; }

    public bool IsInPoolArea => _accessEvents.Count > 0
        && _accessEvents[^1].Type is AccessEventType.Entered;

    public bool AddAccessEvent(AccessEventType type, Instant timestamp)
    {
        if (timestamp < ValidFrom || timestamp > ValidTo)
        {
            return false;
        }

        switch (type)
        {
            case AccessEventType.Entered when IsInPoolArea:
            case AccessEventType.Exited when !IsInPoolArea:
                {
                    return false;
                }
            case AccessEventType.Entered or AccessEventType.Exited:
                {
                    _accessEvents.Add(new AccessEvent(timestamp, type));
                    return true;
                }
            default:
                {
                    throw new ArgumentOutOfRangeException(nameof(type), type, "unknown access type");
                }
        }
    }
}

public readonly record struct AccessEvent(Instant Timestamp, AccessEventType Type);

public enum AccessEventType
{
    Entered = 10,
    Exited = 20
}
