using Farewell.Abstractions.Exceptions;
using Farewell.DI;
using Farewell.Localization;

namespace Farewell.Debug.Tests.Localization;

[Collection("Localization")]
public class DefaultLocaleProviderTests : LocalizationTestBase
{
    [Fact]
    public void Build_ValidModule_DefaultProviderHasCorrectValues()
    {
        var sp = BuildServiceProvider(m => m.AddModule(typeof(TestLocaleModule))
        );

        var provider = GetProvider(sp, "default");

        Assert.Equal("Test message", provider[TestLocaleModule.TestMessage]);
        Assert.Equal("Hello world", provider[TestLocaleModule.HelloWorld]);
        Assert.Equal("Error occurred", provider[TestLocaleModule.ErrorOccurred]);
    }

    [Fact]
    public void Build_MultipleModules_DefaultProviderHasAllKeys()
    {
        var sp = BuildServiceProvider(m => m
            .AddModule(typeof(AuthLocaleModule))
            .AddModule(typeof(ProfileLocaleModule))
        );

        var provider = GetProvider(sp, "default");

        Assert.Equal("Login successful", provider[AuthLocaleModule.LoginSuccess]);
        Assert.Equal("Login failed", provider[AuthLocaleModule.LoginFail]);
        Assert.Equal("Logged out", provider[AuthLocaleModule.Logout]);
        Assert.Equal("Profile updated", provider[ProfileLocaleModule.ProfileUpdated]);
        Assert.Equal("Avatar changed", provider[ProfileLocaleModule.AvatarChanged]);
    }

    [Fact]
    public void Build_ModuleWithoutAttribute_ThrowsLocalizationException()
    {
        var services = new ServiceBuilder();
        services.AddLocaleModules(m => m.AddModule(typeof(ModuleWithoutAttribute)));

        var ex = Assert.Throws<LocalizationException>(services.Build);

        Assert.Contains("DefaultLocale", ex.Message);
    }

    [Fact]
    public void Build_ModuleWithNonConstField_ThrowsLocalizationException()
    {
        var services = new ServiceBuilder();
        services.AddLocaleModules(m => m.AddModule(typeof(ModuleWithNonConstField)));

        var ex = Assert.Throws<LocalizationException>(services.Build);

        Assert.Contains("const", ex.Message);
    }

    [Fact]
    public void Build_ModuleWithNonStringField_ThrowsLocalizationException()
    {
        var services = new ServiceBuilder();
        services.AddLocaleModules(m => m.AddModule(typeof(ModuleWithNonStringField)));

        var ex = Assert.Throws<LocalizationException>(services.Build
        );

        Assert.Contains("string", ex.Message);
    }

    [Fact]
    public void Build_ModuleWithDuplicateKeys_ThrowsLocalizationException()
    {
        var services = new ServiceBuilder();
        services.AddLocaleModules(m => m.AddModule(typeof(ModuleWithDuplicateKeys)));

        var ex = Assert.Throws<LocalizationException>(services.Build
        );

        Assert.Contains("Duplicate", ex.Message);
        Assert.Contains("STR_DUPLICATE", ex.Message);
    }

    [Fact]
    public void Build_DuplicateModuleRegistration_ThrowsLocalizationException()
    {
        var ex = Assert.Throws<LocalizationException>(() =>
            new ServiceBuilder().AddLocaleModules(m => m
                .AddModule(typeof(TestLocaleModule))
                .AddModule(typeof(TestLocaleModule))
            )
        );

        Assert.Contains(nameof(TestLocaleModule), ex.Message);
    }

    [Fact]
    public void Build_DefaultProviderIsSingleton_SameInstance()
    {
        var sp = BuildServiceProvider(m => m.AddModule(typeof(TestLocaleModule))
        );

        var provider1 = GetProvider(sp, "default");
        var provider2 = GetProvider(sp, "default");

        Assert.Same(provider1, provider2);
    }

    [Fact]
    public void Indexer_UnknownKey_ThrowsLocalizationException()
    {
        var sp = BuildServiceProvider(m => m.AddModule(typeof(TestLocaleModule))
        );

        var provider = GetProvider(sp, "default");

        var ex = Assert.Throws<LocalizationException>(() =>
            _ = provider["STR_UNKNOWN"]
        );

        Assert.Contains("STR_UNKNOWN", ex.Message);
    }

    [Fact]
    public void Reload_DefaultProvider_ThrowsNotSupportedException()
    {
        var sp = BuildServiceProvider(m => m.AddModule(typeof(TestLocaleModule))
        );

        var provider = GetProvider(sp, "default");

        Assert.Throws<NotSupportedException>(provider.Reload);
    }
}