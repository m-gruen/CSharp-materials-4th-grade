namespace MapToSuccess.Test.Examples.Keys;

using global::MapToSuccess.Examples.Keys;
using Spectre.Console.Testing;

public sealed class KeysExampleTests
{
    [Fact]
    public void Metadata_IsPresent()
    {
        KeysExample example = new();

        example.Title.Should().NotBeNullOrWhiteSpace();
        example.Description.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Run_ShowsAllKeyScenarios_AndTheBookingsTable()
    {
        KeysExample example = new();
        TestConsole console = new();
        console.Profile.Width = 240;

        example.Run(console);

        string output = console.Output;
        output.Should().Contain("GetHashCode");
        output.Should().Contain("Value key");
        output.Should().Contain("Mutating");
        output.Should().Contain("OrdinalIgnoreCase");
        output.Should().Contain("LocalDate");
        output.Should().Contain("2026-05-31", "the bookings table is keyed by LocalDate");
    }

    [Fact]
    public void Tick_RendersGreenForPass_AndRedForFail()
    {
        KeysExample.Tick(true).Should().Contain("green").And.Contain("✓");
        KeysExample.Tick(false).Should().Contain("red").And.Contain("✗");
    }
}
