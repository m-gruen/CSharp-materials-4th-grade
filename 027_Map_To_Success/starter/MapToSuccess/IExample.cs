namespace MapToSuccess;

using Spectre.Console;

/// <summary>
/// A single, self-contained teaching example. Each example owns its own logic
/// classes and knows how to render itself to a Spectre <see cref="IAnsiConsole"/>.
/// Examples never call <see cref="System.Console"/> directly so that the logic
/// stays terminal-free and fully testable.
/// </summary>
public interface IExample
{
    /// <summary>Short title shown in the menu.</summary>
    string Title { get; }

    /// <summary>One-line description shown next to the title.</summary>
    string Description { get; }

    /// <summary>Runs the example and renders its output to <paramref name="console"/>.</summary>
    void Run(IAnsiConsole console);
}
