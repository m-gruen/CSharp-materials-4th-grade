using OneOf;
using OneOf.Types;

namespace TankMuseum.Core.Exhibits;

public static class ExhibitEndpoint
{
    private const string ApiBasePath = "/api/exhibits";

    public static void MapExhibitEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiBasePath);

        group.MapGet("", async (IExhibitService service) =>
             {
                 IEnumerable<Exhibit> exhibits = await service.GetAllExhibitsAsync();

                 return Results.Ok(new ExhibitListResponse
                 {
                     Items = exhibits.Select(ExhibitInformationDto.FromExhibit).ToList()
                 });
             })
             .Produces<ExhibitListResponse>(StatusCodes.Status200OK);

        group.MapGet("{exhibitId:int}", async (int exhibitId, IExhibitService service) =>
             {
                 OneOf<Exhibit, NotFound> exhibitResult = await service.GetExhibitByIdAsync(exhibitId);

                 return exhibitResult.Match(exhibit => Results.Ok(ExhibitDetailsDto.FromExhibit(exhibit)),
                                            notFound => Results.NotFound());
             })
             .Produces<ExhibitDetailsDto>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status404NotFound);
        
        // Important: in a real application we'd have to check for authorization here

        group.MapPost("", async (ExhibitAddRequest newExhibit, IExhibitService service) =>
             {
                 // I prefer to not hand DTOs to the service, they should stay in the endpoint layer
                 // However, lots of very similar types are the result, so there is always a trade-off
                 OneOf<Exhibit, Error> addResult = await service
                     .AddExhibitAsync(new IExhibitService.ExhibitData(newExhibit.Name, newExhibit.Description,
                                                                      newExhibit.ServiceStartYear,
                                                                      newExhibit.ServiceEndYear,
                                                                      newExhibit.UnitsProduced, newExhibit.Country,
                                                                      newExhibit.Armor, newExhibit.Armament,
                                                                      newExhibit.Crew,
                                                                      newExhibit.ImageUrl));

                 return addResult.Match(exhibit => Results.Created($"{ApiBasePath}/{exhibit.Id}",
                                                                   ExhibitDetailsDto.FromExhibit(exhibit)),
                                        error => Results.BadRequest());
             })
             .Produces<ExhibitDetailsDto>(StatusCodes.Status201Created)
             .Produces(StatusCodes.Status400BadRequest);

        // Here we are re-using the DTO for PATCH, but will ignore the ID property of the object in this case.
        // It might be more clean to create a separate request DTO for this purpose, but it's up to you.
        group.MapPatch("{exhibitId:int}",
                       async (int exhibitId, ExhibitDetailsDto exhibit, IExhibitService service) =>
                       {
                           // validation of values should usually happen here - we will learn how
                           // to do that later, for now let's assume the values are always correct

                           OneOf<Success, NotFound> updateResult = await service
                               .UpdateExhibitAsync(exhibitId,
                                                   new IExhibitService.ExhibitData(exhibit.Name, exhibit.Description,
                                                    exhibit.ServiceStartYear, exhibit.ServiceEndYear,
                                                    exhibit.UnitsProduced, exhibit.Country,
                                                    exhibit.Armor, exhibit.Armament, exhibit.Crew,
                                                    exhibit.ImageUrl, exhibit.AddedToMuseumCollectionAt));

                           return updateResult.Match(success => Results.NoContent(),
                                                     notFound => Results.NotFound());
                       })
             .Produces(StatusCodes.Status204NoContent)
             .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("{exhibitId:int}", async (int exhibitId, IExhibitService service) =>
             {
                 OneOf<Success, NotFound> deleteResult = await service.DeleteExhibitAsync(exhibitId);

                 return deleteResult.Match(success => Results.NoContent(),
                                           notFound => Results.NotFound());
             })
             .Produces(StatusCodes.Status204NoContent)
             .Produces(StatusCodes.Status404NotFound);
    }
}

public class ExhibitInformationDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int ServiceStartYear { get; set; }
    public int? ServiceEndYear { get; set; }

    public static ExhibitInformationDto FromExhibit(Exhibit exhibit) =>
        new()
        {
            Id = exhibit.Id,
            Name = exhibit.Name,
            ServiceStartYear = exhibit.ServiceStartYear,
            ServiceEndYear = exhibit.ServiceEndYear
        };
}

public sealed class ExhibitDetailsDto : ExhibitInformationDto
{
    public required string Description { get; set; }
    public int UnitsProduced { get; set; }
    public LocalDate AddedToMuseumCollectionAt { get; set; }
    public required string Country { get; set; }
    public required string Armor { get; set; }
    public required string Armament { get; set; }
    public int Crew { get; set; }
    public string? ImageUrl { get; set; }

    public new static ExhibitDetailsDto FromExhibit(Exhibit exhibit) =>
        new()
        {
            Id = exhibit.Id,
            Name = exhibit.Name,
            ServiceStartYear = exhibit.ServiceStartYear,
            ServiceEndYear = exhibit.ServiceEndYear,
            Description = exhibit.Description,
            UnitsProduced = exhibit.UnitsProduced,
            AddedToMuseumCollectionAt = exhibit.AddedToMuseumCollectionAt,
            Country = exhibit.Country,
            Armor = exhibit.Armor,
            Armament = exhibit.Armament,
            Crew = exhibit.Crew,
            ImageUrl = exhibit.ImageUrl
        };
}

public sealed class ExhibitListResponse
{
    public required List<ExhibitInformationDto> Items { get; set; }
}

// very similar to the DTO, but without ID and AddedToMuseumCollectionAt properties which will be set by the service
public sealed class ExhibitAddRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int ServiceStartYear { get; set; }
    public int? ServiceEndYear { get; set; }
    public int UnitsProduced { get; set; }
    public required string Country { get; set; }
    public required string Armor { get; set; }
    public required string Armament { get; set; }
    public int Crew { get; set; }
    public string? ImageUrl { get; set; }
}
