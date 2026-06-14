namespace MapToSuccess.Examples.RegistryLookup;

/// <summary>
/// Builds a deterministic list of ids to look up. About half are real ids (hits)
/// and half are deliberately out of range (misses) — misses are the worst case for
/// the linear scan because it has to walk the entire list before giving up.
/// </summary>
public static class RegistryWorkload
{
    /// <summary>
    /// Produces <paramref name="lookupCount"/> query ids against a population of
    /// <paramref name="populationSize"/> people (valid ids are 0 .. populationSize-1).
    /// </summary>
    public static IReadOnlyList<int> BuildQueryIds(int lookupCount, int populationSize, int seed = 99)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(lookupCount);
        ArgumentOutOfRangeException.ThrowIfNegative(populationSize);

        Random random = new(seed);
        int[] ids = new int[lookupCount];
        for (int i = 0; i < lookupCount; i++)
        {
            // Even index → an id that exists (a hit); odd index → an id past the end
            // (a guaranteed miss). Alternating keeps the hit/miss ratio stable.
            ids[i] = i % 2 == 0 && populationSize > 0
                ? random.Next(populationSize)
                : populationSize + random.Next(1, 1000);
        }

        return ids;
    }
}
