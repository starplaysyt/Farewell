## Overview

A lightweight, thread-safe configuration library for .NET that loads structured data from files and exposes it as strongly-typed C# objects through a clean API. Designed to be used as a singleton registered in a DI container, the system parses configuration files into an internal JSON tree on startup and provides both primitive value access and full section deserialization on demand.

Key capabilities:
- Load configuration from **JSON** files (YAML and others via extensions)
- Access values by **colon-separated path** (`"database:connection:host"`)
- Deserialize entire **configuration sections** into typed classes
- **Cache** all resolved values — no repeated parsing or reflection
- **Hot reload** support — swap configuration at runtime without downtime
- Optional **file watching** — automatic reload on file change
- **Keyed DI registration** — multiple independent providers, one per file

---

## Quick Start

**1. Define a configuration class**
```csharp
[ConfigSection("database")]
public class DatabaseConfig
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

**2. Register in DI**
```csharp
services
    .AddConfigurationProvider(
        key: "appsettings",
        parser: new JsonConfigParser("appsettings.json"),
        watchFile: true
    )
    .AddConfigurationSection<DatabaseConfig>("appsettings");
```

**3. Use in your services**
```csharp
// Inject a typed configuration class directly
public class MyService
{
    public MyService(DatabaseConfig config)
    {
        Console.WriteLine(config.Host);
    }
}

// Or work with the provider directly
public class MyService
{
    public MyService([FromKeyedServices("appsettings")] IConfigurationProvider config)
    {
        var port = config.Get<int>("server:port");
        var db = config.GetSection<DatabaseConfig>();
    }
}
```

---

## Core Concepts

### Architecture Overview

```
JSON Files
    ↓
IConfigParser  ──────────────────────────────────────────┐
    ↓                                                     │
JsonElement (internal tree)             IWatchableConfigParser
    ↓                                  (optional file watching)
ConfigurationProvider (singleton)
    ├── volatile JsonElement _root       ← atomic swap on reload
    ├── volatile ConcurrentDictionary    ← invalidated on reload
    └── static path split cache         ← shared across providers
         ↓
    DI Container
    ├── Keyed IConfigurationProvider     ← direct provider access
    └── Typed configuration classes      ← factory → provider → section
```

### Internal Tree

Configuration files are parsed into a `JsonElement` tree using `System.Text.Json`. This representation is format-agnostic — regardless of the source file format, everything is normalized into the same JSON tree internally. Navigation through the tree is performed by splitting the colon-separated path and traversing properties level by level.

### Caching Strategy

The system uses two levels of caching:

| Cache | Scope | Lifetime |
|---|---|---|
| Resolved values (`ConcurrentDictionary`) | Per provider instance | Invalidated on `Reload()` |
| Path segments (`string[]`) | Static, shared | Application lifetime |
| Attribute paths (`Type → string`) | Static, shared | Application lifetime |

### Thread Safety

`_root` and `_cache` are both `volatile`, ensuring atomic reference swaps visible across threads. Concurrent reads are lock-free. On reload, a new root and a new empty cache are built independently and then swapped in — readers continue using the old root until the swap completes.

---

## Configuration Files

### JSON

Standard JSON files are supported out of the box. Keys are matched case-insensitively during deserialization.

```json
{
    "server": {
        "host": "localhost",
        "port": 8080
    },
    "database": {
        "host": "db.local",
        "port": 5432,
        "name": "mydb"
    }
}
```

```csharp
var parser = new JsonConfigParser("appsettings.json");
var provider = new ConfigurationProvider(parser);
```

---

## Accessing Configuration Values

### Primitive values — `Get<T>`

Use `Get<T>` to retrieve a single primitive value by path. `T` must implement `IParsable<T>`.

```csharp
var host = provider.Get<string>("server:host");   // "localhost"
var port = provider.Get<int>("server:port");       // 8080
var ratio = provider.Get<double>("app:threshold"); // 0.95
```

Throws `ConfigKeyNotFoundException` if the path does not exist.

---

### Configuration sections — `GetSection<T>`

Use `GetSection<T>` to deserialize an entire subtree into a typed class.

**By explicit path:**
```csharp
var db = provider.GetSection<DatabaseConfig>("database");
Console.WriteLine(db.Host); // "db.local"
```

**By `[ConfigSection]` attribute:**
```csharp
[ConfigSection("database")]
public class DatabaseConfig
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
}

var db = provider.GetSection<DatabaseConfig>(); // path resolved from attribute
```

Throws `ConfigSectionAttributeMissingException` if the attribute is missing when using the no-path overload.

---

### DI — typed class injection

When registered via `AddConfigurationSection<T>`, the typed class can be injected directly without referencing the provider:

```csharp
services.AddConfigurationSection<DatabaseConfig>("appsettings");

// In your service
public class Repository
{
    public Repository(DatabaseConfig config) { ... }
}
```

---

### DI — keyed provider access

When you need ad-hoc access to arbitrary values without a typed class:

```csharp
public class MyService
{
    private readonly IConfigurationProvider _config;

    public MyService([FromKeyedServices("appsettings")] IConfigurationProvider config)
    {
        _config = config;
    }

    public void DoWork()
    {
        var timeout = _config.Get<int>("app:timeout");
    }
}
```

---

## Hot Reload

### Manual reload

Call `Reload()` explicitly at any point. The old configuration remains available to all readers until the new tree is fully built and swapped in:

```csharp
provider.Reload();
```

### File watching

Enable automatic reload when the file changes by passing `watchFile: true` during registration. A debounce of 300ms is applied to prevent multiple rapid reloads on a single save:

```csharp
services.AddConfigurationProvider(
    key: "appsettings",
    parser: new JsonConfigParser("appsettings.json"),
    watchFile: true  // triggers Reload() automatically on file change
);
```

File watching requires the parser to implement `IWatchableConfigParser`. If `watchFile: true` is passed with a parser that does not implement it, an `InvalidOperationException` is thrown at startup.

---

## Key Design Decisions

**`JsonElement` as internal tree**
`System.Text.Json` is part of the BCL — no additional dependencies required. The internal tree is format-agnostic; any file format can be converted to `JsonElement` before being handed to the provider. `RootElement.Clone()` ensures the element is independent of the originating `JsonDocument` and safe to hold long-term.

**Atomic swap on reload**
Both `_root` and `_cache` are `volatile` references. Reload builds a new tree and a new empty cache independently, then swaps both references atomically. Readers in flight continue using the old root — no locks, no downtime, no inconsistent state.

**Keyed providers over a single merged provider**
Each file gets its own independent provider singleton registered with a DI key. This avoids key collision between files, allows independent reload cycles per file, and keeps provider responsibilities clearly scoped.

**Two-level get API**
`Get<T>` and `GetSection<T>` are intentionally separate methods with different generic constraints rather than a single method resolved at runtime. This makes intent explicit at the call site and avoids runtime type inspection.

**Static caches for reflection and path splitting**
Attribute paths and split path segments are cached in static `ConcurrentDictionary` instances shared across all providers. Both are computed once and never change — the static scope is intentional and correct.

---

## Interface Summary

| Interface | Responsibility |
|---|---|
| `IConfigurationProvider` | Main consumer-facing contract. Exposes `Get<T>`, `GetSection<T>` and `Reload()` |
| `IConfigParser` | Abstracts file reading and parsing. Returns a `JsonElement` root |
| `IWatchableConfigParser` | Extends `IConfigParser` with a `FilePath` property, enabling file watching in the provider |

---

## Extension Points

| What to extend | How |
|---|---|
| Add a new file format (e.g. YAML, TOML, INI) | Implement `IConfigParser` (and optionally `IWatchableConfigParser`), convert your format to `JsonElement` internally |
| Add new DI registration helpers | Add extension methods on `IServiceCollection` following the pattern in `ConfigurationProviderExtensions` |
| Custom reload triggers (e.g. remote config, environment variable changes) | Obtain a reference to `IConfigurationProvider` and call `Reload()` from any trigger — HTTP endpoint, background service, signal handler, etc. |
| Custom deserialization behavior | Modify or extend `JsonSerializerOptions` used in `GetSection<T>` — add converters, naming policies, etc. |