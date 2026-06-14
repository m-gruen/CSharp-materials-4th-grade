namespace MapToSuccess.Test.Examples.Memoization;

using global::MapToSuccess.Examples.Memoization;
using Spectre.Console.Testing;

public sealed class MemoizationExampleTests
{
    [Fact]
    public void Metadata_IsPresent()
    {
        MemoizationExample example = new();

        example.Title.Should().NotBeNullOrWhiteSpace();
        example.Description.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Run_ShowsCacheStats_AndComparesBothRuns()
    {
        MemoizationExample example = new();
        TestConsole console = new();
        console.Profile.Width = 240;

        example.Run(console);

        string output = console.Output;
        output.Should().Contain("No cache");
        output.Should().Contain("Memoized");
        output.Should().Contain("hits");
        output.Should().Contain("identical results");
        output.Should().Contain("faster");
    }

    [Fact]
    public void DescribeAgreement_FlagsDisagreement()
    {
        MemoizationExample.DescribeAgreement(identical: true).Should().Contain("identical");
        MemoizationExample.DescribeAgreement(identical: false).Should().Contain("different");
    }

    [Fact]
    public void DescribeSpeedup_HandlesNumericAndInstantCases()
    {
        MemoizationExample.DescribeSpeedup(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10))
            .Should().Contain("10.0×");
        MemoizationExample.DescribeSpeedup(TimeSpan.FromMilliseconds(100), TimeSpan.Zero)
            .Should().Contain("many times");
    }
}
