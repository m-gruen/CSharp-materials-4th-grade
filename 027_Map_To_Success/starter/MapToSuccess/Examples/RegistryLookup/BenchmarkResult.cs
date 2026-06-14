namespace MapToSuccess.Examples.RegistryLookup;

/// <summary>
/// The outcome of running a batch of lookups against one registry.
/// <para>
/// <see cref="Checksum"/> is the sum of the ids actually found. It serves two
/// purposes: it stops the compiler/JIT from optimising the "useless" lookups away,
/// and it lets tests prove that two strategies returned exactly the same results.
/// </para>
/// </summary>
public sealed record BenchmarkResult(string Strategy, int Lookups, int Hits, long Checksum, TimeSpan Elapsed)
{
    /// <summary>Average time per single lookup.</summary>
    public TimeSpan PerLookup => Lookups == 0 ? TimeSpan.Zero : Elapsed / Lookups;
}
