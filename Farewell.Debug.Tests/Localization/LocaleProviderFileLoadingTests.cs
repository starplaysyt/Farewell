using Farewell.Abstractions.Exceptions;

namespace Farewell.Debug.Tests.Localization;

[Collection("Localization")]
public class LocaleProviderFileLoadingTests : LocalizationTestBase
{
    [Fact]
    public void Build_FileWithAllValues_LoadsCorrectly()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Тестовое сообщение",
            [TestLocaleModule.HelloWorld] = "Привет мир",
            [TestLocaleModule.ErrorOccurred] = "Произошла ошибка"
        });

        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var provider = GetProvider(sp, "ru-ru");

        Assert.Equal("Тестовое сообщение", provider[TestLocaleModule.TestMessage]);
        Assert.Equal("Привет мир", provider[TestLocaleModule.HelloWorld]);
        Assert.Equal("Произошла ошибка", provider[TestLocaleModule.ErrorOccurred]);
    }

    [Fact]
    public void Build_FileWithSomeNullValues_FallsBackToDefault()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Тестовое сообщение",
            [TestLocaleModule.HelloWorld] = null,
            [TestLocaleModule.ErrorOccurred] = null
        });

        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var provider = GetProvider(sp, "ru-ru");

        Assert.Equal("Тестовое сообщение", provider[TestLocaleModule.TestMessage]);
        Assert.Equal("Hello world", provider[TestLocaleModule.HelloWorld]);
        Assert.Equal("Error occurred", provider[TestLocaleModule.ErrorOccurred]);
    }

    [Fact]
    public void Build_FileWithAllNullValues_AllFallBackToDefault()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = null,
            [TestLocaleModule.HelloWorld] = null,
            [TestLocaleModule.ErrorOccurred] = null
        });

        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var provider = GetProvider(sp, "ru-ru");

        Assert.Equal("Test message", provider[TestLocaleModule.TestMessage]);
        Assert.Equal("Hello world", provider[TestLocaleModule.HelloWorld]);
        Assert.Equal("Error occurred", provider[TestLocaleModule.ErrorOccurred]);
    }

    [Fact]
    public void Build_ProviderIsSingleton_SameInstance()
    {
        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var provider1 = GetProvider(sp, "ru-ru");
        var provider2 = GetProvider(sp, "ru-ru");

        Assert.Same(provider1, provider2);
    }

    [Fact]
    public void Indexer_UnknownKey_ThrowsLocalizationException()
    {
        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var provider = GetProvider(sp, "ru-ru");

        var ex = Assert.Throws<LocalizationException>(() =>
            _ = provider["STR_UNKNOWN"]
        );

        Assert.Contains("STR_UNKNOWN", ex.Message);
        Assert.Contains("ru-ru", ex.Message);
    }
}