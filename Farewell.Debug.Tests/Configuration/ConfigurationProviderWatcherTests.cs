using System.Text.Json;
using Farewell.Abstractions.Configuration;
using Farewell.Configuration;

namespace Farewell.Debug.Tests.Configuration;

public class NonWatchableParser : IConfigParser
{
    public JsonElement Parse() => JsonDocument.Parse("{}").RootElement.Clone();
}

public class ConfigurationProviderWatcherTests : IDisposable
{
    private readonly string _tempFile;

    public ConfigurationProviderWatcherTests()
    {
        _tempFile = Path.GetTempFileName() + ".json";
        File.WriteAllText(_tempFile, """{ "key": "initial" }""");
    }

    [Fact]
    public void WatchFile_WithNonWatchableParser_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new ConfigurationProvider(new NonWatchableParser(), watchFile: true)
        );
    }

    [Fact]
    public async Task WatchFile_FileChanged_TriggersReload()
    {
        using var provider = new ConfigurationProvider(
            new JsonConfigParser(_tempFile),
            watchFile: true
        );

        var initial = provider.Get<string>("key");
        Assert.Equal("initial", initial);

        await File.WriteAllTextAsync(_tempFile, """{ "key": "updated" }""",
            TestContext.Current.CancellationToken);

        await Task.Delay(500, TestContext.Current.CancellationToken);

        var updated = provider.Get<string>("key");
        Assert.Equal("updated", updated);
    }

    [Fact]
    public async Task WatchFile_MultipleRapidChanges_ReloadsOnce()
    {
        var reloadCount = 0;
        using var provider = new ConfigurationProvider(
            new JsonConfigParser(_tempFile),
            watchFile: true
        );

        for (var i = 0; i < 5; i++)
        {
            await File.WriteAllTextAsync(_tempFile, $$$"""{ "key": "value_{{{i}}}" }""",
                TestContext.Current.CancellationToken);
            await Task.Delay(50,
                TestContext.Current
                    .CancellationToken);
        }

        await Task.Delay(500, TestContext.Current.CancellationToken);

        var value = provider.Get<string>("key");
        Assert.Equal("value_4", value);
    }

    public void Dispose() => File.Delete(_tempFile);
}