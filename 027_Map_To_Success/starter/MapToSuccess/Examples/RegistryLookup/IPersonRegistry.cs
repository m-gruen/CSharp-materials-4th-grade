namespace MapToSuccess.Examples.RegistryLookup;

/// <summary>
/// A registry of people that can be searched by id. Two implementations exist with
/// identical behaviour but very different speed: one scans a list, one uses a map.
/// </summary>
public interface IPersonRegistry
{
    /// <summary>How many people the registry holds.</summary>
    int Count { get; }

    /// <summary>Returns the person with this id, or <c>null</c> when nobody matches.</summary>
    Person? FindById(int id);
}
