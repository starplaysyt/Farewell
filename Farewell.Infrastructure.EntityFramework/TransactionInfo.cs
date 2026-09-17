using System.Data.Common;
using Farewell.Abstractions.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Farewell.EF;

internal class TransactionInfo : ITransactionInfo, IAsyncDisposable
{
    public IDbContextTransaction DbTransaction { get; set; }

    public void Dispose()
    {
        DbTransaction.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await DbTransaction.DisposeAsync();
    }
}