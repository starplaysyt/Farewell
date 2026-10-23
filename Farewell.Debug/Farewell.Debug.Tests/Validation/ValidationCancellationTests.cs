using Farewell.Abstractions.General;
using Farewell.Abstractions.Validation;
using Farewell.DI;
using Farewell.Validation.Internal;

namespace Farewell.Debug.Tests.Validation;

public sealed class ValidationCancellationTests
{
    [Fact]
    public async Task CancelledToken_ThrowsOperationCancelled()
    {
        var builder = new ConstraintBuilder<User>();
        builder.Property(x => x.Name)
            .AddCheck(ResultCode.ValidationNotEmpty, async (v, _, ct) =>
            {
                await Task.Delay(5000, ct);
                return !string.IsNullOrEmpty(v);
            });

        var compiled = builder.Compile();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await compiled.ValidateAsync(
                new User { Name = "Alice" },
                new ServiceBuilder()
                    .Build(),
                new CollectAllCollector(),
                cts.Token));
    }

    [Fact]
    public async Task Provider_CancelledToken_Propagates()
    {
        var (scope, provider) = TestProviderFactory.Create();
        using var _ = scope;
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // The built-in constraints are sync and don't check ct,
        // so this won't throw — but the token is passed through.
        // This test verifies the plumbing doesn't break.
        var report = await provider.ValidateAsync(
            new User { Name = "Alice", Email = "a@b.com" },
            ct: cts.Token);

        // Sync constraints ignore ct, so validation still completes
        Assert.True(report.IsSuccess);
    }
}