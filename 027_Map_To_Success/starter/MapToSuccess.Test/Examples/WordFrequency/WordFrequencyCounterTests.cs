namespace MapToSuccess.Test.Examples.WordFrequency;

using global::MapToSuccess.Examples.WordFrequency;

public sealed class WordFrequencyCounterTests
{
    [Fact]
    public void Tokenize_LowercasesAndSplitsOnNonLetters()
    {
        IReadOnlyList<string> words = WordFrequencyCounter.Tokenize("Hello, HELLO  world!");

        words.Should().Equal("hello", "hello", "world");
    }

    [Fact]
    public void Tokenize_EmptyText_ReturnsNoWords()
    {
        WordFrequencyCounter.Tokenize("   ...  ").Should().BeEmpty();
    }

    [Fact]
    public void Tokenize_Null_Throws()
    {
        Action act = () => WordFrequencyCounter.Tokenize(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AllCountingApproaches_ProduceTheSameCounts()
    {
        IReadOnlyList<string> words = WordFrequencyCounter.Tokenize("the cat sat on the mat the cat");
        Dictionary<string, int> expected = new()
        {
            ["the"] = 3,
            ["cat"] = 2,
            ["sat"] = 1,
            ["on"] = 1,
            ["mat"] = 1,
        };

        WordFrequencyCounter.CountWithContainsKey(words).Should().Equal(expected);
        WordFrequencyCounter.CountWithTryGetValue(words).Should().Equal(expected);
        WordFrequencyCounter.CountWithGetValueOrDefault(words).Should().Equal(expected);
        WordFrequencyCounter.CountWithLinq(words).Should().Equal(expected);
    }

    [Fact]
    public void CountingApproaches_RejectNullInput()
    {
        IEnumerable<string> nullWords = null!;

        ((Action)(() => WordFrequencyCounter.CountWithContainsKey(nullWords))).Should().Throw<ArgumentNullException>();
        ((Action)(() => WordFrequencyCounter.CountWithTryGetValue(nullWords))).Should().Throw<ArgumentNullException>();
        ((Action)(() => WordFrequencyCounter.CountWithGetValueOrDefault(nullWords))).Should().Throw<ArgumentNullException>();
        ((Action)(() => WordFrequencyCounter.CountWithLinq(nullWords))).Should().Throw<ArgumentNullException>();
        ((Action)(() => WordFrequencyCounter.FirstSeenPositions(nullWords))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FirstSeenPositions_KeepsTheEarliestIndexForRepeatedWords()
    {
        IReadOnlyList<string> words = WordFrequencyCounter.Tokenize("a b a c b");

        Dictionary<string, int> firstSeen = WordFrequencyCounter.FirstSeenPositions(words);

        firstSeen.Should().Equal(new Dictionary<string, int>
        {
            ["a"] = 0,
            ["b"] = 1,
            ["c"] = 3,
        });
    }

    [Fact]
    public void TopWords_OrdersByCountThenAlphabetically()
    {
        Dictionary<string, int> counts = new()
        {
            ["the"] = 5,
            ["fox"] = 3,
            ["dog"] = 2,
            ["cat"] = 2,
            ["owl"] = 1,
        };

        IReadOnlyList<KeyValuePair<string, int>> top = WordFrequencyCounter.TopWords(counts, 3);

        top.Select(p => p.Key).Should().Equal("the", "fox", "cat"); // cat before dog: tie broken alphabetically
    }

    [Fact]
    public void TopWords_NegativeCount_Throws()
    {
        Action act = () => WordFrequencyCounter.TopWords(new Dictionary<string, int>(), -1);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void TopWords_NullCounts_Throws()
    {
        Action act = () => WordFrequencyCounter.TopWords(null!, 1);

        act.Should().Throw<ArgumentNullException>();
    }
}
