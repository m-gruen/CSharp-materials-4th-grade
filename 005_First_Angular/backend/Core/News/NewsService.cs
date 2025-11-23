namespace TankMuseum.Core.News;

public interface INewsService
{
    public ValueTask<IReadOnlyCollection<NewsItem>> GetNewsItemsAsync(LocalDate? fromDate, LocalDate? toDate);
}

public sealed class NewsService(IDataStorage dataStorage) : INewsService
{
    private readonly IDataStorage _dataStorage = dataStorage;

    public async ValueTask<IReadOnlyCollection<NewsItem>> GetNewsItemsAsync(LocalDate? fromDate, LocalDate? toDate)
    {
        IEnumerable<NewsItem> newsItems = await _dataStorage.GetNewsItemsAsync();

        if (fromDate is not null)
        {
            var fromInstant = fromDate.Value.ToStartOfDayInstant();
            newsItems = newsItems.Where(item => item.PublishedAt >= fromInstant);
        }

        if (toDate is not null)
        {
            var toInstant = toDate.Value.PlusDays(1).ToStartOfDayInstant();
            newsItems = newsItems.Where(item => item.PublishedAt < toInstant);
        }

        return newsItems
               .OrderByDescending(ni => ni.PublishedAt)
               .ToList();
    }
}
