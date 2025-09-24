namespace UnitTests.Test;

public class AdderTest
{
    private readonly Adder _sut = new();

    [Fact]
    public void Add_Success()
    {
        // Arrange
        const int A = 17;
        const int B = 15;

        // Act
        int result = _sut.Add(A, B);

        // Assert
        result.Should().Be(A + B);
        // Assert.Equal(A + B, result); -- Other 
    }

    [Theory]
    /*
    [InlineData(1, 2, 3)]
    [InlineData(17, 15, 32)]
    [InlineData(-5, 5, 0)]
    [InlineData(-10, -10, -20)]
    */
    [MemberData(nameof(AddCorrectResultsData))]
    public void Add_ConnectResult(int a, int b, int result)
    {
        // Act
        int actual = _sut.Add(a, b);

        // Assert
        actual.Should().Be(result);
    }

    public static TheoryData<int, int, int> AddCorrectResultsData() =>
        new()
        {
            { 1, 5, 6 },
            { 17, 17, 34 },
            { -15, 15, 0 },
            { -15, -10, -25 }
        };
}
