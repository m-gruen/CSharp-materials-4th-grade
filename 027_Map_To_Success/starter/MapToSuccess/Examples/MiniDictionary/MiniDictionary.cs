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
        return (key.GetHashCode() & int.MaxValue) % _buckets.Length;
    }

    /// <summary>Gets or sets the value for <paramref name="key"/>.</summary>
    public TValue this[TKey key]
    {
        get
        {
            if (!TryGetValue(key, out TValue value))
            {

                throw new KeyNotFoundException($"The key '{key}' was not found.");
            }


            return value;
        }

        set
        {
            var bucket = _buckets[GetBucketIndex(key)];
            var entry = bucket.FirstOrDefault(e => e.Key.Equals(key));
            if (entry is not null)
            {
                entry.Value = value;
            }
            else
            {
                bucket.Add(new Entry(key, value));
                Count++;
            }
        }
    }

    /// <summary>Adds a new pair. Throws if the key already exists.</summary>
    public void Add(TKey key, TValue value)
    {
        if (ContainsKey(key))
        {

            throw new ArgumentException($"An item with the key '{key}' already exists.", nameof(key));
        }


        if ((Count + 1) > _buckets.Length * 0.75)
        {
            Grow();
        }


        _buckets[GetBucketIndex(key)].Add(new Entry(key, value));
        Count++;
    }

    /// <summary>Returns <c>true</c> and the value when the key is present.</summary>
    public bool TryGetValue(TKey key, out TValue value)
    {
        var bucket = _buckets[GetBucketIndex(key)];
        foreach (var entry in bucket)
        {
            if (entry.Key.Equals(key))
            {
                value = entry.Value;
                return true;
            }
        }
        value = default!;
        return false;
    }

    /// <summary>Returns <c>true</c> when the key is present.</summary>
    public bool ContainsKey(TKey key) => TryGetValue(key, out _);

    /// <summary>Removes the key. Returns <c>true</c> when something was removed.</summary>
    public bool Remove(TKey key)
    {
        var bucket = _buckets[GetBucketIndex(key)];
        var entry = bucket.FirstOrDefault(e => e.Key.Equals(key));
        if (entry is null)
        {

            return false;
        }


        bucket.Remove(entry);
        Count--;
        return true;
    }

    /// <summary>Snapshot of one bucket's chain, used by the visualiser and the tests.</summary>
    public IReadOnlyList<KeyValuePair<TKey, TValue>> GetBucket(int bucketIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(bucketIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(bucketIndex, _buckets.Length);
        return [.. _buckets[bucketIndex].Select(entry => new KeyValuePair<TKey, TValue>(entry.Key, entry.Value))];
    }

    private void Grow()
    {
        var newBuckets = CreateBuckets(_buckets.Length * 2);
        foreach (var bucket in _buckets)
        {
            foreach (var entry in bucket)
            {
                int newIndex = (entry.Key.GetHashCode() & int.MaxValue) % newBuckets.Length;
                newBuckets[newIndex].Add(entry);
            }
        }
        _buckets = newBuckets;
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
}
