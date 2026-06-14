namespace MapToSuccess;

using MapToSuccess.Examples.Joins;
using MapToSuccess.Examples.Keys;
using MapToSuccess.Examples.Memoization;
using MapToSuccess.Examples.MiniDictionary;
using MapToSuccess.Examples.RegistryLookup;
using MapToSuccess.Examples.WordFrequency;

/// <summary>
/// The single place that lists every example, in the order students should meet
/// them. Kept tiny and free of side effects so the set can be asserted in tests.
/// </summary>
public static class ExampleCatalog
{
    /// <summary>Creates a fresh instance of every example, in teaching order.</summary>
    public static IReadOnlyList<IExample> CreateAll() =>
    [
        new MiniDictionaryExample(),
        new WordFrequencyExample(),
        new RegistryLookupExample(),
        new JoinExample(),
        new KeysExample(),
        new MemoizationExample(),
    ];
}
