A minimal, convention-based object mapper that compiles and caches mapping delegates at runtime using Expression Trees.

## Quick Start

```csharp
var mapper = MappingProvider.Instance;

var source = new UserEntity { Id = 1, Name = "Alice", Score = 9.5 };
var dto = mapper.Map<UserEntity, UserDto>(source);
```

## Mapping Modes

The mapper selects a strategy automatically based on the **target type**:

| Target Type | Strategy | How |
|---|---|---|
| `class` with parameterless ctor | **Property mapping** | `new T { Prop = src.Prop }` |
| `record` / `record struct` | **Constructor mapping** | `new T(src.Prop1, src.Prop2)` |
| `class` with `[MapToConstructor]` | **Constructor mapping** | Uses the marked constructor |

### Forcing Constructor Mapping on a Class

```csharp
public class MyDto
{
    public int Id { get; }
    public string Name { get; }

    [MapToConstructor]
    public MyDto(int Id, string Name) { ... }
}
```

## Matching Rules

- Properties are matched **by name** (case-sensitive, `PascalCase`).
- Types must be **implicitly convertible** (exact match, numeric widening, user-defined `implicit` operators).
- No recursive / nested object mapping — use a custom delegate instead.

## Missing Source Properties

When a target property has **no matching source property**:

| Target Property | Behavior |
|---|---|
| `required` | `MappingCompilationException` |
| `NullableField<T>?` | Set to `null` |
| `NullableField<T>` (non-nullable) | `MappingCompilationException` |
| Nullable (`T?`, reference type) | Set to `null` |
| Non-nullable, non-required (property) | Set to `default(T)` |
| Non-nullable (constructor parameter) | `MappingCompilationException` |

## NullableField

A custom wrapper to distinguish "value is absent" from "value is default":

```csharp
public readonly struct NullableField<T>(T? value)
{
    public readonly T? Value = value;
    public static implicit operator NullableField<T>(T? value) => new(value);
}
```

Usage in a DTO:

```csharp
public class UserDto
{
    public int Id { get; set; }
    public NullableField<string>? Nickname { get; set; }  // null if missing in source
}
```

> **Important:** Use `NullableField<T>?` (with `?`). A bare `NullableField<T>` without the nullable annotation will throw if the source property is missing, because a non-nullable struct cannot hold `null`.

## Custom Delegate

Override the mapping for a specific call without affecting the cache:

```csharp
var dto = mapper.Map<UserEntity, UserDto>(source, s => new UserDto
{
    Id = s.Id,
    Name = s.Name.ToUpper()
});
```

## Error Handling

All mapping errors are thrown as `MappingCompilationException` at the **first call** (when the delegate is compiled). Subsequent calls reuse the cached delegate and never throw.

```csharp
try
{
    var dto = mapper.Map<Source, Target>(source);
}
catch (MappingCompilationException ex)
{
    Console.WriteLine(ex.Message);
    // "Mapping Source → Target: required property 'Id' (type 'Int32')
    //  on target has no matching source property"
}
```

## Thread Safety

`MappingProvider.Instance` is a thread-safe singleton backed by `ConcurrentDictionary`. Safe to use from multiple threads.