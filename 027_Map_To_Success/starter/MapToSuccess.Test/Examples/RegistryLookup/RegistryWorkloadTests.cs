namespace MapToSuccess.Test.Examples.RegistryLookup;

using global::MapToSuccess.Examples.RegistryLookup;

public sealed class RegistryWorkloadTests
{
    [Fact]
    public void BuildQueryIds_ReturnsRequestedCount_WithHitsAndMisses()
    {
        const int Population = 1000;

        IReadOnlyList<int> ids = RegistryWorkload.BuildQueryIds(lookupCount: 100, populationSize: Population);

        ids.Should().HaveCount(100);
        ids.Where(id => id < Population).Should().NotBeEmpty("some queries must be hits");
        ids.Where(id => id >= Population).Should().NotBeEmpty("some queries must be misses");
    }

    [Fact]
    public void BuildQueryIds_IsDeterministicForAGivenSeed()
    {
        IReadOnlyList<int> first = RegistryWorkload.BuildQueryIds(50, 1000, seed: 7);
        IReadOnlyList<int> second = RegistryWorkload.BuildQueryIds(50, 1000, seed: 7);

        second.Should().Equal(first);
    }

    [Fact]
    public void BuildQueryIds_WithEmptyPopulation_ProducesOnlyMisses()
    {
        IReadOnlyList<int> ids = RegistryWorkload.BuildQueryIds(10, populationSize: 0);

        ids.Should().HaveCount(10);
        ids.Should().OnlyContain(id => id >= 0);
    }

    [Theory]
    [InlineData(-1, 10)]
    [InlineData(10, -1)]
    public void BuildQueryIds_NegativeArguments_Throw(int lookupCount, int populationSize)
    {
        Action act = () => RegistryWorkload.BuildQueryIds(lookupCount, populationSize);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
