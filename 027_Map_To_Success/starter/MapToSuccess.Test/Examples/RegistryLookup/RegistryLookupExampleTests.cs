namespace MapToSuccess.Test.Examples.RegistryLookup;

using global::MapToSuccess.Examples.RegistryLookup;
using Spectre.Console.Testing;

public sealed class RegistryLookupExampleTests
{
    [Fact]
    public void Metadata_IsPresent()
    {
        RegistryLookupExample example = new();

        example.Title.Should().NotBeNullOrWhiteSpace();
        example.Description.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Run_WithSmallPopulation_ComparesBothStrategies()
    {
        // Tiny sizes keep the test fast; the real catalog entry uses millions.
        RegistryLookupExample example = new(populationSize: 5_000, lookupCount: 200, seed: 1);
        TestConsole console = new();
        console.Profile.Width = 240; // avoid table wrapping splitting asserted phrases

        example.Run(console);

        string output = console.Output;
        output.Should().Contain("List scan");
        output.Should().Contain("Dictionary lookup");
        output.Should().Contain("the same people");
        output.Should().Contain("faster");
    }

    [Fact]
    public void RenderComparison_ReportsFactor_AndIdenticalResults_WhenDictionaryIsInstant()
    {
        TestConsole console = new();
        console.Profile.Width = 240;
        BenchmarkResult linear = new("List scan", Lookups: 100, Hits: 50, Checksum: 1234, Elapsed: TimeSpan.FromSeconds(8));
        BenchmarkResult dictionary = new("Dictionary", Lookups: 100, Hits: 50, Checksum: 1234, Elapsed: TimeSpan.Zero);

        RegistryLookupExample.RenderComparison(console, linear, dictionary);

        console.Output.Should().Contain("the same people").And.Contain("thousands of times");
    }

    [Fact]
    public void RenderComparison_ReportsNumericFactor_AndFlagsDisagreement()
    {
        TestConsole console = new();
        console.Profile.Width = 240;
        BenchmarkResult linear = new("List scan", Lookups: 100, Hits: 50, Checksum: 1, Elapsed: TimeSpan.FromMilliseconds(800));
        BenchmarkResult dictionary = new("Dictionary", Lookups: 100, Hits: 49, Checksum: 2, Elapsed: TimeSpan.FromMilliseconds(8));

        RegistryLookupExample.RenderComparison(console, linear, dictionary);

        console.Output.Should().Contain("100×").And.Contain("disagreed");
    }

    [Fact]
    public void FormatTotal_UsesSecondsForLongRuns_AndMillisecondsForShortRuns()
    {
        RegistryLookupExample.FormatTotal(TimeSpan.FromSeconds(2)).Should().Contain("s").And.NotContain("ms");
        RegistryLookupExample.FormatTotal(TimeSpan.FromMilliseconds(5)).Should().Contain("ms");
    }
}
