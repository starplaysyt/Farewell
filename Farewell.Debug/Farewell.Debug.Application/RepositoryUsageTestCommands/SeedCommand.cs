using Farewell.Abstractions.Infrastructure;
using Farewell.Abstractions.Mediator;
using Farewell.Debug.Domain.Entities.RepositoryUsageTestEntities;

namespace Farewell.Debug.Application.RepositoryUsageTestCommands;

public record SeedTestDataCommand() : Command<SeedTestDataCommand, SeedTestDataResponse>;

public record SeedTestDataResponse(
    uint[] ImageIds,
    uint[] ArticleIds,
    uint[] ProductIds);

public class SeedTestDataCommandHandler(
    IQueryableRepository<TestImageEntity> imageRepo,
    IQueryableRepository<ArticleEntity> articleRepo,
    IQueryableRepository<ProductEntity> productRepo)
    : AsyncHandler<SeedTestDataCommand, SeedTestDataResponse>
{
    public override async Task<OperationResult<SeedTestDataResponse>> HandleAsync(
        SeedTestDataCommand operation, CancellationToken cancellationToken = default)
    {
        var image1 = new TestImageEntity { Url = "https://example.com/img1.png" };
        var image2 = new TestImageEntity { Url = "https://example.com/img2.png" };

        await imageRepo.AddAsync(image1, cancellationToken);
        await imageRepo.AddAsync(image2, cancellationToken);
        await imageRepo.SaveChangesAsync(cancellationToken);

        var article1 = new ArticleEntity { Title = "Article #1", CoverImageId = image1.Id };
        var article2 = new ArticleEntity { Title = "Article #2", CoverImageId = image1.Id };
        var article3 = new ArticleEntity { Title = "Article #3", CoverImageId = image2.Id };

        await articleRepo.AddAsync(article1, cancellationToken);
        await articleRepo.AddAsync(article2, cancellationToken);
        await articleRepo.AddAsync(article3, cancellationToken);
        await articleRepo.SaveChangesAsync(cancellationToken);

        var product1 = new ProductEntity { Name = "Product #1", ThumbnailId = image1.Id };
        var product2 = new ProductEntity { Name = "Product #2", ThumbnailId = null };

        await productRepo.AddAsync(product1, cancellationToken);
        await productRepo.AddAsync(product2, cancellationToken);
        await productRepo.SaveChangesAsync(cancellationToken);

        Console.WriteLine("=== Seed completed ===");
        Console.WriteLine($"Images:   [{image1.Id}, {image2.Id}]");
        Console.WriteLine($"Articles: [{article1.Id}, {article2.Id}, {article3.Id}]");
        Console.WriteLine($"Products: [{product1.Id}, {product2.Id}]");
        Console.WriteLine($"→ Image #{image1.Id} is referenced by 2 articles + 1 product (3 total)");
        Console.WriteLine($"→ Image #{image2.Id} is referenced by 1 article (1 total)");

        return new OperationResult<SeedTestDataResponse>(0, "",
            new SeedTestDataResponse(
                [image1.Id, image2.Id],
                [article1.Id, article2.Id, article3.Id],
                [product1.Id, product2.Id]));
    }
}