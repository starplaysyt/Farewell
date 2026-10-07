namespace Farewell.Debug.Tests.Validation;

public sealed class CancellationTests
{
    [Fact]
    public async Task ValidateAsync_CancelledToken_Throws()
    {
        var (scope, provider) = ProviderFactory.CreateProvider();
        using (scope)
        {
            var cts = new CancellationTokenSource();
            await cts.CancelAsync();

            var user = new User { Name = "John", Email = "j@m.co", Age = 25 };

            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                await provider.ValidateAllAsync(user, "Default", cts.Token));
        }
    }
}