namespace MapToSuccess.Examples.Keys;

using NodaTime;

/// <summary>
/// What makes a good dictionary key. The dictionary finds entries by hashing the key and then
/// comparing with equality, so the key's <c>GetHashCode</c>/<c>Equals</c> (or a supplied
/// <see cref="IEqualityComparer{T}"/>) decide everything. Each method returns a single boolean
/// so the behaviour can be asserted in tests — make each one return <c>true</c>.
/// </summary>
public static class KeyDemonstrations
{
    /// <summary>
    /// A value-based key (<see cref="Coordinate"/>) is found even with a brand-new instance,
    /// because equality is by value. Store one Coordinate, then look it up with a DIFFERENT
    /// Coordinate object of the same values; return whether it was found (should be true).
    /// </summary>
    public static bool ValueKeyFindsFreshInstance()
    {
        var places = new Dictionary<Coordinate, string>();
        places[new Coordinate(48, 16)] = "Vienna";
        return places.ContainsKey(new Coordinate(48, 16));
    }

    /// <summary>A reference key is found via the exact same instance it was stored with.</summary>
    public static bool ReferenceKeyFindsSameInstance()
    {
        var key = new ReferenceKey(1);
        var map = new Dictionary<ReferenceKey, string>();
        map[key] = "value";
        return map.ContainsKey(key);
    }

    /// <summary>
    /// A reference key (no equality overrides) is <em>missed</em> when looked up with a different
    /// instance holding the same value. Return <c>true</c> when that trap is reproduced.
    /// </summary>
    public static bool ReferenceKeyMissesFreshInstance()
    {
        var map = new Dictionary<ReferenceKey, string>();
        map[new ReferenceKey(1)] = "value";
        return !map.ContainsKey(new ReferenceKey(1));
    }

    /// <summary>
    /// After mutating a stored key, the mutated key can no longer find its own entry (its hash
    /// code changed). Return <c>true</c> when the entry can no longer be found.
    /// </summary>
    public static bool MutatedKeyEntryIsLost()
    {
        var key = new MutableKey { Id = 1 };
        var map = new Dictionary<MutableKey, string>();
        map[key] = "value";
        key.Id = 2;
        return !map.ContainsKey(key);
    }

    /// <summary>After the mutation, even a fresh key with the original value cannot find the entry.</summary>
    public static bool MutatedEntryIsLostToOriginalValueToo()
    {
        var key = new MutableKey { Id = 1 };
        var map = new Dictionary<MutableKey, string>();
        map[key] = "value";
        key.Id = 2;
        return !map.ContainsKey(new MutableKey { Id = 1 });
    }

    /// <summary>With <see cref="StringComparer.OrdinalIgnoreCase"/>, "vienna" finds "Vienna".</summary>
    public static bool OrdinalIgnoreCaseFinds()
    {
        var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        dict["Vienna"] = 1;
        return dict.ContainsKey("vienna");
    }

    /// <summary>The default (ordinal) comparer treats "vienna" and "Vienna" as different keys.</summary>
    public static bool DefaultComparerMisses()
    {
        var dict = new Dictionary<string, int>();
        dict["Vienna"] = 1;
        return !dict.ContainsKey("vienna");
    }

    /// <summary>A per-day bookings map keyed by a NodaTime <see cref="LocalDate"/> (provided).</summary>
    public static Dictionary<LocalDate, int> BookingsByDate() => new()
    {
        [new LocalDate(2026, 5, 31)] = 3,
        [new LocalDate(2026, 6, 1)] = 5,
    };

    /// <summary>
    /// <see cref="LocalDate"/> is a value type with built-in value equality, so a fresh date
    /// instance finds the entry with no extra code.
    /// </summary>
    public static bool LocalDateFindsFreshInstance()
    {
        var bookings = BookingsByDate();
        return bookings.TryGetValue(new LocalDate(2026, 5, 31), out int count) && count == 3;
    }
}
