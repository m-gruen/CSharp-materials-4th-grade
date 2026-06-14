namespace MapToSuccess.Examples.RegistryLookup;

using Bogus;

/// <summary>
/// Generates realistic-looking test people with Bogus. A fixed seed makes the
/// output deterministic, which keeps tests stable. Ids are sequential and unique
/// (0 .. count-1) so they double as valid lookup keys.
/// </summary>
public static class PersonGenerator
{
    /// <summary>Generates <paramref name="count"/> people with stable, unique ids.</summary>
    public static IReadOnlyList<Person> Generate(int count, int seed = 20260531)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        Faker<Person> faker = new Faker<Person>()
            .CustomInstantiator(f => new Person(
                Id: f.IndexFaker,
                FirstName: f.Name.FirstName(),
                LastName: f.Name.LastName(),
                City: f.Address.City()))
            .UseSeed(seed);

        return faker.Generate(count);
    }
}
