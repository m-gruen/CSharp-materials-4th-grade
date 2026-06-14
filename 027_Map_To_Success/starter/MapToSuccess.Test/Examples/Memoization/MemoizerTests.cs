namespace MapToSuccess.Test.Examples.Memoization;

using global::MapToSuccess.Examples.Memoization;

public sealed class MemoizerTests
{
    [Fact]
    public void Get_RunsTheFunctionOncePerDistinctKey()
    {
        int calls = 0;
        Memoizer<int, int> memoizer = new(key =>
        {
            calls++;
            return key * key;
        });

        int[] results = [memoizer.Get(2), memoizer.Get(3), memoizer.Get(2), memoizer.Get(2)];

        results.Should().Equal(4, 9, 4, 4);
        calls.Should().Be(2, "only the two distinct keys trigger a computation");
    }

    [Fact]
    public void Get_TracksHitsMissesAndCount()
    {
        Memoizer<int, int> memoizer = new(key => key * key);

        memoizer.Get(2); // miss
        memoizer.Get(2); // hit
        memoizer.Get(3); // miss

        memoizer.Misses.Should().Be(2);
        memoizer.Hits.Should().Be(1);
        memoizer.Count.Should().Be(2);
    }

    [Fact]
    public void Get_ReturnsTheCachedValue_OnRepeatedCalls()
    {
        // The function returns a fresh object each call; memoization must hand back the
        // very same cached instance the second time.
        Memoizer<int, object> memoizer = new(_ => new object());

        object first = memoizer.Get(1);
        object second = memoizer.Get(1);

        second.Should().BeSameAs(first);
    }

    [Fact]
    public void Constructor_NullFunction_Throws()
    {
        Action act = () => _ = new Memoizer<int, int>(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
