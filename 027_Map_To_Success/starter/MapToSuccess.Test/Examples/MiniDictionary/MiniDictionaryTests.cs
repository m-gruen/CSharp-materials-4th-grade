namespace MapToSuccess.Test.Examples.MiniDictionary;

using global::MapToSuccess.Examples.MiniDictionary;

public sealed class MiniDictionaryTests
{
    [Fact]
    public void Constructor_RejectsBucketCountBelowOne()
    {
        Action act = () => _ = new MiniDictionary<int, string>(initialBucketCount: 0);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void NewMap_IsEmpty()
    {
        MiniDictionary<int, string> map = new();

        map.Count.Should().Be(0);
        map.BucketCount.Should().Be(4);
        map.LoadFactor.Should().Be(0d);
    }

    [Fact]
    public void Add_StoresValue_AndIsRetrievable()
    {
        MiniDictionary<int, string> map = new();

        map.Add(1, "one");

        map.Count.Should().Be(1);
        map.ContainsKey(1).Should().BeTrue();
        map[1].Should().Be("one");
    }

    [Fact]
    public void Add_DuplicateKey_Throws()
    {
        MiniDictionary<int, string> map = new();
        map.Add(1, "one");

        Action act = () => map.Add(1, "uno");

        act.Should().Throw<ArgumentException>().WithParameterName("key");
    }

    [Fact]
    public void Indexer_Set_AddsThenOverwrites()
    {
        MiniDictionary<int, string> map = new();

        map[1] = "one";
        map[1] = "uno";

        map.Count.Should().Be(1);
        map[1].Should().Be("uno");
    }

    [Fact]
    public void Indexer_Get_MissingKey_Throws()
    {
        MiniDictionary<int, string> map = new();

        Action act = () => _ = map[42];

        act.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void TryGetValue_ReportsHitAndMiss()
    {
        MiniDictionary<int, string> map = new();
        map.Add(1, "one");

        map.TryGetValue(1, out string? hit).Should().BeTrue();
        hit.Should().Be("one");

        map.TryGetValue(2, out string? miss).Should().BeFalse();
        miss.Should().BeNull();
    }

    [Fact]
    public void ContainsKey_FalseForMissingKey()
    {
        MiniDictionary<int, string> map = new();

        map.ContainsKey(99).Should().BeFalse();
    }

    [Fact]
    public void Remove_ExistingKey_ReturnsTrueAndShrinksCount()
    {
        MiniDictionary<int, string> map = new();
        map.Add(1, "one");

        map.Remove(1).Should().BeTrue();
        map.Count.Should().Be(0);
        map.ContainsKey(1).Should().BeFalse();
    }

    [Fact]
    public void Remove_MissingKey_ReturnsFalse()
    {
        MiniDictionary<int, string> map = new();

        map.Remove(1).Should().BeFalse();
    }

    [Fact]
    public void Remove_WalksCollisionChain_ToFindLaterEntry()
    {
        // With 4 buckets, 1 and 5 both hash to bucket 1 (1%4 == 5%4 == 1).
        MiniDictionary<int, string> map = new(initialBucketCount: 4);
        map.Add(1, "one");
        map.Add(5, "five");
        map.GetBucketIndex(1).Should().Be(map.GetBucketIndex(5));

        map.Remove(5).Should().BeTrue();

        map.ContainsKey(1).Should().BeTrue();
        map.ContainsKey(5).Should().BeFalse();
    }

    [Fact]
    public void GetBucketIndex_IsNonNegative_EvenForNegativeKeys()
    {
        MiniDictionary<int, string> map = new(initialBucketCount: 8);

        int index = map.GetBucketIndex(-12345);

        index.Should().BeInRange(0, map.BucketCount - 1);
    }

    [Fact]
    public void GetBucketIndex_NullKey_Throws()
    {
        MiniDictionary<string, int> map = new();

        Action act = () => map.GetBucketIndex(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetBucket_GroupsCollidingKeysIntoOneChain()
    {
        MiniDictionary<int, string> map = new(initialBucketCount: 4);
        map.Add(5, "five");
        map.Add(9, "nine"); // 5%4 == 9%4 == 1

        IReadOnlyList<KeyValuePair<int, string>> chain = map.GetBucket(1);

        chain.Should().HaveCount(2);
        chain.Select(e => e.Key).Should().BeEquivalentTo([5, 9]);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public void GetBucket_OutOfRange_Throws(int bucketIndex)
    {
        MiniDictionary<int, string> map = new(initialBucketCount: 4);

        Action act = () => map.GetBucket(bucketIndex);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Map_GrowsAndRehashes_KeepingEveryEntryReachable()
    {
        MiniDictionary<int, string> map = new(initialBucketCount: 4);

        for (int id = 1; id <= 20; id++)
        {
            map.Add(id, $"item-{id}");
        }

        map.Count.Should().Be(20);
        map.BucketCount.Should().BeGreaterThan(4, "the table must grow to keep chains short");
        map.LoadFactor.Should().BeLessThanOrEqualTo(0.75);

        for (int id = 1; id <= 20; id++)
        {
            map[id].Should().Be($"item-{id}");
        }
    }
}
