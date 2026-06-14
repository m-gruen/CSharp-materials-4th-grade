namespace MapToSuccess.Examples.Keys;

using NodaTime;
using Spectre.Console;

/// <summary>
/// What makes a good dictionary key. Walks through value keys vs reference keys, the
/// danger of mutable keys, choosing a comparer for case-insensitive string keys, and
/// using a NodaTime <see cref="LocalDate"/> as a key.
/// </summary>
public sealed class KeysExample : IExample
{
    public string Title => "Designing good keys (equality & hashing)";

    public string Description => "IEquatable + GetHashCode, comparers, the mutable-key bug, and dates as keys.";

    public void Run(IAnsiConsole console)
    {
        Coordinate vienna = new(48, 16);
        Coordinate viennaAgain = new(48, 16);
        console.MarkupLine(
            $"A key's bucket comes from its hash code: [yellow]Coordinate(48, 16).GetHashCode()[/] = "
            + $"[blue]{vienna.GetHashCode()}[/], and an identical coordinate hashes to "
            + $"[blue]{viennaAgain.GetHashCode()}[/] — equal keys [bold]must[/] share a hash.");
        console.WriteLine();

        Table table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold]Key behaviour[/]")
            .AddColumn("scenario")
            .AddColumn("ok?")
            .AddColumn("lesson");
        AddRow(table, "Value key (IEquatable + GetHashCode)",
            KeyDemonstrations.ValueKeyFindsFreshInstance(),
            "a fresh instance with the same values is found");
        AddRow(table, "Reference key (no overrides)",
            KeyDemonstrations.ReferenceKeyMissesFreshInstance(),
            "without equality, only the same instance matches — write IEquatable!");
        AddRow(table, "Mutating a stored key",
            KeyDemonstrations.MutatedKeyEntryIsLost(),
            "the entry is stranded in the wrong bucket — keep keys immutable");
        AddRow(table, "OrdinalIgnoreCase comparer",
            KeyDemonstrations.OrdinalIgnoreCaseFinds(),
            "\"vienna\" finds \"Vienna\"; the default comparer would not");
        AddRow(table, "NodaTime LocalDate key",
            KeyDemonstrations.LocalDateFindsFreshInstance(),
            "a value type with built-in equality works as a key for free");
        console.Write(table);

        console.WriteLine();
        console.MarkupLine("[bold]Bookings per day[/] (keyed by [grey]LocalDate[/]):");
        Table bookings = new Table()
            .Border(TableBorder.Minimal)
            .AddColumn("date")
            .AddColumn("seats", column => column.RightAligned());
        foreach ((LocalDate date, int seats) in KeyDemonstrations.BookingsByDate().OrderBy(pair => pair.Key))
        {
            bookings.AddRow(date.ToString("yyyy-MM-dd", null), seats.ToString());
        }

        console.Write(bookings);
    }

    private static void AddRow(Table table, string scenario, bool ok, string lesson) =>
        table.AddRow(Markup.Escape(scenario), Tick(ok), Markup.Escape(lesson));

    internal static string Tick(bool ok) => ok ? "[green]✓[/]" : "[red]✗[/]";
}
