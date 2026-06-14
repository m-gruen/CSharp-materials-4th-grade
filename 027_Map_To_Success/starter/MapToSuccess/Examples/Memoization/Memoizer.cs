namespace MapToSuccess.Examples.Memoization;

/// <summary>
/// A cache for an expensive but pure function: the first time a key is requested the function
/// runs and the result is stored; every later request for the same key is served straight from
/// the dictionary. This "compute once, reuse" pattern is another everyday use of a map.
/// </summary>
public sealed class Memoizer<TKey, TResult>
    where TKey : notnull
{
    private readonly Func<TKey, TResult> _compute;
    private readonly Dictionary<TKey, TResult> _cache = new();

    /// <summary>Wraps the expensive function whose results should be cached.</summary>
    public Memoizer(Func<TKey, TResult> compute)
    {
        ArgumentNullException.ThrowIfNull(compute);
        _compute = compute;
    }

    /// <summary>How many requests were served from the cache.</summary>
    public int Hits { get; private set; }

    /// <summary>How many requests had to run the expensive function.</summary>
    public int Misses { get; private set; }

    /// <summary>Number of distinct keys currently cached.</summary>
    public int Count => _cache.Count;

    /// <summary>
    /// Returns the (possibly cached) result for <paramref name="key"/>, running the expensive
    /// function only on the first request for each distinct key.
    /// </summary>
    public TResult Get(TKey key)
    {
        if (_cache.TryGetValue(key, out TResult? cached))
        {
            // Cache hit: we already computed this one.
            Hits++;
            return cached;
        }

        // Cache miss: run the expensive function exactly once for this key...
        TResult value = _compute(key);

        // TODO: ...then store value in the cache under key, count a Miss (Misses++), and return value.
        throw new NotImplementedException();
    }
}
