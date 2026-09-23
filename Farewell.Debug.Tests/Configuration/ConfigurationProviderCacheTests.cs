using Farewell.Configuration;

namespace Farewell.Debug.Tests.Configuration;

public class ConfigurationProviderCacheTests : IDisposable
{
    private readonly string _tempFile;
    private readonly ConfigurationProvider _provider;

    public ConfigurationProviderCacheTests()
    {
        _tempFile = Path.GetTempFileName() + ".json";
        File.WriteAllText(_tempFile, """{ "key": "value" }""");
        _provider = new ConfigurationProvider(new JsonConfigParser(_tempFile));
    }

    [Fact]
    public void Get_AfterCaching_DoesNotReadFileAgain()
    {
        var first = _provider.Get<string>("key");

        File.WriteAllText(_tempFile, """{ "key": "changed" }""");

        var second = _provider.Get<string>("key");

        Assert.Equal(first, second);
        Assert.Equal("value", second);
    }

    [Fact]
    public void Reload_InvalidatesCache_ReturnsNewValue()
    {
        var first = _provider.Get<string>("key");

        File.WriteAllText(_tempFile, """{ "key": "changed" }""");
        _provider.Reload();

        var second = _provider.Get<string>("key");

        Assert.NotEqual(first, second);
        Assert.Equal("changed", second);
    }

    [Fact]
    public void Reload_AfterReload_CacheRepopulates()
    {
        _provider.Get<string>("key");
        _provider.Reload();

        File.WriteAllText(_tempFile, """{ "key": "new" }""");
        _provider.Reload();

        var value = _provider.Get<string>("key");
        Assert.Equal("new", value);
    }

    public void Dispose() => File.Delete(_tempFile);
}