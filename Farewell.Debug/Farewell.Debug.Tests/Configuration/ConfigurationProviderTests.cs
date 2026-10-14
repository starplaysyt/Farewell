using Farewell.Abstractions.Configuration;
using Farewell.Abstractions.Configuration.Exceptions;
using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.Configuration;
using Farewell.DI;

namespace Farewell.Debug.Tests.Configuration;

public sealed class ConfigurationProviderTests(TempDirectoryFixture fixture)
    : IClassFixture<TempDirectoryFixture>
{
    private const string Extension = ".fake";

    private static IServiceProvider BuildContainer(
        string path,
        FakeConfigResolver resolver)
    {
        var builder = new ServiceBuilder();
        builder.AddKeyedSingleton<IConfigResolver>(Extension, (_) => resolver);
        builder.AddConfig<TestConfig>(path);
        return builder.Build();
    }

    [Fact]
    public void Get_WhenFileExists_ReturnsResolvedConfig()
    {
        var path = fixture.GetFilePath($"config{Extension}");
        File.WriteAllText(path, "{}");

        var expected = new TestConfig { Name = "FromFile", Value = 99 };
        var resolver = new FakeConfigResolver(resolveFunc: _ => expected);

        var provider = BuildContainer(path, resolver)
            .GetRequiredService<IConfigurationProvider<TestConfig>>();

        var result = provider.Get();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Get_WhenFileExists_CallsResolverWithCorrectPath()
    {
        var path = fixture.GetFilePath($"resolver_path{Extension}");
        File.WriteAllText(path, "{}");

        var resolver = new FakeConfigResolver();

        BuildContainer(path, resolver)
            .GetRequiredService<IConfigurationProvider<TestConfig>>();

        Assert.Contains(path, resolver.ResolvedPaths);
    }

    [Fact]
    public void Get_CalledMultipleTimes_ResolverCalledOnlyOnce()
    {
        var path = fixture.GetFilePath($"cached{Extension}");
        File.WriteAllText(path, "{}");

        var resolver = new FakeConfigResolver();

        var provider = BuildContainer(path, resolver)
            .GetRequiredService<IConfigurationProvider<TestConfig>>();

        provider.Get();
        provider.Get();
        provider.Get();

        Assert.Single(resolver.ResolvedPaths);
    }

    [Fact]
    public void Get_WhenFileNotExists_ReturnsDefaultConfig()
    {
        var path = fixture.GetFilePath($"nonexistent{Extension}");
        var resolver = new FakeConfigResolver();

        var provider = BuildContainer(path, resolver)
            .GetRequiredService<IConfigurationProvider<TestConfig>>();

        var result = provider.Get();

        Assert.NotNull(result);
        Assert.Equal("Default", result.Name);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Get_WhenFileNotExists_SavesDefaultsToFile()
    {
        var path = fixture.GetFilePath($"will_be_created{Extension}");
        var resolver = new FakeConfigResolver();

        BuildContainer(path, resolver)
            .GetRequiredService<IConfigurationProvider<TestConfig>>();

        Assert.Contains(path, resolver.SavedPaths);
    }

    [Fact]
    public void Get_WhenFileNotExists_DoesNotCallResolve()
    {
        var path = fixture.GetFilePath($"no_resolve{Extension}");
        var resolver = new FakeConfigResolver();

        BuildContainer(path, resolver)
            .GetRequiredService<IConfigurationProvider<TestConfig>>();

        Assert.Empty(resolver.ResolvedPaths);
    }

    [Fact]
    public void Get_WhenDirectoryNotExists_CreatesDirectoryAndFile()
    {
        var path = fixture.GetFilePath(Path.Combine("nested", "deep", $"config{Extension}"));
        var resolver = new FakeConfigResolver();

        BuildContainer(path, resolver)
            .GetRequiredService<IConfigurationProvider<TestConfig>>();

        Assert.True(Directory.Exists(Path.GetDirectoryName(path)));
        Assert.Contains(path, resolver.SavedPaths);
    }

    [Fact]
    public void Build_WhenResolverThrows_ThrowsConfigurationResolvingException()
    {
        var path = fixture.GetFilePath($"invalid{Extension}");
        File.WriteAllText(path, "{}");

        var resolver = new FakeConfigResolver(
            resolveFunc: _ => throw new InvalidOperationException("bad content"));

        var builder = new ServiceBuilder();
        builder.AddKeyedSingleton<IConfigResolver>(Extension, (_) => resolver);
        builder.AddConfig<TestConfig>(path);

        Assert.Throws<ConfigurationResolvingException>(builder.Build);
    }

    [Fact]
    public void Build_WhenResolverThrowsConfigurationException_NotWrapped()
    {
        var path = fixture.GetFilePath($"invalid2{Extension}");
        File.WriteAllText(path, "{}");

        var resolver = new FakeConfigResolver(
            resolveFunc: _ => throw new ConfigurationResolvingException(
                path, new Exception("inner")));

        var builder = new ServiceBuilder();
        builder.AddKeyedSingleton<IConfigResolver>(Extension, (_) => resolver);
        builder.AddConfig<TestConfig>(path);

        var ex = Assert.Throws<ConfigurationResolvingException>(builder.Build);

        Assert.Equal(path, ex.ConfigPath);
    }

    [Fact]
    public void Build_WhenResolverThrows_ExceptionContainsPath()
    {
        var path = fixture.GetFilePath($"bad{Extension}");
        File.WriteAllText(path, "{}");

        var builder = new ServiceBuilder();
        builder.AddKeyedSingleton<IConfigResolver>(Extension, (_) => new FakeConfigResolver(
            resolveFunc: _ => throw new Exception("parse error")));
        builder.AddConfig<TestConfig>(path);

        var ex = Assert.ThrowsAny<ConfigurationResolvingException>(builder.Build);

        Assert.Equal(path, ex.ConfigPath);
    }

    [Fact]
    public void Reload_UpdatesCachedValue()
    {
        var path = fixture.GetFilePath($"reload{Extension}");
        File.WriteAllText(path, "{}");

        var first = new TestConfig { Name = "First" };
        var second = new TestConfig { Name = "Second" };
        var callCount = 0;

        var resolver = new FakeConfigResolver(
            resolveFunc: _ => ++callCount == 1 ? first : second);

        var provider = BuildContainer(path, resolver)
            .GetRequiredService<IConfigurationProvider<TestConfig>>();

        provider.Reload();
        var result = provider.Get();

        Assert.Equal("Second", result.Name);
    }

    [Fact]
    public void Reload_WhenResolverThrows_KeepsOldCache()
    {
        var path = fixture.GetFilePath($"reload_fail{Extension}");
        File.WriteAllText(path, "{}");

        var original = new TestConfig { Name = "Original" };
        var callCount = 0;

        var resolver = new FakeConfigResolver(resolveFunc: _ =>
            ++callCount > 1 ? throw new InvalidOperationException("corrupted") : original);

        var provider = BuildContainer(path, resolver)
            .GetRequiredService<IConfigurationProvider<TestConfig>>();
        
        Assert.Throws<ConfigurationResolvingException>(provider.Reload);
        
        Assert.Equal("Original", provider.Get().Name);
    }
}