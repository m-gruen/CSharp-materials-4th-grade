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

        // TODO:
        //   - start a Stopwatch
        //   - for each id in queryIds, call registry.FindById(id); if it returns a person,
        //     count a hit and add person.Id to a long checksum
        //   - stop the stopwatch
        //   - return new BenchmarkResult(strategy, queryIds.Count, hits, checksum, stopwatch.Elapsed)
        // (The checksum lets tests prove two strategies returned the SAME results.)
        throw new NotImplementedException();
    }
}
