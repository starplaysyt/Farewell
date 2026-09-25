## Overview

Configuration System is a lightweight, extensible library for managing application configuration files in .NET. It provides a strongly-typed, tree-structured approach to configuration — each configuration file is represented as a plain C# class that is serialized and deserialized automatically.

The system is built around the DI container and follows a provider-per-file model: each configuration file gets its own singleton `IConfigurationProvider<T>`, resolved by generic type. Multiple file formats are supported through a pluggable resolver mechanism, with JSON support built in via BCL with no additional dependencies.

---

## Quick Start

```csharp
// 1. Declare your configuration class with defaults
public sealed class AppConfig
{
    public string ApplicationName { get; set; } = "MyApp";
    public int MaxConnections { get; set; } = 100;
    public LoggingConfig Logging { get; set; } = new();
}

public sealed class LoggingConfig
{
    public string Level { get; set; } = "Information";
    public bool WriteToFile { get; set; } = false;
}

// 2. Register the system
ServiceBuilder builder = new ServiceBuilder();

builder
    .AddJsonConfigResolver()
    .AddConfig<AppConfig>("/etc/myapp/app.json")
    .AddConfig<DatabaseConfig>("/etc/myapp/database.json");

IServiceProvider provider = builder.Build();

// 3. Consume in your service
public sealed class MyService
{
    private readonly IConfigurationProvider<AppConfig> _config;

    public MyService(IConfigurationProvider<AppConfig> config)
    {
        _config = config;
    }

    public void Run()
    {
        var config = _config.Get();
        Console.WriteLine(config.ApplicationName);
    }
}
```

---

## Core Concepts

### Provider-per-file model

Each configuration file is backed by a dedicated `IConfigurationProvider<T>` singleton. The generic parameter `T` uniquely identifies the provider in the DI container — no keyed services or string identifiers are needed on the consumer side.

```
IConfigurationProvider<AppConfig>      →  app.json
IConfigurationProvider<DatabaseConfig> →  database.json
IConfigurationProvider<NetworkConfig>  →  network.yml
```

### Configuration tree

A configuration is a plain C# class — a POCO with no required base classes or attributes. Nesting is supported naturally through object composition. Default values are declared directly on the class properties.

```csharp
public sealed class AppConfig          // root
{
    public string Name { get; set; } = "MyApp";
    public DatabaseConfig Database { get; set; } = new();  // branch
    public List<EndpointConfig> Endpoints { get; set; } = new();  // collection
}
```

### Resolver-per-format model

File format handling is decoupled from the provider. An `IConfigResolver` is responsible for reading and writing a specific file format. Resolvers are registered as keyed singletons in the DI container, where the key is the file extension.

```
".json"  →  JsonConfigResolver
".yml"   →  YamlConfigResolver   (external package)
".toml"  →  TomlConfigResolver   (custom)
```

### Eager initialization

All singletons — including configuration providers — are constructed at `Build()` time. This means configuration errors (missing resolvers, invalid file content) surface immediately at startup rather than at runtime when a service first requests the config.

### File lifecycle

```
AddConfig<T>(path) called
    │
    └── at Build()
            │
            ├── file exists → resolver reads and deserializes → cached in provider
            │       └── invalid content → ConfigurationResolvingException ❌
            │
            └── file not found → new T() with defaults → resolver writes to disk → cached
                    └── directory created automatically if missing
```

---

## Configuration Classes

Configuration classes are plain C# classes. There are no required base classes, interfaces, or attributes. The only requirement is a parameterless constructor, which is used to generate default values when a config file does not yet exist.

```csharp
public sealed class AppConfig
{
    public string ApplicationName { get; set; } = "MyApp";
    public int MaxConnections { get; set; } = 100;
    public NestedConfig Nested { get; set; } = new();
}
```

Nesting depth is unlimited. Collections are supported as long as the underlying serializer handles them.

---

## Registering Resolvers

A resolver must be registered before any config file using that format can be added. The system ships with a built-in JSON resolver.

```csharp
// Built-in JSON support
builder.AddJsonConfigResolver();

// Custom resolver for any format
builder.AddConfigResolver<MyTomlResolver>(".toml");

// External YAML package (example)
builder.AddYamlConfigResolver();
```

`AddConfigResolver<TResolver>(string fileExtension)` is the universal registration method. It registers `TResolver` as a keyed singleton under the given extension. The same resolver type can be registered under multiple extensions:

```csharp
builder.AddConfigResolver<YamlConfigResolver>(".yml");
builder.AddConfigResolver<YamlConfigResolver>(".yaml");
```

---

## Registering Configuration Files

```csharp
// Absolute path
builder.AddConfig<AppConfig>("/etc/myapp/app.json");

// Relative to application base directory
builder.AddConfig<AppConfig>("config/app.json", relativeToBaseDirectory: true);
```

If the specified file does not exist, it will be created with the default values from `new T()`. The directory structure will also be created if it does not exist. If the file exists but cannot be parsed, a `ConfigurationResolvingException` is thrown at `Build()` time.

If no resolver is registered for the file's extension, a `ConfigurationResolverNotFoundException` is thrown immediately with a message indicating which extension is missing and how to register a resolver for it.

---

## Consuming Configuration

Configuration is consumed by injecting `IConfigurationProvider<T>` into any service. Since it is a singleton, it is safe to hold a reference for the lifetime of the application.

```csharp
public sealed class MyService
{
    private readonly IConfigurationProvider<AppConfig> _config;

    public MyService(IConfigurationProvider<AppConfig> config)
    {
        _config = config;
    }

    public void DoWork()
    {
        // Always returns the current cached value
        AppConfig config = _config.Get();
    }
}
```

`Get()` is a direct field read with no I/O — it returns the cached root object.

---

## Reloading Configuration

The provider exposes a `Reload()` method that re-reads the file from disk and updates the internal cache. The reload follows the same logic as the initial load.

```csharp
IConfigurationProvider<AppConfig> provider = ...;

provider.Reload();
AppConfig fresh = provider.Get();
```

If the file is invalid at reload time, `ConfigurationResolvingException` is thrown and **the previous cached value is preserved**. This means the application continues running with the last known good configuration.

`Get()` is thread-safe by virtue of `volatile` field semantics. Reference replacement is atomic, and `volatile` ensures visibility across cores. No locking is performed.

---

## Implementing a Custom Resolver

To support a new file format, implement `IConfigResolver` and register it with the appropriate extension.

```csharp
public sealed class TomlConfigResolver : IConfigResolver
{
    public T Resolve<T>(string absolutePath) where T : class
    {
        var content = File.ReadAllText(absolutePath);
        return TomlSerializer.Deserialize<T>(content)
            ?? throw new InvalidOperationException("Deserialization returned null.");
    }

    public void SaveDefaults<T>(string absolutePath, T instance) where T : class
    {
        var content = TomlSerializer.Serialize(instance);
        File.WriteAllText(absolutePath, content);
    }
}

// Registration
builder.AddConfigResolver<TomlConfigResolver>(".toml");
```

`Resolve<T>` is responsible for reading the file and returning a fully deserialized object. Any exception thrown from `Resolve<T>` that is not already a `ConfigurationException` will be wrapped in `ConfigurationResolvingException` by the provider.

`SaveDefaults<T>` is called only when a config file does not exist. It receives a `new T()` instance populated with declared defaults and is responsible for persisting it in the correct format.

---

## Validation Integration

If `IValidationProvider` is registered in the DI container, the configuration system will automatically validate the deserialized object after every load and reload. No additional setup is required — the provider resolves `IValidationProvider` as an optional dependency.

```csharp
// Validation runs automatically if this is registered
builder.AddSingleton<IValidationProvider, MyValidationProvider>();
builder.AddJsonConfigResolver();
builder.AddConfig<AppConfig>("app.json");
```

If validation fails, a `ConfigurationValidationException` is thrown containing the full `ValidationReport`. Default values written to a newly created file are **not validated** — defaults are considered trusted by design.

---

## Key Design Decisions

**Generic provider as DI key**
`IConfigurationProvider<T>` is unique per type `T`. This eliminates the need for string-based keyed service resolution on the consumer side, which avoids stringly-typed identifiers and makes misconfiguration a compile-time issue rather than a runtime one.

**Resolvers as keyed singletons by file extension**
Resolvers are stateless by design — they open a file, parse it, and return. Registering them as keyed singletons by extension keeps format selection implicit (driven by the file path) while making it fully explicit in the registration. No resolver registry object is needed.

**Eager initialization**
Fail-fast at startup is preferable to silent failure at runtime. Building the DI container with broken configuration should crash the process immediately with a clear error, not surface as a `NullReferenceException` minutes later in production.

**Defaults are not validated**
Default values are declared by the developer in the class definition and are considered correct by contract. Validating them would couple the configuration system's startup behavior to the validation rules, making it impossible to start with intentionally partial defaults.

**`volatile` without locking on reload**
Replacing the cached reference is atomic on 64-bit runtimes. `volatile` provides the necessary memory visibility guarantee across cores. A full lock would add overhead to every `Get()` call — which is a hot path — for a scenario (concurrent reload) that is rare and non-critical in most applications.

**No merge, no layering**
Configuration files are fully isolated. Each file maps to exactly one type and one provider. This removes an entire class of bugs related to override order, partial merges, and unexpected value sources.

---

## Interface Summary

| Interface | Responsibility |
|---|---|
| `IConfigurationProvider<T>` | Owns the lifecycle of the root configuration object of type `T`. Provides access via `Get()` and supports explicit cache refresh via `Reload()`. |
| `IConfigResolver` | Handles file format–specific reading (`Resolve<T>`) and writing (`SaveDefaults<T>`). Stateless by design. |
| `IValidationProvider` | Optional dependency. Validates a deserialized configuration object and returns a `ValidationReport`. Consumed by the provider after every successful resolve. |

---

## Extension Points

| What to extend | How |
|---|---|
| Add a new file format | Implement `IConfigResolver`, register via `builder.AddConfigResolver<TResolver>(".ext")` |
| Support multiple extensions for one format | Call `AddConfigResolver<TResolver>` multiple times with different extensions |
| Enable validation | Register `IValidationProvider` in the DI container — the provider picks it up automatically |
| Package a resolver for distribution | Create a separate NuGet package, expose an extension method `AddMyFormatConfigResolver()` that calls `AddConfigResolver<T>` internally |