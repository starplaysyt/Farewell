using Farewell.Abstractions.Infrastructure;
using Farewell.Abstractions.Mediator;
using Farewell.Debug.Domain.Entities.RepositoryUsageTestEntities;

namespace Farewell.Debug.Application.RepositoryUsageTestCommands;

public record DeleteImageCommand(uint ImageId, bool ForceDelete = false)
    : Command<DeleteImageCommand, DeleteImageResponse>;

public record DeleteImageResponse(
    bool Deleted,
    int DependentCount,
    Dictionary<string, int> DependentsByType);

public class DeleteImageCommandHandler(IQueryableRepository<TestImageEntity> repository)
    : AsyncHandler<DeleteImageCommand, DeleteImageResponse>
{
    public override async Task<OperationResult<DeleteImageResponse>> HandleAsync(
        DeleteImageCommand operation, CancellationToken cancellationToken = default)
    {
        var analyzer = repository.UsagesAnalyzer;
        
        var count = await analyzer.CountDependentsAsync(operation.ImageId, cancellationToken);
        Console.WriteLine($"\n=== Delete Image #{operation.ImageId} ===");
        Console.WriteLine($"Dependent count: {count}");
        
        var dependents = await analyzer.GetDependentsAsync(operation.ImageId, cancellationToken);
        var breakdown = new Dictionary<string, int>();

        foreach (var (type, entities) in dependents)
        {
            Console.WriteLine($"  → {type.Name}: {entities.Length} entities");
            foreach (var e in entities)
                Console.WriteLine($"      - {e}");

            breakdown[type.Name] = entities.Length;
        }
        
        if (count > 0 && !operation.ForceDelete)
        {
            Console.WriteLine("Deletion aborted: dependents exist. Use ForceDelete=true to override.");
            return new OperationResult<DeleteImageResponse>(1,
                $"Image #{operation.ImageId} has {count} dependents",
                new DeleteImageResponse(false, count, breakdown));
        }
        
        var image = await repository.FirstOrDefaultAsync(repository.GetQuery(),
            x => x.Id == operation.ImageId, cancellationToken);

        if (image is null)
        {
            Console.WriteLine("Image not found.");
            return new OperationResult<DeleteImageResponse>(2, "Not found",
                new DeleteImageResponse(false, 0, breakdown));
        }

        await repository.ExecuteDeleteAsync(repository.GetQuery().Where(e => e.Id == image.Id), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        Console.WriteLine($"Image #{operation.ImageId} deleted. {count} dependents had their FK set to NULL.");
        return new OperationResult<DeleteImageResponse>(0, "",
            new DeleteImageResponse(true, count, breakdown));
    }
}