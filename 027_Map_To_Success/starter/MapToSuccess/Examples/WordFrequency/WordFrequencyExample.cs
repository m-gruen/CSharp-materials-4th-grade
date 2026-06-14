namespace MapToSuccess.Examples.WordFrequency;

using Spectre.Console;

/// <summary>
/// "What is a dictionary for?" — accumulating a value per key. Counts the words in
/// a sample text, shows the most frequent ones as a bar chart, and points out that
/// several different APIs all produce the same counts.
/// </summary>
public sealed class WordFrequencyExample : IExample
{
    private const string SampleText =
        "The quick brown fox jumps over the lazy dog. " +
        "The dog barks, the fox runs, and the quick fox jumps again.";

    public string Title => "Counting words (what a map is for)";

    public string Description => "Accumulate a value per key: indexer, TryGetValue, GetValueOrDefault, GroupBy.";

    public void Run(IAnsiConsole console)
    {
        IReadOnlyList<string> words = WordFrequencyCounter.Tokenize(SampleText);
        Dictionary<string, int> counts = WordFrequencyCounter.CountWithGetValueOrDefault(words);

        console.MarkupLine($"[grey]Sample text:[/] {Markup.Escape(SampleText)}");
        console.MarkupLine($"[grey]{words.Count} words, {counts.Count} distinct.[/]");
        console.WriteLine();

        IReadOnlyList<KeyValuePair<string, int>> top = WordFrequencyCounter.TopWords(counts, 5);
        BarChart chart = new BarChart()
            .Width(60)
            .Label("[bold]Top 5 words[/]")
            .CenterLabel();
        foreach ((string word, int count) in top)
        {
            chart.AddItem(word, count, Color.Aqua);
        }

        console.Write(chart);
        console.WriteLine();

        console.MarkupLine("[bold]The same counts, four ways[/] — pick whichever reads best:");
        Table table = new Table()
            .Border(TableBorder.Minimal)
            .AddColumn("approach")
            .AddColumn(@"count of ""the""");
        AddApproachRow(table, "ContainsKey + indexer", WordFrequencyCounter.CountWithContainsKey(words));
        AddApproachRow(table, "TryGetValue", WordFrequencyCounter.CountWithTryGetValue(words));
        AddApproachRow(table, "GetValueOrDefault", WordFrequencyCounter.CountWithGetValueOrDefault(words));
        AddApproachRow(table, "GroupBy + ToDictionary", WordFrequencyCounter.CountWithLinq(words));
        console.Write(table);

        Dictionary<string, int> firstSeen = WordFrequencyCounter.FirstSeenPositions(words);
        console.MarkupLine(
            $"[bold]TryAdd[/] keeps only the first sighting: [yellow]fox[/] first appeared at word "
            + $"[green]#{firstSeen["fox"]}[/] and later repeats were ignored.");
    }

    private static void AddApproachRow(Table table, string approach, IReadOnlyDictionary<string, int> counts) =>
        table.AddRow(Markup.Escape(approach), counts["the"].ToString());
}
