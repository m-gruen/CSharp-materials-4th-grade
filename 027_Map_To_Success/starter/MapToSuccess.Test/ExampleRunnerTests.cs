namespace MapToSuccess.Test;

using global::MapToSuccess;
using Spectre.Console;
using Spectre.Console.Testing;

public sealed class ExampleRunnerTests
{
    private sealed class FakeExample(string title) : IExample
    {
        public int Runs { get; private set; }

        public string Title => title;

        public string Description => $"description of {title}";

        public void Run(IAnsiConsole console)
        {
            Runs++;
            console.WriteLine($"ran {title}");
        }
    }

    private static TestConsole NewConsole() => new TestConsole().Interactive();

    [Fact]
    public void Examples_ExposesTheProvidedList()
    {
        FakeExample first = new("Alpha");
        ExampleRunner runner = new([first]);

        runner.Examples.Should().ContainSingle().Which.Should().BeSameAs(first);
    }

    [Fact]
    public void RunStep_RendersMenuWithEveryExample()
    {
        FakeExample first = new("Alpha");
        FakeExample second = new("Beta");
        ExampleRunner runner = new([first, second]);
        TestConsole console = NewConsole();
        console.Input.PushTextWithEnter("0");

        runner.RunStep(console);

        console.Output.Should().Contain("Alpha").And.Contain("Beta").And.Contain("Quit");
    }

    [Fact]
    public void RunStep_QuitChoice_ReturnsFalseAndRunsNothing()
    {
        FakeExample example = new("Alpha");
        ExampleRunner runner = new([example]);
        TestConsole console = NewConsole();
        console.Input.PushTextWithEnter("0");

        bool keepGoing = runner.RunStep(console);

        keepGoing.Should().BeFalse();
        example.Runs.Should().Be(0);
        console.Output.Should().Contain("Goodbye");
    }

    [Fact]
    public void RunStep_RunsSelectedExample_AndReturnsTrue()
    {
        FakeExample first = new("Alpha");
        FakeExample second = new("Beta");
        ExampleRunner runner = new([first, second]);
        TestConsole console = NewConsole();
        console.Input.PushTextWithEnter("2");

        bool keepGoing = runner.RunStep(console);

        keepGoing.Should().BeTrue();
        second.Runs.Should().Be(1);
        first.Runs.Should().Be(0);
        console.Output.Should().Contain("ran Beta");
    }

    [Fact]
    public void RunStep_OutOfRangeSelection_IsRejectedThenAccepted()
    {
        FakeExample example = new("Alpha");
        ExampleRunner runner = new([example]);
        TestConsole console = NewConsole();
        console.Input.PushTextWithEnter("-1"); // invalid: below range
        console.Input.PushTextWithEnter("9");  // invalid: above range (only 1 example)
        console.Input.PushTextWithEnter("0");  // then quit

        bool keepGoing = runner.RunStep(console);

        keepGoing.Should().BeFalse();
        console.Output.Should().Contain("between 0 and 1");
    }

    [Fact]
    public void Run_LoopsUntilQuitChosen()
    {
        FakeExample first = new("Alpha");
        FakeExample second = new("Beta");
        ExampleRunner runner = new([first, second]);
        TestConsole console = NewConsole();
        console.Input.PushTextWithEnter("1");
        console.Input.PushTextWithEnter("2");
        console.Input.PushTextWithEnter("0");

        runner.Run(console);

        first.Runs.Should().Be(1);
        second.Runs.Should().Be(1);
    }
}
