using System.Diagnostics.CodeAnalysis;
using System.Text;
using MapToSuccess;
using Spectre.Console;

// Composition root only: wire up the console and the example catalog, then run.
// All real behaviour lives in testable classes; this file is intentionally tiny
// and excluded from coverage because it cannot be exercised without a real TTY.
[ExcludeFromCodeCoverage]
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        ExampleRunner runner = new(ExampleCatalog.CreateAll());
        runner.Run(AnsiConsole.Console);
    }
}
