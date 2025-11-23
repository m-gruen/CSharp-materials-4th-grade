namespace TankMuseum.Core.Exhibits;

public class Exhibit
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int ServiceStartYear { get; set; }
    public int? ServiceEndYear { get; set; }
    public LocalDate AddedToMuseumCollectionAt { get; set; }
    public int UnitsProduced { get; set; }
    public required string Country { get; set; }
    public required string Armor { get; set; }
    public required string Armament { get; set; }
    public int Crew { get; set; }
    public string? ImageUrl { get; set; }
}
