using System.ComponentModel.DataAnnotations.Schema;
using Farewell.Abstractions.Attributes.Domain;
using Farewell.Abstractions.Domain;

namespace Farewell.Debug.Domain.Entities.RepositoryUsageTestEntities;

[OnDelete(OnDeleteBehavior.SetNull)]
public class TestImageEntity : DomainEntity<uint>
{
    public string Url { get; set; } = "";
}

public class ArticleEntity : DomainEntity<uint>
{
    public string Title { get; set; } = "";

    [ForeignKey(nameof(CoverImage)), Column("cover_image_id")]
    public uint? CoverImageId { get; set; }

    public TestImageEntity? CoverImage { get; set; }
}

public class ProductEntity : DomainEntity<uint>
{
    public string Name { get; set; } = "";

    [ForeignKey(nameof(Thumbnail)), Column("thumbnail_id")]
    public uint? ThumbnailId { get; set; }

    public TestImageEntity? Thumbnail { get; set; }
}