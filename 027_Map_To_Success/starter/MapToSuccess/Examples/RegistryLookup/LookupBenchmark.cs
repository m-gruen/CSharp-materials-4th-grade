using System.Diagnostics;

namespace MapToSuccess.Examples.RegistryLookup;

/// <summary>
/// Runs a batch of lookups against a registry and times them with a <c>Stopwatch</c> (you will
/// add <c>using System.Diagnostics;</c>). Keep the measurement simple — the rendering layer
/// handles warm-up runs before the timed one.
/// </summary>
public static class LookupBenchmark
{
    /// <summary>
    /// Looks up every id in <paramref name="queryIds"/> against <paramref name="registry"/>,
    /// counting hits and summing the found ids, and reports how long it took.
    /// </summary>
    public static BenchmarkResult Measure(string strategy, IPersonRegistry registry, IReadOnlyList<int> queryIds)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(strategy);
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(queryIds);

        var sw = Stopwatch.StartNew();
        int hits = 0;
        long checksum = 0;

        foreach (int id in queryIds)
        {
            var person = registry.FindById(id);
            if (person is not null)
            {
                hits++;
                checksum += person.Id;
            }
        }

        sw.Stop();
        return new BenchmarkResult(strategy, queryIds.Count, hits, checksum, sw.Elapsed);
    }
}
