namespace MapToSuccess.Test.Examples.RegistryLookup;

using global::MapToSuccess.Examples.RegistryLookup;

public sealed class LookupBenchmarkTests
{
    private static IReadOnlyList<Person> People => PersonGenerator.Generate(100);

    [Fact]
    public void Measure_CountsHitsAndChecksumsTheFoundIds()
    {
        IPersonRegistry registry = new DictionaryRegistry(People);
        int[] queries = [1, 2, 500, 3]; // 500 is a miss

        BenchmarkResult result = LookupBenchmark.Measure("dict", registry, queries);

        result.Lookups.Should().Be(4);
        result.Hits.Should().Be(3);
        result.Checksum.Should().Be(1 + 2 + 3);
        result.Elapsed.Should().BeGreaterThanOrEqualTo(TimeSpan.Zero);
    }

    [Fact]
    public void Measure_LinearAndDictionary_ProduceIdenticalResults()
    {
        IReadOnlyList<Person> people = People;
        IReadOnlyList<int> queries = RegistryWorkload.BuildQueryIds(40, 100);

        BenchmarkResult linear = LookupBenchmark.Measure("linear", new LinearRegistry(people), queries);
        BenchmarkResult dictionary = LookupBenchmark.Measure("dict", new DictionaryRegistry(people), queries);

        linear.Hits.Should().Be(dictionary.Hits);
        linear.Checksum.Should().Be(dictionary.Checksum);
    }

    [Fact]
    public void Measure_BlankStrategy_Throws()
    {
        Action act = () => LookupBenchmark.Measure(" ", new DictionaryRegistry(People), [1]);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Measure_NullRegistry_Throws()
    {
        Action act = () => LookupBenchmark.Measure("x", null!, [1]);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Measure_NullQueries_Throws()
    {
        Action act = () => LookupBenchmark.Measure("x", new DictionaryRegistry(People), null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void PerLookup_IsZero_WhenNoLookups()
    {
        BenchmarkResult empty = LookupBenchmark.Measure("x", new DictionaryRegistry(People), []);

        empty.Lookups.Should().Be(0);
        empty.PerLookup.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void PerLookup_DividesElapsedByLookupCount()
    {
        BenchmarkResult result = new("x", Lookups: 4, Hits: 4, Checksum: 0, Elapsed: TimeSpan.FromMilliseconds(40));

        result.PerLookup.Should().Be(TimeSpan.FromMilliseconds(10));
    }
}
