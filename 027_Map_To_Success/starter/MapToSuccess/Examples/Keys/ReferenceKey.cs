namespace MapToSuccess.Examples.Keys;

/// <summary>
/// A <em>bad</em> dictionary key: it carries a value but does not override equality,
/// so the dictionary falls back to reference equality. Two instances with the same
/// <see cref="Value"/> are treated as different keys — a classic beginner trap.
/// </summary>
public sealed class ReferenceKey(int value)
{
    public int Value => value;

    public override string ToString() => $"ref:{Value}";
}
