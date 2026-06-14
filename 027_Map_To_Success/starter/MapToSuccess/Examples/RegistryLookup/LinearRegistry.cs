namespace MapToSuccess.Examples.RegistryLookup;

/// <summary>
/// The "obvious" approach a beginner writes: keep everyone in a list and scan it to find a
/// match. Each lookup is O(n) — on average it touches half the list, and a miss touches the
/// whole list. Fine for a handful of items, painful for millions.
/// </summary>
public sealed class LinearRegistry(IReadOnlyList<Person> people) : IPersonRegistry
{
    public int Count => people.Count;

    /// <summary>Returns the person with this id, or <c>null</c> when nobody matches.</summary>
    public Person? FindById(int id) => people.FirstOrDefault(person => person.Id == id);
}
