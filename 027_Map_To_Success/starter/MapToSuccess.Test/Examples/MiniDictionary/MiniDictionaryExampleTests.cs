namespace MapToSuccess.Test.Examples.MiniDictionary;

using global::MapToSuccess.Examples.MiniDictionary;
using Spectre.Console.Testing;

public sealed class MiniDictionaryExampleTests
{
    [Fact]
    public void Metadata_IsPresent()
    {
        MiniDictionaryExample example = new();

        example.Title.Should().NotBeNullOrWhiteSpace();
        example.Description.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Run_ShowsCollisionChainAndResize()
    {
        MiniDictionaryExample example = new();
        TestConsole console = new();

        example.Run(console);

        string output = console.Output;
        output.Should().Contain("Eve").And.Contain("Ivan", "the colliding keys share a bucket chain");
        output.Should().Contain("Carol");
        output.Should().Contain("bucket");
        output.Should().Contain("load factor");
        output.Should().Contain("→", "a resize is shown as oldCount → newCount");
    }
}
