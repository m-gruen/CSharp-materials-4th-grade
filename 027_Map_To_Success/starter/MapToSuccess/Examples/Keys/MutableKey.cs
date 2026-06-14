namespace MapToSuccess.Examples.Keys;

/// <summary>
/// A key that <em>does</em> implement value equality but is mutable. This is dangerous: if you
/// change <see cref="Id"/> after using the key in a dictionary, its hash code changes and the
/// entry becomes stranded in the wrong bucket — unreachable by any key. The lesson: dictionary
/// keys should be immutable. (You still implement equality here so the demonstration can show
/// the problem.)
/// </summary>
public sealed class MutableKey : IEquatable<MutableKey>
{
    public int Id { get; set; }

    public bool Equals(MutableKey? other)
    {
        // TODO: equal when other is not null and Id matches.
        throw new NotImplementedException();
    }

    public override bool Equals(object? obj) => Equals(obj as MutableKey);

    public override int GetHashCode()
    {
        // TODO: base the hash code on Id (e.g. Id.GetHashCode()).
        throw new NotImplementedException();
    }

    public override string ToString() => $"mutable:{Id}";
}
