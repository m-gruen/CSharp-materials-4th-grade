namespace MapToSuccess.Examples.Keys;

/// <summary>
/// A good custom dictionary key: immutable, with <em>value</em> equality written out by hand.
/// Implementing <see cref="IEquatable{T}"/> defines when two keys are equal, and
/// <see cref="GetHashCode"/> decides which bucket the key lands in. The dictionary calls both
/// — that is exactly where a key's hash code "comes from".
/// </summary>
public sealed class Coordinate(int latitude, int longitude) : IEquatable<Coordinate>
{
    public int Latitude => latitude;

    public int Longitude => longitude;

    public bool Equals(Coordinate? other)
    {
        // TODO: two coordinates are equal when other is not null AND both Latitude and
        //       Longitude match. (Return false when other is null.)
        throw new NotImplementedException();
    }

    public override bool Equals(object? obj) => Equals(obj as Coordinate);

    public override int GetHashCode()
    {
        // TODO: equal coordinates MUST return the same hash code, or the dictionary would look
        //       in the wrong bucket and never find the entry. Combine the fields:
        //       HashCode.Combine(Latitude, Longitude)
        throw new NotImplementedException();
    }

    public override string ToString() => $"({Latitude}, {Longitude})";
}
