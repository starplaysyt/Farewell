using Farewell.Abstractions.General;
using Farewell.Abstractions.Infrastructure;
using Farewell.Abstractions.Mediator;
using Farewell.Debug.Domain.Entities.RepositoryUsageTestEntities;

namespace Farewell.Debug.Application.RepositoryUsageTestCommands;

public record CreateImageCommand(string Url) : Command<CreateImageCommand, CreateImageResponse>;
public record CreateImageResponse(uint Id);

public class CreateImageCommandHandler(IQueryableRepository<TestImageEntity> repository)
    : AsyncHandler<CreateImageCommand, CreateImageResponse>
{
    public override async Task<OperationResult<CreateImageResponse>> HandleAsync(
        CreateImageCommand operation, CancellationToken cancellationToken = default)
    {
        var entity = new TestImageEntity { Url = operation.Url };
        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        Console.WriteLine($"Created image #{entity.Id} ({entity.Url})");
        return new OperationResult<CreateImageResponse>(0, "", new CreateImageResponse(entity.Id));
    }
}

public record CreateArticleCommand(string Title, uint? CoverImageId)
    : Command<CreateArticleCommand, CreateArticleResponse>;
public record CreateArticleResponse(uint Id);

public class CreateArticleCommandHandler(IQueryableRepository<ArticleEntity> repository)
    : AsyncHandler<CreateArticleCommand, CreateArticleResponse>
{
    public override async Task<OperationResult<CreateArticleResponse>> HandleAsync(
        CreateArticleCommand operation, CancellationToken cancellationToken = default)
    {
        var entity = new ArticleEntity
        {
            Title = operation.Title,
            CoverImageId = operation.CoverImageId
        };
        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        Console.WriteLine($"Created article #{entity.Id} ('{entity.Title}', cover={entity.CoverImageId?.ToString() ?? "null"})");
        return new OperationResult<CreateArticleResponse>(0, "", new CreateArticleResponse(entity.Id));
    }
}

public record CreateProductCommand(string Name, uint? ThumbnailId)
    : Command<CreateProductCommand, CreateProductResponse>;
public record CreateProductResponse(uint Id);

public class CreateProductCommandHandler(IQueryableRepository<ProductEntity> repository)
    : AsyncHandler<CreateProductCommand, CreateProductResponse>
{
    public override async Task<OperationResult<CreateProductResponse>> HandleAsync(
        CreateProductCommand operation, CancellationToken cancellationToken = default)
    {
        var entity = new ProductEntity
        {
            Name = operation.Name,
            ThumbnailId = operation.ThumbnailId
        };
        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        Console.WriteLine($"Created product #{entity.Id} ('{entity.Name}', thumbnail={entity.ThumbnailId?.ToString() ?? "null"})");
        return new OperationResult<CreateProductResponse>(0, "", new CreateProductResponse(entity.Id));
    }
}