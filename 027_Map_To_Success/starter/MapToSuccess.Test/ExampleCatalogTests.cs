namespace MapToSuccess.Test;

using global::MapToSuccess;

public sealed class ExampleCatalogTests
{
    [Fact]
    public void CreateAll_ReturnsExamplesWithUsableMetadata()
    {
        IReadOnlyList<IExample> examples = ExampleCatalog.CreateAll();

        examples.Should().NotBeEmpty();
        examples.Should().OnlyContain(e => !string.IsNullOrWhiteSpace(e.Title));
        examples.Should().OnlyContain(e => !string.IsNullOrWhiteSpace(e.Description));
    }

    [Fact]
    public void CreateAll_HasNoDuplicateTitles()
    {
        IReadOnlyList<IExample> examples = ExampleCatalog.CreateAll();

        examples.Select(e => e.Title).Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void CreateAll_ReturnsFreshInstancesEachCall()
    {
        IReadOnlyList<IExample> first = ExampleCatalog.CreateAll();
        IReadOnlyList<IExample> second = ExampleCatalog.CreateAll();

        first.Should().NotBeSameAs(second);
        first[0].Should().NotBeSameAs(second[0]);
    }
}
