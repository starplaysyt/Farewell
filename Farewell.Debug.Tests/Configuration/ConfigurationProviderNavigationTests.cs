using Farewell.Configuration;
using Farewell.Configuration.Exceptions;

namespace Farewell.Debug.Tests.Configuration;

public class ConfigurationProviderNavigationTests : IDisposable
{
    private readonly string _tempFile;
    private readonly ConfigurationProvider _provider;

    public ConfigurationProviderNavigationTests()
    {
        _tempFile = Path.GetTempFileName() + ".json";
        var json = """
                   {
                       "server": {
                           "port": "8080",
                           "host": "localhost"
                       },
                       "database": {
                           "connection": {
                               "host": "db.local",
                               "port": "5432"
                           },
                           "name": "mydb"
                       },
                       "feature": "enabled"
                   }
                   """;
        File.WriteAllText(_tempFile, json);
        _provider = new ConfigurationProvider(new JsonConfigParser(_tempFile));
    }

    [Fact]
    public void Get_TopLevelKey_ReturnsValue()
    {
        var value = _provider.Get<string>("feature");
        Assert.Equal("enabled", value);
    }

    [Fact]
    public void Get_NestedKey_ReturnsValue()
    {
        var value = _provider.Get<string>("server:host");
        Assert.Equal("localhost", value);
    }

    [Fact]
    public void Get_DeeplyNestedKey_ReturnsValue()
    {
        var value = _provider.Get<string>("database:connection:host");
        Assert.Equal("db.local", value);
    }

    [Fact]
    public void Get_NonExistentKey_ThrowsConfigKeyNotFoundException()
    {
        Assert.Throws<ConfigKeyNotFoundException>(() =>
            _provider.Get<string>("nonexistent:key")
        );
    }

    [Fact]
    public void Get_IntValue_ReturnsParsedInt()
    {
        var value = _provider.Get<int>("server:port");
        Assert.Equal(8080, value);
    }

    [Fact]
    public void Get_SameKeyTwice_ReturnsSameValue()
    {
        var first = _provider.Get<string>("server:host");
        var second = _provider.Get<string>("server:host");

        Assert.Equal(first, second);
    }

    public void Dispose() => File.Delete(_tempFile);
}