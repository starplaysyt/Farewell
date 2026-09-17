namespace Farewell.Abstractions.Infrastructure;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    Task<ITransactionInfo> BeginTransactionAsync(CancellationToken ct = default);

    Task CommitTransactionAsync(ITransactionInfo info, CancellationToken ct = default);

    Task RollbackTransactionAsync(ITransactionInfo info, CancellationToken ct = default);
}