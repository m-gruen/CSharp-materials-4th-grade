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

    public bool Equals(Coordinate? other) => other is not null && Latitude == other.Latitude && Longitude == other.Longitude;

    public override bool Equals(object? obj) => Equals(obj as Coordinate);

    public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);

    public override string ToString() => $"({Latitude}, {Longitude})";
}
