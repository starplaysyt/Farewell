using Farewell.Abstractions.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Farewell.Infrastructure;

internal class EFTransactionInfo : ITransactionInfo, IAsyncDisposable
{
    public required IDbContextTransaction DbTransaction { get; set; }

    public void Dispose()
    {
        DbTransaction.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await DbTransaction.DisposeAsync();
    }
}