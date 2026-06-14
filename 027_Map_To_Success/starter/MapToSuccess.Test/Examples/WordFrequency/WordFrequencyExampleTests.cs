namespace MapToSuccess.Test.Examples.WordFrequency;

using global::MapToSuccess.Examples.WordFrequency;
using Spectre.Console.Testing;

public sealed class WordFrequencyExampleTests
{
    [Fact]
    public void Metadata_IsPresent()
    {
        WordFrequencyExample example = new();

        example.Title.Should().NotBeNullOrWhiteSpace();
        example.Description.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Run_ShowsTopWordsAndTheFourApproaches()
    {
        WordFrequencyExample example = new();
        TestConsole console = new();

        example.Run(console);

        string output = console.Output;
        output.Should().Contain("the", "the most frequent word should appear in the chart");
        output.Should().Contain("GroupBy + ToDictionary");
        output.Should().Contain("TryAdd");
        output.Should().Contain("fox");
    }
}
