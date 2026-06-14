namespace MapToSuccess.Examples.MiniDictionary;

using Spectre.Console;

/// <summary>
/// Demonstrates the internals of a hash map: how a key becomes a bucket index,
/// what a collision looks like, and why the table grows (resizes).
/// Integer keys are used on purpose — an <see cref="int"/>'s hash code is the
/// number itself, so "hash % buckets" is predictable and easy to follow
/// (string hash codes are randomised per process and would change every run).
/// </summary>
public sealed class MiniDictionaryExample : IExample
{
    public string Title => "Build your own map (MiniDictionary)";

    public string Description => "How a hash map really works: hashing, buckets and collisions.";

    public void Run(IAnsiConsole console)
    {
        ShowHashingAndCollisions(console);
        console.WriteLine();
        ShowResize(console);
    }

    private static void ShowHashingAndCollisions(IAnsiConsole console)
    {
        // 4 buckets, three keys. With 4 buckets: 3%4=3, 5%4=1, 9%4=1.
        // So 5 and 9 collide in bucket 1, while 3 sits alone in bucket 3.
        MiniDictionary<int, string> map = new(initialBucketCount: 4);
        (int Id, string Name)[] people =
        [
            (3, "Carol"),
            (5, "Eve"),
            (9, "Ivan"),
        ];

        foreach ((int id, string name) in people)
        {
            map.Add(id, name);
        }

        console.MarkupLine("[bold]Step 1 — a key becomes a bucket index:[/] [grey]bucket = (hash & 0x7FFFFFFF) %% bucketCount[/]");
        Table hashing = new Table()
            .Border(TableBorder.Minimal)
            .AddColumn("key")
            .AddColumn("hashCode")
            .AddColumn($"% {map.BucketCount}")
            .AddColumn("bucket");
        foreach ((int id, _) in people)
        {
            hashing.AddRow(id.ToString(), id.GetHashCode().ToString(), $"{id} %% {map.BucketCount}", map.GetBucketIndex(id).ToString());
        }

        console.Write(hashing);

        console.MarkupLine("[bold]Step 2 — the buckets:[/] keys [yellow]5[/] and [yellow]9[/] collide, so they share a chain.");
        console.Write(RenderBuckets(map));

        console.MarkupLine(
            "[bold]Step 3 — a lookup[/] for key [yellow]9[/]: go straight to bucket "
            + $"[green]{map.GetBucketIndex(9)}[/] and scan only its short chain — never the whole map.");
    }

    private static void ShowResize(IAnsiConsole console)
    {
        console.MarkupLine("[bold]Step 4 — growing:[/] chains must stay short, so the table doubles once it passes 75%% full.");

        MiniDictionary<int, string> map = new(initialBucketCount: 4);
        Table growth = new Table()
            .Border(TableBorder.Minimal)
            .AddColumn("after adding key")
            .AddColumn("count")
            .AddColumn("buckets")
            .AddColumn("load factor");

        for (int id = 1; id <= 8; id++)
        {
            int bucketsBefore = map.BucketCount;
            map.Add(id, $"item-{id}");
            string bucketsCell = map.BucketCount == bucketsBefore
                ? map.BucketCount.ToString()
                : $"[green]{bucketsBefore} → {map.BucketCount}[/]";
            growth.AddRow(id.ToString(), map.Count.ToString(), bucketsCell, $"{map.LoadFactor:0.00}");
        }

        console.Write(growth);
    }

    private static Table RenderBuckets<TKey, TValue>(MiniDictionary<TKey, TValue> map)
        where TKey : notnull
    {
        Table table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("bucket")
            .AddColumn("chain (key → value)");

        for (int bucket = 0; bucket < map.BucketCount; bucket++)
        {
            IReadOnlyList<KeyValuePair<TKey, TValue>> entries = map.GetBucket(bucket);
            string chain = entries.Count == 0
                ? "[grey](empty)[/]"
                : string.Join(
                    " [grey]→[/] ",
                    entries.Select(e => $"[yellow]{Markup.Escape($"{e.Key}")}[/]=[blue]{Markup.Escape($"{e.Value}")}[/]"));
            table.AddRow(bucket.ToString(), chain);
        }

        return table;
    }
}
