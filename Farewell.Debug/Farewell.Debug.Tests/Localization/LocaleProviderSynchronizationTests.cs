namespace Farewell.Debug.Tests.Localization;

[Collection("Localization")]
public class LocaleProviderSynchronizationTests : LocalizationTestBase
{
    [Fact]
    public void Build_FileMissingKeys_AddsMissingKeysToFile()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Тестовое сообщение"
        });

        BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var dict = ReadLocaleFileAsDict("ru-ru");

        Assert.True(dict.ContainsKey(TestLocaleModule.TestMessage));
        Assert.True(dict.ContainsKey(TestLocaleModule.HelloWorld));
        Assert.True(dict.ContainsKey(TestLocaleModule.ErrorOccurred));
    }

    [Fact]
    public void Build_FileMissingKeys_NewKeysAreNullInFile()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Тестовое сообщение"
        });

        BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var dict = ReadLocaleFileAsDict("ru-ru");

        Assert.Null(dict[TestLocaleModule.HelloWorld]);
        Assert.Null(dict[TestLocaleModule.ErrorOccurred]);
    }

    [Fact]
    public void Build_FileMissingKeys_ExistingValuesPreserved()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Тестовое сообщение"
        });

        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var provider = GetProvider(sp, "ru-ru");

        Assert.Equal("Тестовое сообщение", provider[TestLocaleModule.TestMessage]);
    }

    [Fact]
    public void Build_FileMissingKeys_MissingKeysFallBackToDefault()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Тестовое сообщение"
        });

        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var provider = GetProvider(sp, "ru-ru");

        Assert.Equal("Hello world", provider[TestLocaleModule.HelloWorld]);
        Assert.Equal("Error occurred", provider[TestLocaleModule.ErrorOccurred]);
    }

    [Fact]
    public void Build_FileHasExtraKeys_RemovesExtraKeysFromFile()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Тестовое сообщение",
            [TestLocaleModule.HelloWorld] = "Привет мир",
            [TestLocaleModule.ErrorOccurred] = "Произошла ошибка",
            ["STR_EXTRA_KEY"] = "Лишний ключ"
        });

        BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var dict = ReadLocaleFileAsDict("ru-ru");

        Assert.False(dict.ContainsKey("STR_EXTRA_KEY"));
    }

    [Fact]
    public void Build_FileHasExtraKeys_ValidValuesPreserved()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Тестовое сообщение",
            [TestLocaleModule.HelloWorld] = "Привет мир",
            [TestLocaleModule.ErrorOccurred] = "Произошла ошибка",
            ["STR_EXTRA_KEY"] = "Лишний ключ"
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
    public void Build_FileMissingAndExtraKeys_SynchronizesCorrectly()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Тестовое сообщение",
            ["STR_EXTRA_KEY"] = "Лишний ключ"
        });

        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var provider = GetProvider(sp, "ru-ru");
        var dict = ReadLocaleFileAsDict("ru-ru");

        Assert.False(dict.ContainsKey("STR_EXTRA_KEY"));
        Assert.True(dict.ContainsKey(TestLocaleModule.HelloWorld));
        Assert.True(dict.ContainsKey(TestLocaleModule.ErrorOccurred));
        Assert.Equal("Тестовое сообщение", provider[TestLocaleModule.TestMessage]);
        Assert.Equal("Hello world", provider[TestLocaleModule.HelloWorld]);
    }

    [Fact]
    public void Build_FileAlreadySynchronized_DoesNotRewriteFile()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Тестовое сообщение",
            [TestLocaleModule.HelloWorld] = "Привет мир",
            [TestLocaleModule.ErrorOccurred] = "Произошла ошибка"
        });

        var lastWriteTime = File.GetLastWriteTimeUtc(GetLocalePath("ru-ru"));

        Thread.Sleep(50);

        BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var newWriteTime = File.GetLastWriteTimeUtc(GetLocalePath("ru-ru"));

        Assert.Equal(lastWriteTime, newWriteTime);
    }
}