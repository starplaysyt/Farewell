## Overview

A lightweight, zero-reflection validation library for .NET. Designed around three principles:

- **Compile once** — validation logic is compiled into delegates on first use and cached indefinitely
- **Pure data results** — validation results carry only codes and property names, no formatting or localization
- **Separation of concerns** — entities know *that* they are validated, not *how*

---

## Quick Start

### 1. Define your entity and attach a validator

```csharp
[ValidatedBy(typeof(UserValidator))]
public class User
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public int Age { get; set; }
}
```

### 2. Implement the validator

```csharp
public sealed class UserValidator : FluentValidator<User>
{
    protected override void DefineRules(PropertiesValidatorBuilder<User> builder)
    {
        builder.Rule(u => u.Name)
            .NotEmpty()
            .Length(2, 50);

        builder.Rule(u => u.Email)
            .NotEmpty()
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        builder.Rule(u => u.Age)
            .InRange(0, 150);
    }
}
```

### 3. Validate

```csharp
// Obtain IValidationProvider from DI
var report = provider.ValidateAll(user);

if (!report.IsSuccess)
{
    foreach (var error in report.Errors!)
        Console.WriteLine($"{error.PropertyName}: {error.Code}");
}

// Stop at first failure
var status = provider.ValidateBreak(user);
if (!status.IsSuccess)
    Console.WriteLine($"Failed: {status.PropertyName} — {status.Code}");
```

---

## Core Concepts

### ValidationCode

All validation results are expressed as enum codes. No strings at the validation layer — localization is a concern of the presentation layer.

```csharp
public enum ValidationCode
{
    Ok,
    MultipleErrors,
    NotEmpty,
    MinLength,
    MaxLength,
    OutOfRange,
    InvalidFormat
}
```

### ValidationStatus

Represents the result of a single rule evaluation.

```csharp
// Success
var ok = ValidationStatus.Ok;

// Failure
var error = ValidationStatus.Error(ValidationCode.NotEmpty, "Name");

// Inspect
bool passed = status.IsSuccess;
string field = status.PropertyName;
ValidationCode code = status.Code;
```

### ValidationReport

Represents the result of all rules evaluated against an instance.

```csharp
report.IsSuccess        // true if no errors
report.Status           // Ok | <specific code> | MultipleErrors
report.Errors           // null if Ok, otherwise IReadOnlyList<ValidationStatus>
```

Status resolution:

| Errors count | Report.Status |
|---|---|
| 0 | `ValidationCode.Ok` |
| 1 | The error's own code |
| 2+ | `ValidationCode.MultipleErrors` |

### ValidateAll vs ValidateBreak

```csharp
// Evaluates every rule, collects all failures
ValidationReport report = provider.ValidateAll(user);

// Evaluates rules sequentially, stops at first failure
ValidationStatus status = provider.ValidateBreak(user);
```

Use `ValidateAll` when you need to display all errors to the user.
Use `ValidateBreak` when you need a fast pass/fail check, e.g. in a pipeline gate.

---

## Fluent API

### Basic rules

```csharp
builder.Rule(u => u.Name)
    .NotEmpty()          // rejects null, empty, whitespace
    .MinLength(2)        // rejects if length < 2
    .MaxLength(50)       // rejects if length > 50
    .Length(2, 50);      // shorthand for MinLength + MaxLength

builder.Rule(u => u.Email)
    .NotEmpty()
    .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");  // regex match

builder.Rule(u => u.Age)
    .InRange(0, 150);    // inclusive range for any IComparable<T>
```

### Conditional rules — rule level

`When` and `Unless` skip the entire rule based on a condition evaluated against the instance:

```csharp
builder.Rule(u => u.ManagerId)
    .NotEmpty()
    .When(u => u.Role == Role.Manager);      // only validate if condition is true

builder.Rule(u => u.Password)
    .MinLength(8)
    .Unless(u => u.IsOAuthUser);             // skip if condition is true
```

### Conditional rules — constraint group level

When only a subset of constraints should be conditional, use the grouped `When` overload.
The inner builder `ConstraintGroupBuilder` intentionally does not support nested `When` — this is enforced by the type system:

```csharp
builder.Rule(u => u.Phone)
    .When(u => u.HasPhone, r => r           // r is ConstraintGroupBuilder, not PropertyRuleBuilder
        .NotEmpty()
        .Matches(@"^\+7\d{10}$"));
```

### Inline constraints

For one-off checks that don't warrant a dedicated constraint class:

```csharp
builder.Rule(u => u.Username)
    .Must(name => !name!.Contains(' '), ValidationCode.InvalidFormat);
```

### Custom constraints

Implement `IValidationConstraint<TValue>` and pass via `Must`:

```csharp
public sealed class UniqueUsernameConstraint : IValidationConstraint<string?>
{
    public ValidationCode Code => ValidationCode.InvalidFormat;

    public bool Check(string? value)
        => !ReservedNames.Contains(value);  // your logic here
}

builder.Rule(u => u.Username)
    .Must(new UniqueUsernameConstraint());
```

---

## Validation Contexts

A single entity can have multiple validators attached, each for a different context:

```csharp
[ValidatedBy(typeof(UserValidator))]                    // context: "Default"
[ValidatedBy(typeof(UserCreateValidator), "Create")]    // context: "Create"
[ValidatedBy(typeof(UserUpdateValidator), "Update")]    // context: "Update"
public class User { ... }
```

Invoke with a specific context:

```csharp
provider.ValidateAll(user);                  // uses "Default"
provider.ValidateAll(user, "Create");        // uses "Create"
provider.ValidateBreak(user, "Update");      // uses "Update"
```

Rules:
- One validator per context per type — registering two validators for the same context throws `InvalidOperationException` at resolve time
- Unknown context with no registered validator returns `ValidationReport.Ok` / `ValidationStatus.Ok` silently

---

## Direct Validators

When fluent rules are insufficient — cross-entity checks, database lookups, complex branching — implement `IDirectValidator<T>` directly:

```csharp
public sealed class UserExistsValidator : IDirectValidator<User>
{
    private readonly IUserRepository _repo;

    public UserExistsValidator(IUserRepository repo) => _repo = repo;

    public ValidationReport ValidateAll(User instance)
    {
        if (_repo.ExistsByEmail(instance.Email))
            return new ValidationReport(new[]
            {
                ValidationStatus.Error(ValidationCode.InvalidFormat, nameof(User.Email))
            });

        return ValidationReport.Ok;
    }

    public ValidationStatus ValidateBreak(User instance)
    {
        if (_repo.ExistsByEmail(instance.Email))
            return ValidationStatus.Error(ValidationCode.InvalidFormat, nameof(User.Email));

        return ValidationStatus.Ok;
    }
}
```

`IDirectValidator<T>` and `IFluentValidator<T>` both extend `IValidator<T>`. The provider handles both transparently. Direct validators are stored separately and are not subject to cache invalidation.

---

## Localization

Validation results carry only `ValidationCode` and `PropertyName`. Translation to human-readable strings is the responsibility of the consumer, typically at the presentation layer:

```csharp
public sealed class RussianMessageResolver : IValidationMessageResolver
{
    public string Resolve(ValidationCode code, string propertyName)
        => code switch
        {
            ValidationCode.NotEmpty      => $"Поле «{propertyName}» не должно быть пустым",
            ValidationCode.MinLength     => $"Поле «{propertyName}» слишком короткое",
            ValidationCode.MaxLength     => $"Поле «{propertyName}» слишком длинное",
            ValidationCode.OutOfRange    => $"Значение поля «{propertyName}» вне допустимого диапазона",
            ValidationCode.InvalidFormat => $"Поле «{propertyName}» имеет неверный формат",
            _                            => $"Ошибка в поле «{propertyName}»"
        };
}
```

---

## Cache Invalidation

Compiled validators are cached forever by default. If your application reloads configuration at runtime and validators capture config values, invalidate the cache explicitly:

```csharp
// Cast to concrete type or expose via interface
((ValidationProvider)provider).InvalidateCache();
```

Direct validators are never invalidated — they hold no compiled state.

---

---

# Architecture & Internals

## Layer diagram

```
┌─────────────────────────────────────────────────────┐
│                   Consumer code                     │
│         provider.ValidateAll(user, "Create")        │
└────────────────────────┬────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────┐
│               IValidationProvider                   │
│               ValidationProvider                    │
│                                                     │
│   _fluentCache: ConcurrentDictionary                │
│   _directCache: ConcurrentDictionary                │
│   _resolver:    IValidatorResolver                  │
└──────────┬─────────────────────────┬────────────────┘
           │                         │
┌──────────▼──────────┐   ┌──────────▼──────────────┐
│  IFluentValidator   │   │   IDirectValidator       │
│  FluentValidator<T> │   │   (user implementation) │
│                     │   └─────────────────────────┘
│  GetOrCompile()     │
└──────────┬──────────┘
           │
┌──────────▼──────────────────────────────────────────┐
│              ValidatorCompiler                      │
│                                                     │
│  Compile(Func<T, ValidationStatus?>[])              │
│    → CompiledValidator<T>                           │
│        validateAll:   Func<T, ValidationReport>     │
│        validateBreak: Func<T, ValidationStatus>     │
└─────────────────────────────────────────────────────┘
```

---

## Compilation pipeline

This is the most important internal process. It runs **once per (Type, Context) pair** and produces a set of closed-over delegates with no further reflection or allocation.

### Step 1 — Attribute resolution

```
typeof(User)
  .GetCustomAttributes<ValidatedByAttribute>()
  .Where(a => a.Context == context)
  → ValidatorType = typeof(UserValidator)
  → cached in _attributeCache
```

### Step 2 — Validator instantiation

```
Activator.CreateInstance(typeof(UserValidator))
  → UserValidator instance
```

### Step 3 — DefineRules execution

```
UserValidator.DefineRules(PropertiesValidatorBuilder<User>)
  → builder accumulates List<IPropertyRuleBuilder<User>>
  → each entry is a PropertyRuleBuilder<User, TValue>
     holding:
       - Func<User, TValue>         selector    (compiled from Expression once)
       - IValidationConstraint[]    constraints
       - Func<User, bool>?          when
       - Func<User, bool>?          unless
```

### Step 4 — Evaluator construction

Each `PropertyRuleBuilder<T, TValue>` produces a `Func<T, ValidationStatus?>` via `BuildEvaluator()`.
All values are captured in closures at this point — no reflection ever runs again:

```
PropertyRuleBuilder<User, string?>.BuildEvaluator()

  captures:
    selector     = u => u.Name          (Func<User, string?>)
    propertyName = "Name"               (string)
    when         = null
    unless       = null
    checks[]     = [
      (instance, value) => NotEmptyConstraint.Check(value)
                           ? null
                           : ValidationStatus.Error(NotEmpty, "Name"),
      (instance, value) => MinLengthConstraint.Check(value)
                           ? null
                           : ValidationStatus.Error(MinLength, "Name"),
      (instance, value) => MaxLengthConstraint.Check(value)
                           ? null
                           : ValidationStatus.Error(MaxLength, "Name")
    ]

  returns:
    (User instance) =>
    {
        var value = selector(instance);
        foreach (var check in checks)
        {
            var status = check(instance, value);
            if (status is not null) return status;
        }
        return null;
    }
```

### Step 5 — ValidatorCompiler assembles final delegates

```
evaluators = [ eval_Name, eval_Email, eval_Age, eval_Phone ]

validateAll = (User instance) =>
{
    List<ValidationStatus>? errors = null;
    foreach (var eval in evaluators)
    {
        var status = eval(instance);
        if (status is not null)
        {
            errors ??= new();
            errors.Add(status);
        }
    }
    return errors is null
        ? ValidationReport.Ok
        : new ValidationReport(errors.ToArray());
}

validateBreak = (User instance) =>
{
    foreach (var eval in evaluators)
    {
        var status = eval(instance);
        if (status is not null) return status;
    }
    return ValidationStatus.Ok;
}
```

### Step 6 — Caching

```
CompiledValidator<User> stored in _fluentCache[(typeof(User), "Default")]

All subsequent calls to provider.ValidateAll(user) reach:
  ConcurrentDictionary.TryGetValue → hit → direct delegate invocation
  Zero reflection. Zero allocations beyond ValidationReport on failure.
```

---

## Key design decisions

### Why attribute-based resolver instead of DI registration

Keeps validator co-located with the entity. The entity declares *what* validates it, not *how* it is wired. The resolver is swappable — implement `IValidatorResolver` to use a DI container, a manual registry, or a convention-based scanner.

### Why separate caches for fluent and direct validators

Fluent validators produce compiled state that must be invalidated when configuration changes. Direct validators are stateful by design — the consumer controls their lifecycle. Mixing them in one cache would either over-invalidate or under-invalidate.

### Why ConstraintGroupBuilder cannot nest When

Enforced at the type level, not by convention. `PropertyRuleBuilder<T, TValue>.When(condition, group)` passes a `ConstraintGroupBuilder<T, TValue>` to the lambda — a type that has no `When` method. Recursive conditional groups are therefore a compile-time error, not a runtime surprise.

### Why ValidationStatus carries no message

Messages are culture-dependent, context-dependent, and change independently of validation logic. Keeping results as pure codes makes the validation layer fully decoupled from presentation. The same `ValidationCode.MinLength` can render as an API error code, a localized string, or a log entry depending on where it is consumed.

### Why ValidateBreak returns ValidationStatus not ValidationReport

`ValidateBreak` is a fast-path operation — it answers "is this instance valid enough to proceed?" A single status is sufficient and avoids allocating a report wrapper and an array when all you need is one answer.

---

## Interface summary

| Interface | Responsibility |
|---|---|
| `IValidator<T>` | Common contract — `ValidateAll` + `ValidateBreak` |
| `IFluentValidator<T>` | Marker — implemented via `FluentValidator<T>` base class |
| `IDirectValidator<T>` | Marker — implemented manually by consumer |
| `IValidationProvider` | Entry point — resolves and invokes validators |
| `IValidatorResolver` | Locates validator type for a given `(Type, context)` pair |
| `IValidationConstraint<TValue>` | Single atomic check on a value |
| `IValidationMessageResolver` | Translates `ValidationCode` to human-readable string |

---

## Extension points

| What to extend | How |
|---|---|
| New constraint | Implement `IValidationConstraint<TValue>`, use via `.Must()` |
| New fluent method | Extension method on `PropertyRuleBuilder<T, TValue>` |
| Custom resolver | Implement `IValidatorResolver` |
| Localization | Implement `IValidationMessageResolver` |
| Direct validation | Implement `IDirectValidator<T>` |
| Pipeline integration | Call `provider.ValidateBreak(payload)` as a pipeline step |