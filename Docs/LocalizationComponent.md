## Overview

A strongly-typed, file-based localization system for .NET applications. The system provides compile-time safe access to localization strings through static module classes, automatic file synchronization, and runtime reloading without application restart.

Key capabilities:
- Compile-time key safety via static module classes with `const string` fields
- Automatic creation and synchronization of locale files against a declared contract
- Runtime reload of locale data without restarting the application
- Thread-safe reads and reloads via `volatile` reference swap on `FrozenDictionary`
- Fallback to default values for untranslated keys
- JSONC output with inline comments showing default values for each key

---

## Quick Start

**1. Declare a locale module**
```csharp
public static class AuthLocaleModule
{
    [DefaultLocale("Login successful")]
    public const string LoginSuccess = "STR_LOGIN_SUCCESS";

    [DefaultLocale("Login failed")]
    public const string LoginFail = "STR_LOGIN_FAIL";
}
```

**2. Register the system**
```csharp
services.AddLocaleModules(modules => modules
    .AddModule<AuthLocaleModule>()
);

services.AddLocalizationProvider("ru-ru");
services.AddLocalizationProvider("en-us");
```

**3. Use in your services**
```csharp
public class AuthService
{
    private readonly ILocalizationProvider _locale;

    public AuthService(
        [FromKeyedServices("ru-ru")] ILocalizationProvider locale)
    {
        _locale = locale;
    }

    public string GetLoginSuccess() => _locale[AuthLocaleModule.LoginSuccess];
}
```

**4. Reload at runtime**
```csharp
provider.Reload();
```

---

## Core Concepts

### Provider Hierarchy

The system is built around two kinds of providers registered as keyed singletons under `ILocalizationProvider`:

```
ILocalizationProvider (keyed: "default")   ← DefaultLocaleProvider
ILocalizationProvider (keyed: "ru-ru")     ← LocaleProvider
ILocalizationProvider (keyed: "en-us")     ← LocaleProvider
```

**DefaultLocaleProvider** is built entirely from reflection over registered module classes. It holds the full set of keys and their default English values declared via `[DefaultLocale]`. It serves as both the authoritative contract and the fallback source for untranslated keys.

**LocaleProvider** owns a single locale file (`locales/{key}.json`). On load it synchronizes the file against the default provider, merges translated values with defaults, and exposes the result through the indexer.

### Locale Modules

A locale module is a `static` class that declares the string contract for a feature area. Each field must satisfy three rules enforced at startup:

- Must be `const`
- Must be of type `string`
- Must carry `[DefaultLocale("...")]`

Violation of any rule throws `LocalizationException` at `BuildServiceProvider`.

```csharp
public static class OrderLocaleModule
{
    [DefaultLocale("Order placed successfully")]
    public const string OrderPlaced = "STR_ORDER_PLACED";

    [DefaultLocale("Order cancelled")]
    public const string OrderCancelled = "STR_ORDER_CANCELLED";
}
```

### Locale Files

Each locale provider owns one JSON file located at:
```
{AppContext.BaseDirectory}/locales/{localeKey}.json
```

The file is a flat key-value map. Each entry is preceded by a comment showing the default value:

```jsonc
{
    // Default: "Login successful"
    "STR_LOGIN_SUCCESS": "Вход выполнен успешно",

    // Default: "Login failed"
    "STR_LOGIN_FAIL": null
}
```

A `null` value means the key is untranslated. The provider substitutes the default value transparently — callers always receive a non-null string.

### File Synchronization

Every time a `LocaleProvider` loads (on startup and on `Reload`), it synchronizes the locale file against the current contract:

| Situation | Action |
|---|---|
| Key in contract, missing in file | Added to file with `null` value |
| Key in file, missing in contract | Removed from file |
| File does not exist | Created with all keys set to `null` |
| File is already in sync | Not rewritten |

Key order in the written file follows the declaration order of fields within each module (`MetadataToken`), and module order follows registration order in `AddLocaleModules`.

---

## Declaring Locale Modules

Modules group related keys by feature area. There is no limit on the number of modules. Each module is registered once and contributes its keys to the shared contract.

```csharp
public static class ProfileLocaleModule
{
    [DefaultLocale("Profile updated")]
    public const string ProfileUpdated = "STR_PROFILE_UPDATED";

    [DefaultLocale("Avatar changed")]
    public const string AvatarChanged = "STR_AVATAR_CHANGED";
}
```

Keys must be globally unique across all registered modules. A duplicate key across two modules throws `LocalizationException` at startup.

---

## Registering Providers

### AddLocaleModules

Registers all locale modules and builds the `DefaultLocaleProvider`. Must be called before `AddLocalizationProvider`.

```csharp
services.AddLocaleModules(modules => modules
    .AddModule<AuthLocaleModule>()
    .AddModule<ProfileLocaleModule>()
    .AddModule<OrderLocaleModule>()
);
```

Registering the same module type twice throws `LocalizationException` immediately at the `AddModule` call.

### AddLocalizationProvider

Registers a locale provider for a specific locale key. The key becomes the DI service key and maps directly to the locale file name.

```csharp
services.AddLocalizationProvider("ru-ru");
services.AddLocalizationProvider("en-us");
services.AddLocalizationProvider("de-de");
```

An empty or whitespace key throws `LocalizationException` immediately.

---

## Accessing Providers

Providers are resolved by locale key from the DI container:

```csharp
// Constructor injection
public class MyService(
    [FromKeyedServices("ru-ru")] ILocalizationProvider locale)
{ }

// Manual resolution
var provider = sp.GetRequiredKeyedService<ILocalizationProvider>("ru-ru");
```

The default provider is accessible under the reserved key `LocalizationConstants.DefaultLocaleKey`:

```csharp
var defaultProvider = sp.GetRequiredKeyedService<ILocalizationProvider>(
    LocalizationConstants.DefaultLocaleKey
);
```

---

## Reading Values

Values are accessed through the indexer. The key is always taken from the corresponding module field — never written as a raw string literal in consuming code:

```csharp
// Correct
string message = provider[AuthLocaleModule.LoginSuccess];

// Avoid — defeats compile-time safety
string message = provider["STR_LOGIN_SUCCESS"];
```

Accessing a key that does not exist in the provider throws `LocalizationException`. This should never happen in practice since all providers are built from the same contract — but will surface immediately if a key is accessed that belongs to an unregistered module.

---

## Reloading

Call `Reload()` on any `LocaleProvider` to re-read its file from disk. The reload:
1. Re-synchronizes the file against the current contract
2. Rebuilds the internal `FrozenDictionary`
3. Swaps the reference atomically via `volatile` write

```csharp
provider.Reload();
```

Reads happening concurrently with a reload always see either the fully old or fully new dictionary — never a partial state.

`Reload()` is not supported on the default provider and throws `NotSupportedException`. The default provider is derived entirely from source code and requires no file.

---

## Key Design Decisions

**`const string` fields as keys**
Using `const string` fields instead of enums or plain string constants gives the best combination of readability, IntelliSense support, refactoring safety, and direct JSON compatibility. The field name documents intent; the field value is the exact JSON key.

**`[DefaultLocale]` as the single source of truth**
Placing the default value directly on the field declaration keeps the contract self-contained. There is no separate default file to maintain. The attribute simultaneously documents, validates, and seeds missing translations.

**`FrozenDictionary` for locale data**
After loading, locale data is never mutated. `FrozenDictionary` is optimized for read-heavy workloads and is the correct structure for immutable lookup tables in .NET 8+.

**`volatile` reference swap on reload**
Rather than locking the dictionary during reload, a new `FrozenDictionary` is built in full and then swapped in via a `volatile` write. This ensures concurrent readers are never blocked and always observe a consistent snapshot.

**File synchronization on every load**
Synchronizing the file against the contract on every startup and reload means locale files are always up to date with the current codebase. Translators never encounter a stale file with missing or obsolete keys.

**Keyed singleton registration**
Using the built-in keyed services feature of `Microsoft.Extensions.DependencyInjection` keeps provider resolution consistent with standard DI patterns. No custom service locator or factory is needed.

**Startup validation**
All contract violations (missing attributes, wrong field types, duplicate keys) are detected at `BuildServiceProvider`. The application fails fast with a descriptive message rather than silently misbehaving at runtime.

---

## Interface Summary

| Interface | Responsibility |
|---|---|
| `ILocalizationProvider` | Public contract for reading localized strings by key and triggering a reload |
| `IDefaultLocaleProvider` *(internal)* | Extends `ILocalizationProvider` with access to the full frozen data set and ordered key list, used internally by `LocaleProvider` during file synchronization and writing |
| `ILocaleModulesProvider` *(internal)* | Carries the list of registered module types from the DI registration phase to `DefaultLocaleProvider` at construction time |

---

## Extension Points

| What to extend | How |
|---|---|
| Add a new feature area | Create a new `static` class with `const string` fields annotated with `[DefaultLocale]`, register it with `.AddModule<T>()` |
| Add a new language | Call `services.AddLocalizationProvider("xx-xx")` — the file is created automatically with all keys on first run |
| Add new keys to an existing module | Add `const string` fields with `[DefaultLocale]` to the module class — all existing locale files gain the new keys automatically on next startup or reload |
| Remove obsolete keys | Remove the field from the module class — the key is dropped from all locale files on next startup or reload |
| Trigger translation refresh | Call `Reload()` on the target `ILocalizationProvider` instance — can be wired to an HTTP endpoint, a file watcher, a CLI command, or any other trigger |