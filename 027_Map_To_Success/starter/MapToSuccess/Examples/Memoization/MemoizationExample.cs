namespace MapToSuccess.Examples.Memoization;

using System.Diagnostics;
using Spectre.Console;

/// <summary>
/// "Compute once, reuse" with a dictionary. The same set of repeated requests is
/// answered twice: first by calling an expensive function every time, then through a
/// <see cref="Memoizer{TKey,TResult}"/>. The cached run does far fewer computations
/// and is noticeably faster, while returning exactly the same answers.
/// </summary>
public sealed class MemoizationExample : IExample
{
    // The queries deliberately repeat: 24 of the 30 lookups ask for a key seen before.
    private static readonly int[] queries =
        [.. Enumerable.Range(0, 30).Select(i => 1 + (i % 6))];

    public string Title => "Caching results (memoization)";

    public string Description => "Use a map to compute an expensive function once per key and reuse the answer.";

    public void Run(IAnsiConsole console)
    {
        console.MarkupLine(
            $"Answering [yellow]{queries.Length}[/] requests for [yellow]{queries.Distinct().Count()}[/] distinct keys, "
            + "where each fresh computation is expensive.");

        int naiveComputations = 0;
        Stopwatch naiveTimer = Stopwatch.StartNew();
        long[] naiveResults =
        [
            .. queries.Select(key =>
            {
                naiveComputations++;
                return Expensive(key);
            }),
        ];
        naiveTimer.Stop();

        int memoComputations = 0;
        Memoizer<int, long> memoizer = new(key =>
        {
            memoComputations++;
            return Expensive(key);
        });
        Stopwatch memoTimer = Stopwatch.StartNew();
        long[] memoResults = [.. queries.Select(memoizer.Get)];
        memoTimer.Stop();

        Table table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold]Same answers, far less work[/]")
            .AddColumn("approach")
            .AddColumn("requests", column => column.RightAligned())
            .AddColumn("expensive calls", column => column.RightAligned())
            .AddColumn("time", column => column.RightAligned());
        table.AddRow(
            "[red]No cache[/]",
            queries.Length.ToString(),
            naiveComputations.ToString(),
            $"{naiveTimer.Elapsed.TotalMilliseconds:N1} ms");
        table.AddRow(
            "[green]Memoized[/]",
            queries.Length.ToString(),
            memoComputations.ToString(),
            $"{memoTimer.Elapsed.TotalMilliseconds:N1} ms");
        console.Write(table);

        console.MarkupLine(
            $"The cache served [green]{memoizer.Hits}[/] hits and ran the expensive function only "
            + $"[green]{memoizer.Misses}[/] times (one per distinct key).");
        console.MarkupLine(DescribeAgreement(naiveResults.SequenceEqual(memoResults)));
        console.MarkupLine(DescribeSpeedup(naiveTimer.Elapsed, memoTimer.Elapsed));
    }

    // Stands in for a genuinely costly operation (heavy math, a slow API call, …).
    private static long Expensive(int n)
    {
        long sum = 0;
        for (int i = 0; i < 2_000_000; i++)
        {
            sum += i % (n + 1);
        }

        return sum + ((long)n * n);
    }

    internal static string DescribeAgreement(bool identical) => identical
        ? "[green]✓ cached and uncached runs produced identical results.[/]"
        : "[red]✗ the cached run produced different results![/]";

    internal static string DescribeSpeedup(TimeSpan naive, TimeSpan memoized)
    {
        string factor = memoized.TotalMilliseconds <= 0
            ? "many times"
            : $"{(naive.TotalMilliseconds / memoized.TotalMilliseconds).ToString("N1", System.Globalization.CultureInfo.InvariantCulture)}×";
        return $"[bold]Caching made it about [green]{factor}[/] faster.[/]";
    }
}
