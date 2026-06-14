namespace MapToSuccess.Test.Examples.Joins;

using global::MapToSuccess.Examples.Joins;
using Spectre.Console.Testing;

public sealed class JoinExampleTests
{
    [Fact]
    public void Metadata_IsPresent()
    {
        JoinExample example = new();

        example.Title.Should().NotBeNullOrWhiteSpace();
        example.Description.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Run_ShowsEnrichedJoin_UnknownCustomer_AndGrouping()
    {
        JoinExample example = new();
        TestConsole console = new();
        console.Profile.Width = 240;

        example.Run(console);

        string output = console.Output;
        output.Should().Contain("Alice").And.Contain("Bob").And.Contain("Carol");
        output.Should().Contain("(unknown)", "order 106 references a customer that does not exist");
        output.Should().Contain("same rows", "both join strategies must agree");
        output.Should().Contain("orders × customers");
    }

    [Fact]
    public void RenderAgreement_FlagsDisagreement_WhenJoinsDiffer()
    {
        TestConsole console = new();
        console.Profile.Width = 240;

        JoinExample.RenderAgreement(console, identical: false);

        console.Output.Should().Contain("disagreed");
    }
}
