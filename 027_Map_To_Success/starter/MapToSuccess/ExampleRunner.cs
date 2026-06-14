namespace MapToSuccess;

using Spectre.Console;

/// <summary>
/// Drives the interactive menu: lists the available examples, reads a numeric
/// selection and runs the chosen example. Kept free of <see cref="System.Console"/>
/// so it can be tested end-to-end with a Spectre <c>TestConsole</c>.
/// </summary>
public sealed class ExampleRunner(IReadOnlyList<IExample> examples)
{
    /// <summary>The examples offered by this runner, in menu order.</summary>
    public IReadOnlyList<IExample> Examples => examples;

    /// <summary>
    /// Shows the menu in a loop until the user selects <c>0</c> (quit).
    /// </summary>
    public void Run(IAnsiConsole console)
    {
        while (RunStep(console))
        {
            // Keep looping until the user chooses to quit.
        }
    }

    /// <summary>
    /// Renders the menu once, reads a selection and runs it.
    /// Returns <c>false</c> when the user asked to quit.
    /// </summary>
    public bool RunStep(IAnsiConsole console)
    {
        RenderMenu(console);

        int choice = console.Prompt(
            new TextPrompt<int>("Select an example ([green]0[/] to quit):")
                .Validate(value => value >= 0 && value <= examples.Count
                    ? ValidationResult.Success()
                    : ValidationResult.Error($"[red]Enter a number between 0 and {examples.Count}[/]")));

        if (choice == 0)
        {
            console.MarkupLine("[grey]Goodbye![/]");
            return false;
        }

        IExample example = examples[choice - 1];
        console.Write(new Rule($"[yellow]{Markup.Escape(example.Title)}[/]").LeftJustified());
        example.Run(console);
        console.WriteLine();
        return true;
    }

    private void RenderMenu(IAnsiConsole console)
    {
        Table table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold underline]Map To Success — Dictionary examples[/]")
            .AddColumn("[grey]#[/]")
            .AddColumn("Example")
            .AddColumn("What it teaches");

        for (int index = 0; index < examples.Count; index++)
        {
            IExample example = examples[index];
            table.AddRow(
                (index + 1).ToString(),
                $"[bold]{Markup.Escape(example.Title)}[/]",
                Markup.Escape(example.Description));
        }

        table.AddRow("[grey]0[/]", "[grey]Quit[/]", string.Empty);
        console.Write(table);
    }
}
