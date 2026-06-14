namespace MapToSuccess.Test.Examples.RegistryLookup;

using global::MapToSuccess.Examples.RegistryLookup;

public sealed class RegistryDataTests
{
    [Fact]
    public void Generate_ProducesRequestedCount_WithUniqueSequentialIds()
    {
        IReadOnlyList<Person> people = PersonGenerator.Generate(50);

        people.Should().HaveCount(50);
        people.Select(p => p.Id).Should().Equal(Enumerable.Range(0, 50));
        people.Should().OnlyContain(p => !string.IsNullOrWhiteSpace(p.FirstName));
    }

    [Fact]
    public void Generate_IsDeterministicForAGivenSeed()
    {
        IReadOnlyList<Person> first = PersonGenerator.Generate(20, seed: 123);
        IReadOnlyList<Person> second = PersonGenerator.Generate(20, seed: 123);

        second.Should().Equal(first);
    }

    [Fact]
    public void Generate_NegativeCount_Throws()
    {
        Action act = () => PersonGenerator.Generate(-1);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void BothRegistries_ReportTheSameCount()
    {
        IReadOnlyList<Person> people = PersonGenerator.Generate(30);

        new LinearRegistry(people).Count.Should().Be(30);
        new DictionaryRegistry(people).Count.Should().Be(30);
    }

    [Fact]
    public void BothRegistries_FindTheSamePersonForAHit()
    {
        IReadOnlyList<Person> people = PersonGenerator.Generate(30);
        Person expected = people[17];

        new LinearRegistry(people).FindById(17).Should().Be(expected);
        new DictionaryRegistry(people).FindById(17).Should().Be(expected);
    }

    [Fact]
    public void BothRegistries_ReturnNullForAMiss()
    {
        IReadOnlyList<Person> people = PersonGenerator.Generate(30);

        new LinearRegistry(people).FindById(9999).Should().BeNull();
        new DictionaryRegistry(people).FindById(9999).Should().BeNull();
    }

    [Fact]
    public void DictionaryRegistry_NullPeople_Throws()
    {
        Action act = () => _ = new DictionaryRegistry(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
