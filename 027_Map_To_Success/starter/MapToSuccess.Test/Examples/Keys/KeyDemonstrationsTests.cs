namespace MapToSuccess.Test.Examples.Keys;

using global::MapToSuccess.Examples.Keys;
using NodaTime;

public sealed class KeyDemonstrationsTests
{
    [Fact]
    public void ValueKey_IsFoundWithAFreshInstance()
    {
        KeyDemonstrations.ValueKeyFindsFreshInstance().Should().BeTrue();
    }

    [Fact]
    public void ReferenceKey_FindsSameInstance_ButMissesFreshInstance()
    {
        KeyDemonstrations.ReferenceKeyFindsSameInstance().Should().BeTrue();
        KeyDemonstrations.ReferenceKeyMissesFreshInstance().Should().BeTrue();
    }

    [Fact]
    public void MutatingAStoredKey_StrandsTheEntry()
    {
        KeyDemonstrations.MutatedKeyEntryIsLost().Should().BeTrue();
        KeyDemonstrations.MutatedEntryIsLostToOriginalValueToo().Should().BeTrue();
    }

    [Fact]
    public void Comparer_DecidesCaseSensitivity()
    {
        KeyDemonstrations.OrdinalIgnoreCaseFinds().Should().BeTrue();
        KeyDemonstrations.DefaultComparerMisses().Should().BeTrue();
    }

    [Fact]
    public void LocalDateKey_IsFoundWithAFreshInstance()
    {
        KeyDemonstrations.LocalDateFindsFreshInstance().Should().BeTrue();
    }

    [Fact]
    public void BookingsByDate_IsKeyedByLocalDate()
    {
        Dictionary<LocalDate, int> bookings = KeyDemonstrations.BookingsByDate();

        bookings[new LocalDate(2026, 5, 31)].Should().Be(3);
        bookings[new LocalDate(2026, 6, 1)].Should().Be(5);
    }
}
