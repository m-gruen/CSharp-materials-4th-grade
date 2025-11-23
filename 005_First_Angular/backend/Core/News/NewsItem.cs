namespace TankMuseum.Core.News;

public sealed class NewsItem(int id, Instant publishedAt, string title, string content, HashSet<int> relatedExhibitIds)
{
    public int Id { get; } = id;
    public Instant PublishedAt { get; } = publishedAt;
    public string Title { get; } = title;
    public string Content { get; } = content;
    public HashSet<int> RelatedExhibitIds { get; } = relatedExhibitIds;
}
