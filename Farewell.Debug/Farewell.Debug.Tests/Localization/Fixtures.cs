using Farewell.Abstractions.Attributes.Localization;

namespace Farewell.Debug.Tests.Localization;

public static class TestLocaleModule
{
    [DefaultLocale("Test message")]
    public const string TestMessage = "STR_TEST_MESSAGE";

    [DefaultLocale("Hello world")]
    public const string HelloWorld = "STR_HELLO_WORLD";

    [DefaultLocale("Error occurred")]
    public const string ErrorOccurred = "STR_ERROR_OCCURRED";
}

public static class AuthLocaleModule
{
    [DefaultLocale("Login successful")]
    public const string LoginSuccess = "STR_LOGIN_SUCCESS";

    [DefaultLocale("Login failed")]
    public const string LoginFail = "STR_LOGIN_FAIL";

    [DefaultLocale("Logged out")]
    public const string Logout = "STR_LOGOUT";
}

public static class ProfileLocaleModule
{
    [DefaultLocale("Profile updated")]
    public const string ProfileUpdated = "STR_PROFILE_UPDATED";

    [DefaultLocale("Avatar changed")]
    public const string AvatarChanged = "STR_AVATAR_CHANGED";
}

// ♿♿♿ Invalidity team ♿♿♿
public static class ModuleWithoutAttribute
{
    public const string NoAttribute = "STR_NO_ATTRIBUTE";
}

public static class ModuleWithNonConstField
{
    [DefaultLocale("Non const")]
    public static readonly string NonConst = "STR_NON_CONST"; 
}

public static class ModuleWithNonStringField
{
    [DefaultLocale("Non string")]
    public const int NonString = 42;
}

public static class ModuleWithDuplicateKeys
{
    [DefaultLocale("First")]
    public const string First = "STR_DUPLICATE";

    [DefaultLocale("Second")]
    public const string Second = "STR_DUPLICATE";
}