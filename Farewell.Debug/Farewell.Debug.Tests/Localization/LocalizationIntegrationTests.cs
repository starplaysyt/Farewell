namespace Farewell.Debug.Tests.Localization;

[Collection("Localization")]
public class LocalizationIntegrationTests : LocalizationTestBase
{
    [Fact]
    public void Integration_MultipleLocales_ReturnCorrectValues()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [AuthLocaleModule.LoginSuccess] = "Вход выполнен",
            [AuthLocaleModule.LoginFail] = "Ошибка входа",
            [AuthLocaleModule.Logout] = "Выход выполнен"
        });

        WriteLocaleFile("en-us", new Dictionary<string, string?>
        {
            [AuthLocaleModule.LoginSuccess] = "Login successful",
            [AuthLocaleModule.LoginFail] = "Login failed",
            [AuthLocaleModule.Logout] = "Logged out"
        });

        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(AuthLocaleModule)),
            "ru-ru", "en-us"
        );

        var ruProvider = GetProvider(sp, "ru-ru");
        var enProvider = GetProvider(sp, "en-us");

        Assert.Equal("Вход выполнен", ruProvider[AuthLocaleModule.LoginSuccess]);
        Assert.Equal("Login successful", enProvider[AuthLocaleModule.LoginSuccess]);
    }

    [Fact]
    public void Integration_MultipleModules_AllKeysAccessible()
    {
        var sp = BuildServiceProvider(
            m => m
                .AddModule(typeof(AuthLocaleModule))
                .AddModule(typeof(ProfileLocaleModule)),
            "ru-ru"
        );

        var provider = GetProvider(sp, "ru-ru");

        Assert.Equal("Login successful", provider[AuthLocaleModule.LoginSuccess]);
        Assert.Equal("Profile updated", provider[ProfileLocaleModule.ProfileUpdated]);
    }

    [Fact]
    public void Integration_PartialTranslation_FallsBackToDefault()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [AuthLocaleModule.LoginSuccess] = "Вход выполнен",
            [AuthLocaleModule.LoginFail] = null,
            [AuthLocaleModule.Logout] = null
        });

        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(AuthLocaleModule)),
            "ru-ru"
        );

        var provider = GetProvider(sp, "ru-ru");

        Assert.Equal("Вход выполнен", provider[AuthLocaleModule.LoginSuccess]);
        Assert.Equal("Login failed", provider[AuthLocaleModule.LoginFail]);
        Assert.Equal("Logged out", provider[AuthLocaleModule.Logout]);
    }

    [Fact]
    public void Integration_DefaultAndLocaleProviders_AreIndependentInstances()
    {
        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var defaultProvider = GetProvider(sp, "default");
        var ruProvider = GetProvider(sp, "ru-ru");

        Assert.NotSame(defaultProvider, ruProvider);
    }

    [Fact]
    public void Integration_ReloadAfterFileUpdate_ReflectsChanges()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [AuthLocaleModule.LoginSuccess] = "Вход выполнен",
            [AuthLocaleModule.LoginFail] = "Ошибка входа",
            [AuthLocaleModule.Logout] = "Выход выполнен"
        });

        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(AuthLocaleModule)),
            "ru-ru"
        );

        var provider = GetProvider(sp, "ru-ru");

        Assert.Equal("Вход выполнен", provider[AuthLocaleModule.LoginSuccess]);

        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [AuthLocaleModule.LoginSuccess] = "Добро пожаловать",
            [AuthLocaleModule.LoginFail] = "Ошибка входа",
            [AuthLocaleModule.Logout] = "Выход выполнен"
        });

        provider.Reload();

        Assert.Equal("Добро пожаловать", provider[AuthLocaleModule.LoginSuccess]);
    }
}