namespace MapToSuccess.Examples.RegistryLookup;

using Spectre.Console;

/// <summary>
/// The headline example: why a dictionary matters. We build a big registry of
/// people and look them up by id, first by scanning a <see cref="List{T}"/> and then
/// through a <see cref="Dictionary{TKey,TValue}"/>. Both return identical results,
/// but the list takes seconds while the dictionary answers in well under a
/// millisecond — and the gap only widens as the data grows.
/// <para>
/// Population and lookup counts are constructor parameters so the demo can be tuned
/// (or run small in tests). The defaults are sized to make the list take ~10 seconds.
/// </para>
/// </summary>
public sealed class RegistryLookupExample(
    int populationSize = 10_000_000,
    int lookupCount = 400,
    int seed = 20260531) : IExample
{
    public string Title => "Fast lookup: list scan vs map";

    public string Description => "Find people by id in a huge dataset — O(n) list scan versus O(1) dictionary.";

    public void Run(IAnsiConsole console)
    {
        console.MarkupLine(
            $"Registry of [yellow]{populationSize:N0}[/] people; running [yellow]{lookupCount:N0}[/] lookups "
            + "(half hits, half misses).");

        IReadOnlyList<Person> people = [];
        console.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("green"))
            .Start(
                $"Generating {populationSize:N0} sample people with Bogus… (one-time setup; real data would already exist)",
                _ => people = PersonGenerator.Generate(populationSize, seed));

        IReadOnlyList<int> queries = RegistryWorkload.BuildQueryIds(lookupCount, populationSize);
        IPersonRegistry linear = new LinearRegistry(people);
        IPersonRegistry dictionary = new DictionaryRegistry(people);

        // A couple of throwaway lookups warm up the JIT and CPU caches so the first
        // timed run is not unfairly slow.
        IReadOnlyList<int> warmup = RegistryWorkload.BuildQueryIds(2, populationSize, seed: 1);
        LookupBenchmark.Measure("warmup", linear, warmup);
        LookupBenchmark.Measure("warmup", dictionary, warmup);

        BenchmarkResult linearResult = LookupBenchmark.Measure("List scan (FirstOrDefault)", linear, queries);
        BenchmarkResult dictionaryResult = LookupBenchmark.Measure("Dictionary lookup (O(1))", dictionary, queries);

        RenderComparison(console, linearResult, dictionaryResult);
    }

    internal static void RenderComparison(IAnsiConsole console, BenchmarkResult linear, BenchmarkResult dictionary)
    {
        Table table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold]Same lookups, two data structures[/]")
            .AddColumn("strategy")
            .AddColumn("lookups", column => column.RightAligned())
            .AddColumn("hits", column => column.RightAligned())
            .AddColumn("total time", column => column.RightAligned())
            .AddColumn("per lookup", column => column.RightAligned());

        table.AddRow(
            $"[red]{Markup.Escape(linear.Strategy)}[/]",
            linear.Lookups.ToString("N0"),
            linear.Hits.ToString("N0"),
            FormatTotal(linear.Elapsed),
            FormatPerLookup(linear.PerLookup));
        table.AddRow(
            $"[green]{Markup.Escape(dictionary.Strategy)}[/]",
            dictionary.Lookups.ToString("N0"),
            dictionary.Hits.ToString("N0"),
            FormatTotal(dictionary.Elapsed),
            FormatPerLookup(dictionary.PerLookup));

        console.Write(table);

        bool identical = linear.Hits == dictionary.Hits && linear.Checksum == dictionary.Checksum;
        console.MarkupLine(identical
            ? "[green]✓ both strategies returned exactly the same people.[/]"
            : "[red]✗ the strategies disagreed — something is wrong![/]");

        double linearMs = linear.Elapsed.TotalMilliseconds;
        double dictionaryMs = dictionary.Elapsed.TotalMilliseconds;
        string factor = dictionaryMs <= 0
            ? "thousands of times"
            : $"{linearMs / dictionaryMs:N0}×";
        console.MarkupLine(
            $"[bold]The dictionary was about [green]{factor}[/] faster.[/] "
            + "Doubling the data would roughly double the list's time, but barely touch the dictionary.");
    }

    internal static string FormatTotal(TimeSpan elapsed) =>
        elapsed.TotalSeconds >= 1
            ? $"{elapsed.TotalSeconds:N2} s"
            : $"{elapsed.TotalMilliseconds:N3} ms";

    private static string FormatPerLookup(TimeSpan perLookup) =>
        $"{perLookup.TotalMicroseconds:N2} µs";
}
