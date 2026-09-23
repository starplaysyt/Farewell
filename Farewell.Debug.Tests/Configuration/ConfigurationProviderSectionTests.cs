using Farewell.Abstractions.Attributes.Configuration;
using Farewell.Configuration;
using Farewell.Configuration.Exceptions;

namespace Farewell.Debug.Tests.Configuration;

[ConfigSection("database")]
public class DatabaseConfig
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Name { get; set; } = string.Empty;
}

[ConfigSection("server")]
public class ServerConfig
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
}

public class MissingAttributeConfig
{
}

public class ConfigurationProviderSectionTests : IDisposable
{
    private readonly string _tempFile;
    private readonly ConfigurationProvider _provider;

    public ConfigurationProviderSectionTests()
    {
        _tempFile = Path.GetTempFileName() + ".json";
        var json = """
                   {
                       "database": {
                           "host": "db.local",
                           "port": 5432,
                           "name": "mydb"
                       },
                       "server": {
                           "host": "localhost",
                           "port": 8080
                       }
                   }
                   """;
        File.WriteAllText(_tempFile, json);
        _provider = new ConfigurationProvider(new JsonConfigParser(_tempFile));
    }

    [Fact]
    public void GetSection_WithPath_ReturnsDeserializedObject()
    {
        var config = _provider.GetSection<DatabaseConfig>("database");

        Assert.NotNull(config);
        Assert.Equal("db.local", config.Host);
        Assert.Equal(5432, config.Port);
        Assert.Equal("mydb", config.Name);
    }

    [Fact]
    public void GetSection_WithAttribute_ReturnsDeserializedObject()
    {
        var config = _provider.GetSection<ServerConfig>();

        Assert.NotNull(config);
        Assert.Equal("localhost", config.Host);
        Assert.Equal(8080, config.Port);
    }

    [Fact]
    public void GetSection_MissingAttribute_ThrowsConfigSectionAttributeMissingException()
    {
        Assert.Throws<ConfigSectionAttributeMissingException>(() =>
            _provider.GetSection<MissingAttributeConfig>()
        );
    }

    [Fact]
    public void GetSection_NonExistentPath_ThrowsConfigKeyNotFoundException()
    {
        Assert.Throws<ConfigKeyNotFoundException>(() =>
            _provider.GetSection<DatabaseConfig>("nonexistent")
        );
    }

    [Fact]
    public void GetSection_SameTypeTwice_ReturnsCachedInstance()
    {
        var first = _provider.GetSection<DatabaseConfig>();
        var second = _provider.GetSection<DatabaseConfig>();

        Assert.Same(first, second);
    }

    [Fact]
    public void GetSection_AfterReload_ReturnsNewInstance()
    {
        var first = _provider.GetSection<DatabaseConfig>();

        File.WriteAllText(_tempFile, """
                                     {
                                         "database": {
                                             "host": "new.host",
                                             "port": 5432,
                                             "name": "mydb"
                                         }
                                     }
                                     """);
        _provider.Reload();

        var second = _provider.GetSection<DatabaseConfig>();

        Assert.NotSame(first, second);
        Assert.Equal("new.host", second!.Host);
    }

    public void Dispose() => File.Delete(_tempFile);
}