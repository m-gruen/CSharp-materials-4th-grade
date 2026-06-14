namespace MapToSuccess.Test.Examples.Keys;

using global::MapToSuccess.Examples.Keys;

public sealed class KeyTypesTests
{
    [Fact]
    public void Coordinate_EqualCoordinates_AreEqual_AndShareHashCode()
    {
        Coordinate a = new(48, 16);
        Coordinate b = new(48, 16);

        a.Equals(b).Should().BeTrue();
        a.Equals((object)b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
        a.ToString().Should().Be("(48, 16)");
    }

    [Fact]
    public void Coordinate_DiffersByLatitudeOrLongitude_AreNotEqual()
    {
        Coordinate a = new(48, 16);

        a.Equals(new Coordinate(49, 16)).Should().BeFalse(); // latitude differs
        a.Equals(new Coordinate(48, 17)).Should().BeFalse(); // longitude differs
    }

    [Fact]
    public void Coordinate_Equals_HandlesNullAndOtherTypes()
    {
        Coordinate a = new(48, 16);
        object equalBoxed = new Coordinate(48, 16);
        object differentType = "not a coordinate";

        a.Equals(equalBoxed).Should().BeTrue();      // Equals(object?) with a real match
        a.Equals(differentType).Should().BeFalse();  // Equals(object?) with a wrong type
        a.Equals((Coordinate?)null).Should().BeFalse();
    }

    [Fact]
    public void ReferenceKey_ExposesValue_AndToString()
    {
        ReferenceKey key = new(7);

        key.Value.Should().Be(7);
        key.ToString().Should().Be("ref:7");
    }

    [Fact]
    public void MutableKey_EqualityFollowsId()
    {
        MutableKey a = new() { Id = 5 };
        MutableKey b = new() { Id = 5 };

        a.Equals(b).Should().BeTrue();
        a.Equals((object)b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
        a.Equals(new MutableKey { Id = 6 }).Should().BeFalse();
        a.ToString().Should().Be("mutable:5");
    }

    [Fact]
    public void MutableKey_Equals_HandlesNullAndOtherTypes()
    {
        MutableKey a = new() { Id = 5 };
        object differentType = "not a key";

        a.Equals(differentType).Should().BeFalse(); // Equals(object?) with a wrong type
        a.Equals((MutableKey?)null).Should().BeFalse();
    }
}
