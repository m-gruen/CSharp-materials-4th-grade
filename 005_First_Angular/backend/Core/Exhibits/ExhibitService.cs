using OneOf;
using OneOf.Types;

namespace TankMuseum.Core.Exhibits;

public interface IExhibitService
{
    public ValueTask<IReadOnlyCollection<Exhibit>> GetAllExhibitsAsync();
    public ValueTask<OneOf<Exhibit, NotFound>> GetExhibitByIdAsync(int exhibitId);
    public ValueTask<OneOf<Exhibit, Error>> AddExhibitAsync(ExhibitData newExhibit);
    public ValueTask<OneOf<Success, NotFound>> UpdateExhibitAsync(int exhibitId, ExhibitData exhibitData);
    public ValueTask<OneOf<Success, NotFound>> DeleteExhibitAsync(int exhibitId);

    public sealed record ExhibitData(
        string Name,
        string Description,
        int ServiceStartYear,
        int? ServiceEndYear,
        int UnitsProduced,
        string Country,
        string Armor,
        string Armament,
        int Crew,
        string? ImageUrl,
        LocalDate? AddedToMuseumCollectionAt = null);
}

public sealed class ExhibitService(IDataStorage dataStorage, IClock clock) : IExhibitService
{
    private readonly IClock _clock = clock;
    private readonly IDataStorage _dataStorage = dataStorage;
    private int? _nextId;

    public async ValueTask<IReadOnlyCollection<Exhibit>> GetAllExhibitsAsync()
    {
        IEnumerable<Exhibit> exhibits = await _dataStorage.GetExhibitsAsync();

        return exhibits.ToList();
    }

    public async ValueTask<OneOf<Exhibit, NotFound>> GetExhibitByIdAsync(int exhibitId)
    {
        var exhibit = await GetExhibitById(exhibitId);

        return exhibit is not null ? exhibit : new NotFound();
    }

    public async ValueTask<OneOf<Exhibit, Error>> AddExhibitAsync(IExhibitService.ExhibitData newExhibit)
    {
        try
        {
            // ID would be typically assigned by the database, but we don't have one here
            int id = await GetNextId();
            var exhibit = new Exhibit
            {
                Id = id,
                Name = newExhibit.Name,
                Description = newExhibit.Description,
                ServiceStartYear = newExhibit.ServiceStartYear,
                ServiceEndYear = newExhibit.ServiceEndYear,
                AddedToMuseumCollectionAt = _clock.GetCurrentInstant().ToLocalDateTime().Date,
                UnitsProduced = newExhibit.UnitsProduced,
                Country = newExhibit.Country,
                Armor = newExhibit.Armor,
                Armament = newExhibit.Armament,
                Crew = newExhibit.Crew,
                ImageUrl = newExhibit.ImageUrl
            };

            await _dataStorage.AddExhibitAsync(exhibit);

            return exhibit;
        }
        catch (Exception)
        {
            // here we should log...

            // If we do not catch an exception in the service layer, it will lead to a 500 status code.
            // Usually, we want to handle everything we expect to sometimes happen, but don't bother
            // with unexpected exceptions => let them bubble up and return a 500 status code to the client,
            // but still log them to be able to investigate the issue!

            return new Error();
        }
    }

    public async ValueTask<OneOf<Success, NotFound>> UpdateExhibitAsync(
        int exhibitId, IExhibitService.ExhibitData exhibitData)
    {
        var exhibit = await GetExhibitById(exhibitId);
        
        if (exhibit is null)
        {
            return new NotFound();
        }
        
        exhibit.Name = exhibitData.Name;
        exhibit.Description = exhibitData.Description;
        exhibit.ServiceStartYear = exhibitData.ServiceStartYear;
        exhibit.ServiceEndYear = exhibitData.ServiceEndYear;
        exhibit.UnitsProduced = exhibitData.UnitsProduced;
        exhibit.Country = exhibitData.Country;
        exhibit.Armor = exhibitData.Armor;
        exhibit.Armament = exhibitData.Armament;
        exhibit.Crew = exhibitData.Crew;
        exhibit.ImageUrl = exhibitData.ImageUrl;

        if (exhibitData.AddedToMuseumCollectionAt is not null 
            && exhibitData.AddedToMuseumCollectionAt.Value != default(LocalDate))
        {
            exhibit.AddedToMuseumCollectionAt = exhibitData.AddedToMuseumCollectionAt.Value;
        }
        
        // why do you think there is no call to the data storage needed here?

        return new Success();
    }

    public async ValueTask<OneOf<Success, NotFound>> DeleteExhibitAsync(int exhibitId)
    {
        var exhibit = await GetExhibitById(exhibitId);

        if (exhibit is null)
        {
            return new NotFound();
        }

        await _dataStorage.RemoveExhibitAsync(exhibit);

        return new Success();
    }

    private async ValueTask<Exhibit?> GetExhibitById(int exhibitId)
    {
        IEnumerable<Exhibit> exhibits = await _dataStorage.GetExhibitsAsync();

        return exhibits.FirstOrDefault(e => e.Id == exhibitId);
    }

    private async ValueTask<int> GetNextId()
    {
        if (_nextId is null)
        {
            int maxId = await FindMaxId();
            _nextId = maxId + 1;

            return _nextId.Value;
        }

        int retVal = _nextId.Value;
        _nextId++;

        return retVal;
    }

    private async ValueTask<int> FindMaxId()
    {
        IEnumerable<Exhibit> exhibits = await _dataStorage.GetExhibitsAsync();

        return exhibits.Max(e => e.Id);
    }
}
