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

        // TODO: walk the characters; collect runs of letters (use char.IsLetter) into
        //       words, lower-cased with char.ToLowerInvariant. "Hello, HELLO" -> ["hello","hello"].
        throw new NotImplementedException();
    }

    /// <summary>Beginner style: <c>ContainsKey</c> then the indexer (looks the key up twice).</summary>
    public static Dictionary<string, int> CountWithContainsKey(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        // TODO: for each word, if counts.ContainsKey(word) then counts[word]++ else counts[word] = 1.
        throw new NotImplementedException();
    }

    /// <summary><c>TryGetValue</c> gives the current count (0 when missing) in a single lookup.</summary>
    public static Dictionary<string, int> CountWithTryGetValue(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        // TODO: for each word, counts.TryGetValue(word, out int current); counts[word] = current + 1;
        throw new NotImplementedException();
    }

    /// <summary>The most compact imperative form, using <c>GetValueOrDefault</c>.</summary>
    public static Dictionary<string, int> CountWithGetValueOrDefault(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        // TODO: for each word, counts[word] = counts.GetValueOrDefault(word) + 1;
        throw new NotImplementedException();
    }

    /// <summary>The declarative LINQ form (the same shape you use against Entity Framework).</summary>
    public static Dictionary<string, int> CountWithLinq(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        // TODO: words.GroupBy(word => word).ToDictionary(group => group.Key, group => group.Count())
        throw new NotImplementedException();
    }

    /// <summary>Records the position at which each word was <em>first</em> seen, using <c>TryAdd</c>.</summary>
    public static Dictionary<string, int> FirstSeenPositions(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        // TODO: keep a running position counter; for each word call firstSeen.TryAdd(word, position).
        //       TryAdd stores only when the key is new, so later repeats are ignored.
        throw new NotImplementedException();
    }

    /// <summary>The <paramref name="count"/> most frequent words, ties broken alphabetically.</summary>
    public static IReadOnlyList<KeyValuePair<string, int>> TopWords(
        IReadOnlyDictionary<string, int> counts,
        int count)
    {
        ArgumentNullException.ThrowIfNull(counts);
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        // TODO: order by value descending, then by key (StringComparer.Ordinal) for ties,
        //       then Take(count) and materialise to a list.
        throw new NotImplementedException();
    }
}
