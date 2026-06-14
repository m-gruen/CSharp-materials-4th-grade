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
        // TODO: build a Dictionary<Coordinate,string>, add new Coordinate(48,16) -> "Vienna",
        //       then return places.ContainsKey(new Coordinate(48, 16)).
        throw new NotImplementedException();
    }

    /// <summary>A reference key is found via the exact same instance it was stored with.</summary>
    public static bool ReferenceKeyFindsSameInstance()
    {
        // TODO: create one ReferenceKey, store it, and return whether ContainsKey finds it
        //       using that SAME instance (true).
        throw new NotImplementedException();
    }

    /// <summary>
    /// A reference key (no equality overrides) is <em>missed</em> when looked up with a different
    /// instance holding the same value. Return <c>true</c> when that trap is reproduced.
    /// </summary>
    public static bool ReferenceKeyMissesFreshInstance()
    {
        // TODO: store new ReferenceKey(1), then return !map.ContainsKey(new ReferenceKey(1)).
        //       (A different instance does not match, because ReferenceKey has no equality.)
        throw new NotImplementedException();
    }

    /// <summary>
    /// After mutating a stored key, the mutated key can no longer find its own entry (its hash
    /// code changed). Return <c>true</c> when the entry can no longer be found.
    /// </summary>
    public static bool MutatedKeyEntryIsLost()
    {
        // TODO: create a MutableKey { Id = 1 }, store a value under it, THEN set key.Id = 2,
        //       and return !map.ContainsKey(key).
        throw new NotImplementedException();
    }

    /// <summary>After the mutation, even a fresh key with the original value cannot find the entry.</summary>
    public static bool MutatedEntryIsLostToOriginalValueToo()
    {
        // TODO: same setup as above; after mutating, return
        //       !map.ContainsKey(new MutableKey { Id = 1 }).
        throw new NotImplementedException();
    }

    /// <summary>With <see cref="StringComparer.OrdinalIgnoreCase"/>, "vienna" finds "Vienna".</summary>
    public static bool OrdinalIgnoreCaseFinds()
    {
        // TODO: build a Dictionary<string,int>(StringComparer.OrdinalIgnoreCase) with "Vienna",
        //       then return whether it ContainsKey("vienna").
        throw new NotImplementedException();
    }

    /// <summary>The default (ordinal) comparer treats "vienna" and "Vienna" as different keys.</summary>
    public static bool DefaultComparerMisses()
    {
        // TODO: build a plain Dictionary<string,int> with "Vienna", then return
        //       !dictionary.ContainsKey("vienna").
        throw new NotImplementedException();
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
        // TODO: from BookingsByDate(), look up new LocalDate(2026, 5, 31) and return whether the
        //       value you get back is 3 (use TryGetValue or the indexer).
        throw new NotImplementedException();
    }
}
