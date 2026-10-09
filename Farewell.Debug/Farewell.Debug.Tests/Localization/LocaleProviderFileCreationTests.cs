using Farewell.Abstractions.Exceptions;
using Farewell.DI;
using Farewell.Localization;

namespace Farewell.Debug.Tests.Localization;

[Collection("Localization")]
public class LocaleProviderFileCreationTests : LocalizationTestBase
{
    [Fact]
    public void Build_FileDoesNotExist_CreatesFile()
    {
        BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "en-us"
        );

        Assert.True(File.Exists(GetLocalePath("en-us")));
    }

    [Fact]
    public void Build_FileDoesNotExist_CreatesFileWithAllKeys()
    {
        BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "en-us"
        );

        var dict = ReadLocaleFileAsDict("en-us");

        Assert.True(dict.ContainsKey(TestLocaleModule.TestMessage));
        Assert.True(dict.ContainsKey(TestLocaleModule.HelloWorld));
        Assert.True(dict.ContainsKey(TestLocaleModule.ErrorOccurred));
    }

    [Fact]
    public void Build_FileDoesNotExist_CreatesFileWithNullValues()
    {
        BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "en-us"
        );

        var dict = ReadLocaleFileAsDict("en-us");

        Assert.All(dict.Values, value => Assert.Null(value));
    }

    [Fact]
    public void Build_FileDoesNotExist_CreatesFileWithDefaultComments()
    {
        BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "en-us"
        );

        var content = ReadLocaleFileRaw("en-us");

        Assert.Contains("Default: \"Test message\"", content);
        Assert.Contains("Default: \"Hello world\"", content);
        Assert.Contains("Default: \"Error occurred\"", content);
    }

    [Fact]
    public void Build_FileDoesNotExist_KeysInMetadataTokenOrder()
    {
        BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "en-us"
        );

        var content = ReadLocaleFileRaw("en-us");

        var testMessagePos = content.IndexOf(TestLocaleModule.TestMessage, StringComparison.Ordinal);
        var helloWorldPos = content.IndexOf(TestLocaleModule.HelloWorld, StringComparison.Ordinal);
        var errorPos = content.IndexOf(TestLocaleModule.ErrorOccurred, StringComparison.Ordinal);

        Assert.True(testMessagePos < helloWorldPos);
        Assert.True(helloWorldPos < errorPos);
    }

    [Fact]
    public void Build_FileDoesNotExist_ProviderFallsBackToDefaults()
    {
        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "en-us"
        );

        var provider = GetProvider(sp, "en-us");

        Assert.Equal("Test message", provider[TestLocaleModule.TestMessage]);
        Assert.Equal("Hello world", provider[TestLocaleModule.HelloWorld]);
        Assert.Equal("Error occurred", provider[TestLocaleModule.ErrorOccurred]);
    }

    [Fact]
    public void Build_LocalesDirectoryDoesNotExist_CreatesDirectoryAndFile()
    {
        Directory.Delete(LocalesDirectory, recursive: true);

        BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "en-us"
        );

        Assert.True(Directory.Exists(LocalesDirectory));
        Assert.True(File.Exists(GetLocalePath("en-us")));
    }

    [Fact]
    public void AddLocalizationProvider_EmptyKey_ThrowsLocalizationException()
    {
        var services = new ServiceBuilder();
        services.AddLocaleModules(m => m.AddModule(typeof(TestLocaleModule)));

        Assert.Throws<LocalizationException>(() =>
            services.AddLocalizationProvider(string.Empty)
        );
    }

    [Fact]
    public void AddLocalizationProvider_WhitespaceKey_ThrowsLocalizationException()
    {
        var services = new ServiceBuilder();
        services.AddLocaleModules(m => m.AddModule(typeof(TestLocaleModule)));

        Assert.Throws<LocalizationException>(() =>
            services.AddLocalizationProvider("   ")
        );
    }
}