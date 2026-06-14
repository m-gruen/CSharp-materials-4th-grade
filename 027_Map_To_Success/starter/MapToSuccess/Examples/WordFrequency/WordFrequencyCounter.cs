namespace MapToSuccess.Examples.WordFrequency;

/// <summary>
/// Counting how often each word appears is the classic "what is a dictionary for"
/// example: the key is the word, the value is the running total. You will implement the
/// count several equivalent ways so you can compare the common
/// <see cref="System.Collections.Generic.Dictionary{TKey,TValue}"/> APIs and see they all
/// give the same answer.
/// </summary>
public static class WordFrequencyCounter
{
    /// <summary>Splits text into lower-cased words, treating any non-letter as a separator.</summary>
    public static IReadOnlyList<string> Tokenize(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var words = new List<string>();
        var current = new System.Text.StringBuilder();

        foreach (char c in text)
        {
            if (char.IsLetter(c))
            {
                current.Append(char.ToLowerInvariant(c));
            }
            else if (current.Length > 0)
            {
                words.Add(current.ToString());
                current.Clear();
            }
        }

        if (current.Length > 0)
        {
            words.Add(current.ToString());
        }

        return words;
    }

    /// <summary>Beginner style: <c>ContainsKey</c> then the indexer (looks the key up twice).</summary>
    public static Dictionary<string, int> CountWithContainsKey(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        var counts = new Dictionary<string, int>();
        foreach (string word in words)
        {
            if (counts.TryGetValue(word, out int value))
            {
                counts[word] = ++value;
            }
            else
            {
                counts[word] = 1;
            }
        }
        return counts;
    }

    /// <summary><c>TryGetValue</c> gives the current count (0 when missing) in a single lookup.</summary>
    public static Dictionary<string, int> CountWithTryGetValue(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        var counts = new Dictionary<string, int>();
        foreach (string word in words)
        {
            counts.TryGetValue(word, out int current);
            counts[word] = current + 1;
        }
        return counts;
    }

    /// <summary>The most compact imperative form, using <c>GetValueOrDefault</c>.</summary>
    public static Dictionary<string, int> CountWithGetValueOrDefault(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        var counts = new Dictionary<string, int>();
        foreach (string word in words)
        {
            counts[word] = counts.GetValueOrDefault(word) + 1;
        }
        return counts;
    }

    /// <summary>The declarative LINQ form (the same shape you use against Entity Framework).</summary>
    public static Dictionary<string, int> CountWithLinq(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        return words.GroupBy(word => word).ToDictionary(group => group.Key, group => group.Count());
    }

    /// <summary>Records the position at which each word was <em>first</em> seen, using <c>TryAdd</c>.</summary>
    public static Dictionary<string, int> FirstSeenPositions(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        var firstSeen = new Dictionary<string, int>();
        int position = 0;
        foreach (string word in words)
        {
            firstSeen.TryAdd(word, position);
            position++;
        }
        return firstSeen;
    }

    /// <summary>The <paramref name="count"/> most frequent words, ties broken alphabetically.</summary>
    public static IReadOnlyList<KeyValuePair<string, int>> TopWords(
        IReadOnlyDictionary<string, int> counts,
        int count)
    {
        ArgumentNullException.ThrowIfNull(counts);
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        return [.. counts
            .OrderByDescending(kv => kv.Value)
            .ThenBy(kv => kv.Key, StringComparer.Ordinal)
            .Take(count)];
    }
}
