using Farewell.Abstractions.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Infrastructure;

public class EFUnitOfWork(DbContext context) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await context.SaveChangesAsync(ct);

    public async Task<ITransactionInfo> BeginTransactionAsync(CancellationToken ct = default) =>
        new EFTransactionInfo {  DbTransaction = await context.Database.BeginTransactionAsync(ct) };

    public async Task CommitTransactionAsync(ITransactionInfo info, CancellationToken ct = default)
    {
        if (info is not EFTransactionInfo transactionInfo)
            throw new InvalidOperationException("Invalid transaction info for EFUnitOfWork.");
        if (transactionInfo.DbTransaction is null)
            throw new InvalidOperationException("Transaction is invalid.");
        
        await transactionInfo.DbTransaction.CommitAsync(ct);
        await transactionInfo.DisposeAsync();
        transactionInfo.DbTransaction = null!;
    }

    public async Task RollbackTransactionAsync(ITransactionInfo info, CancellationToken ct = default)
    {
        if (info is not EFTransactionInfo transactionInfo)
            throw new InvalidOperationException("Invalid transaction info for EFUnitOfWork.");
        if (transactionInfo.DbTransaction is null)
            throw new InvalidOperationException("Transaction is invalid.");
        
        await transactionInfo.DbTransaction.RollbackAsync(ct);
        await transactionInfo.DisposeAsync();
        transactionInfo.DbTransaction = null!;
    }
}