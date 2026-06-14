namespace MapToSuccess.Examples.RegistryLookup;

/// <summary>
/// The same registry backed by a <see cref="Dictionary{TKey,TValue}"/>, built once. Each
/// lookup hashes the id straight to its bucket, so it is effectively O(1) — independent of
/// how many people are stored.
/// </summary>
public sealed class DictionaryRegistry : IPersonRegistry
{
    private readonly Dictionary<int, Person> _byId = new();

    /// <summary>Builds the id → person index up front from the supplied people.</summary>
    public DictionaryRegistry(IReadOnlyList<Person> people)
    {
        ArgumentNullException.ThrowIfNull(people);

        // TODO: fill _byId with an id -> person index so that lookups are O(1), then delete the
        //       `throw` below. Hint: _byId = people.ToDictionary(person => person.Id);
        //       (a readonly field may be reassigned inside the constructor)
        throw new NotImplementedException();
    }

    public int Count => _byId.Count;

    /// <summary>Returns the person with this id, or <c>null</c> when nobody matches.</summary>
    public Person? FindById(int id)
    {
        // TODO: a single dictionary lookup. Hint: _byId.GetValueOrDefault(id)
        throw new NotImplementedException();
    }
}
