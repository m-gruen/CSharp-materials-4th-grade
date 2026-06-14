namespace MapToSuccess.Examples.MiniDictionary;

/// <summary>
/// A deliberately small, readable hash map you implement yourself. It uses
/// <em>separate chaining</em>: an array of buckets, where each bucket holds a short list
/// of <see cref="Entry"/> items (the pairs that hashed to that slot).
/// <para>
/// The map must call the <em>key's own</em> <see cref="object.GetHashCode"/> and
/// <see cref="object.Equals(object)"/> — that is why a good key matters (see the
/// "Designing good keys" example).
/// </para>
/// This is for learning; real code uses <see cref="System.Collections.Generic.Dictionary{TKey,TValue}"/>.
/// </summary>
public sealed class MiniDictionary<TKey, TValue>
    where TKey : notnull
{
    /// <summary>One stored key/value pair living inside a bucket's chain.</summary>
    private sealed class Entry(TKey key, TValue value)
    {
        public TKey Key { get; } = key;

        public TValue Value { get; set; } = value;
    }

    // Each bucket is a short "chain" of entries. The whole point of a hash map is that a
    // lookup only ever scans ONE of these chains, never the entire collection.
    private List<Entry>[] _buckets;

    /// <summary>Creates an empty map with the given number of starting buckets.</summary>
    public MiniDictionary(int initialBucketCount = 4)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(initialBucketCount, 1);
        _buckets = CreateBuckets(initialBucketCount);
    }

    /// <summary>Number of key/value pairs currently stored. Update this as you add/remove.</summary>
    public int Count { get; private set; }

    /// <summary>Number of buckets in the backing array.</summary>
    public int BucketCount => _buckets.Length;

    /// <summary>Entries per bucket on average — how "full" the table is.</summary>
    public double LoadFactor => (double)Count / _buckets.Length;

    /// <summary>Maps a key to its bucket slot.</summary>
    public int GetBucketIndex(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);

        // TODO: turn the key into a valid bucket index:
        //   1. start from key.GetHashCode()
        //   2. mask off the sign bit so it is never negative:  value & int.MaxValue
        //      (a hash code can be negative, but an array index cannot)
        //   3. wrap it into range:  % _buckets.Length
        throw new NotImplementedException();
    }

    /// <summary>Gets or sets the value for <paramref name="key"/>.</summary>
    public TValue this[TKey key]
    {
        // TODO (get): return the value for key. If it is missing, throw
        //             new KeyNotFoundException(...). Hint: reuse TryGetValue.
        get => throw new NotImplementedException();

        // TODO (set): find the entry in the key's bucket. If it exists, overwrite its Value;
        //             otherwise add a new Entry (and increase Count).
        set => throw new NotImplementedException();
    }

    /// <summary>Adds a new pair. Throws if the key already exists.</summary>
    public void Add(TKey key, TValue value)
    {
        // TODO:
        //   - if the key already exists, throw
        //       new ArgumentException($"...{key}...", nameof(key));
        //   - otherwise grow the table first if adding one more pair would make it more
        //     than ~75% full (see the Grow hint at the bottom), then add
        //       new Entry(key, value)  to  _buckets[GetBucketIndex(key)]  and increase Count.
        throw new NotImplementedException();
    }

    /// <summary>Returns <c>true</c> and the value when the key is present.</summary>
    public bool TryGetValue(TKey key, out TValue value)
    {
        // TODO: look ONLY inside _buckets[GetBucketIndex(key)]. If an entry's Key.Equals(key),
        //       set value to that entry's Value and return true. Otherwise set
        //       value = default! and return false.
        throw new NotImplementedException();
    }

    /// <summary>Returns <c>true</c> when the key is present.</summary>
    public bool ContainsKey(TKey key)
    {
        // TODO: return whether the key is present. Hint: you can reuse TryGetValue.
        throw new NotImplementedException();
    }

    /// <summary>Removes the key. Returns <c>true</c> when something was removed.</summary>
    public bool Remove(TKey key)
    {
        // TODO: in the key's bucket, find the entry whose Key.Equals(key); remove it,
        //       decrease Count and return true. Return false if the key is not there.
        throw new NotImplementedException();
    }

    /// <summary>Snapshot of one bucket's chain, used by the visualiser and the tests.</summary>
    public IReadOnlyList<KeyValuePair<TKey, TValue>> GetBucket(int bucketIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(bucketIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(bucketIndex, _buckets.Length);
        return [.. _buckets[bucketIndex].Select(entry => new KeyValuePair<TKey, TValue>(entry.Key, entry.Value))];
    }

    // Provided helper: builds an array of empty buckets.
    private static List<Entry>[] CreateBuckets(int count)
    {
        List<Entry>[] result = new List<Entry>[count];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = [];
        }

        return result;
    }

    // GROW HINT (do this once your basic version passes its tests):
    // Long chains make lookups slow, so the table should grow when it gets crowded.
    // Add a private method that, when Count would pass ~75% of _buckets.Length, replaces
    // `_buckets` with a bigger array (e.g. _buckets.Length * 2) and RE-INSERTS every existing
    // entry — its bucket index changes because it depends on _buckets.Length. Call it from
    // Add before inserting a brand-new key.
}
