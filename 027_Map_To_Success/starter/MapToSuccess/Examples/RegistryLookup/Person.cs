namespace MapToSuccess.Examples.RegistryLookup;

/// <summary>A person in the registry, looked up by their unique <see cref="Id"/>.</summary>
public sealed record Person(int Id, string FirstName, string LastName, string City);
