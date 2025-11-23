using NodaTime.Text;

namespace TankMuseum.Core.News;

public static class NewsEndpoint
{
    private const string ApiBasePath = "/api/news";

    public static void MapNewsEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiBasePath);

        group.MapGet("", async (string? from, string? to, INewsService service, IClock clock) =>
             {
                 LocalDate? fromDate = ParseOptionalDate(from);
                 LocalDate? toDate = ParseOptionalDate(to);

                 if (!AreParamsValid())
                 {
                     return Results.BadRequest();
                 }

                 IReadOnlyCollection<NewsItem> newsItems = await service.GetNewsItemsAsync(fromDate, toDate);

                 return Results.Ok(new NewsItemListResponse
                 {
                     Items = newsItems.Select(NewsItemDto.FromNewsItem).ToList()
                 });

                 bool AreParamsValid()
                 {
                     var today = clock.GetCurrentInstant().ToLocalDateTime().Date;

                     return !(fromDate > today)
                            && !(toDate > today)
                            && !(fromDate > toDate);
                 }
             })
             .Produces<NewsItemListResponse>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest);
    }

    private static LocalDate? ParseOptionalDate(string? dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
        {
            return null;
        }

        ParseResult<LocalDate> result = LocalDatePattern.Iso.Parse(dateString);

        return result.Success ? result.Value : null;
    }
}

public sealed class NewsItemDto
{
    public int Id { get; set; }
    public Instant PublishedAt { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required List<int> RelatedExhibitIds { get; set; }

    public static NewsItemDto FromNewsItem(NewsItem newsItem) =>
        new()
        {
            Id = newsItem.Id,
            PublishedAt = newsItem.PublishedAt,
            Title = newsItem.Title,
            Content = newsItem.Content,
            RelatedExhibitIds = newsItem.RelatedExhibitIds.ToList()
        };
}

public sealed class NewsItemListResponse
{
    public required List<NewsItemDto> Items { get; set; }
}
