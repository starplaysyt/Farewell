# Farewell.Validation

Async-first, DI-native fluent validation for .NET.

## Setup

```csharp
services.AddValidation();
services.AddValidators(typeof(Program).Assembly);
```

## Quick Start

```csharp
public sealed class CreateUserValidator : FluentValidator<CreateUserCommand>
{
    protected override void DefineRules(IConstraintBuilder<CreateUserCommand> b)
    {
        b.Property(x => x.Name)
            .NotEmpty()
            .MinLength(3)
            .MaxLength(100);

        b.Property(x => x.Email)
            .NotEmpty()
            .Email();

        b.Property(x => x.Age)
            .Positive()
            .InRange(0, 150);
    }
}
```

Inject and use:

```csharp
public sealed class CreateUserHandler(IValidationProvider validator)
{
    public async Task Handle(CreateUserCommand cmd, CancellationToken ct)
    {
        var report = await validator.ValidateAsync(cmd, ct: ct);
        if (!report.IsSuccess)
            throw new ValidationException(report);
    }
}
```

## Validation Contexts

Multiple validators per entity, selected by string key:

```csharp
[ValidationContext("Strict")]
public sealed class StrictUserValidator : FluentValidator<User> { ... }

// Usage
var report = await validator.ValidateAsync(user, context: "Strict");
```

No attribute = `"Default"`.

## Conditional Validation

Pass `when` to `Property()` — checks run only when condition is true:

```csharp
b.Property(x => x.CompanyName, x => x.IsBusinessAccount)
    .NotEmpty()
    .MinLength(2);
```

Each `Property()` call is independent. Multiple calls for the same property with different conditions are allowed.

## Custom Checks

Three overloads — pick the simplest one that fits:

```csharp
// Pure sync
b.Property(x => x.Code)
    .AddCheck(ValidationCode.InvalidFormat, v => v.StartsWith("X-"));

// Sync + IServiceProvider
b.Property(x => x.Role)
    .AddCheck(ValidationCode.Forbidden, (v, sp) =>
        sp.GetRequiredService<IRolePolicy>().IsAllowed(v));

// Full async
b.Property(x => x.Username)
    .AddCheck(ValidationCode.NotUnique, async (v, sp, ct) =>
        !await sp.GetRequiredService<IUserRepo>().ExistsAsync(v, ct));
```

## Direct Validators

For validators that need constructor-injected dependencies:

```csharp
[ValidationContext("Direct")]
public sealed class UserDirectValidator(IUserRepository repo) : IDirectValidator<User>
{
    public async ValueTask<ValidationReport> ValidateAsync(
        User instance, IServiceProvider sp,
        IValidationCollector collector, CancellationToken ct)
    {
        if (await repo.EmailExistsAsync(instance.Email, ct))
            collector.Collect(ValidationStatus.Error(ValidationCode.NotUnique, "Email"));

        return collector.ToReport();
    }
}
```

`IDirectValidator<T>` is **Scoped** (can depend on DbContext, repositories).
`FluentValidator<T>` is **Singleton** (stateless, compiled once).

Both are resolved as `IAsyncValidator<T>` — the provider doesn't distinguish them.

## Collect All vs Break on First

```csharp
// Collect every error (default)
var report = await validator.ValidateAsync(user);

// Stop at first error
var report = await validator.ValidateAsync(user, breakOnFirst: true);
```

## Sync Wrapper

For non-async call sites:

```csharp
var report = validator.Validate(user);
```

Uses `.ConfigureAwait(false).GetAwaiter().GetResult()` internally.

## Built-in Constraints

| Extension | Types | Code |
|---|---|---|
| `.NotEmpty()` | `string?` | 100 |
| `.MinLength(n)` | `string?` | 200 |
| `.MaxLength(n)` | `string?` | 201 |
| `.Email()` | `string?` | 203 |
| `.Matches(pattern)` | `string?` | 202 |
| `.Positive()` | `int`, `decimal`, `IComparable<T>` | 301 |
| `.Negative()` | `IComparable<T>` | 302 |
| `.InRange(min, max)` | `int`, `decimal`, `IComparable<T>` | 300 |

## Database Validation

Requires `IQueryableRepository<TEntity>` registered in DI for each entity type.
All extensions constrain `TEntity : DomainEntity`.

### Available Checks

| Extension | Code | Logic |
|---|---|---|
| `.DbExists<TValue, TEntity>(query)` | `NotFound` (401) | `AnyAsync` must be true |
| `.DbNotExists<TValue, TEntity>(query)` | `Conflict` (600) | `AnyAsync` must be false |
| `.DbUnique<TValue, TEntity>(query)` | `NotUnique` (400) | `CountAsync` must be ≤ 1 |
| `.DbCount<TValue, TEntity>(query, min, max)` | `OutOfRange` (300) | `CountAsync` must be in `[min, max]` |
| `.DbAny<TValue, TEntity>(query)` | `NotFound` (401) | `AnyAsync` must be true (value-independent) |
| `.DbNone<TValue, TEntity>(query)` | `Conflict` (600) | `AnyAsync` must be false (value-independent) |

### Query Builder Signature

Value-dependent checks (`DbExists`, `DbNotExists`, `DbUnique`, `DbCount`):

```csharp
Func<TValue, IServiceProvider, IQueryable<TEntity>, IQueryable<TEntity>>
```

Value-independent checks (`DbAny`, `DbNone`):

```csharp
Func<IServiceProvider, IQueryable<TEntity>, IQueryable<TEntity>>
```

### Examples

```csharp
public sealed class CreateUserValidator : FluentValidator<CreateUserCommand>
{
    protected override void DefineRules(IConstraintBuilder<CreateUserCommand> b)
    {
        // Email must be unique
        b.Property(x => x.Email)
            .NotEmpty()
            .DbUnique<string?, UserEntity>(
                (email, sp, q) => q.Where(u => u.Email == email));

        // Department must exist (FK check)
        b.Property(x => x.DepartmentId)
            .Positive()
            .DbExists<int, DepartmentEntity>(
                (id, sp, q) => q.Where(d => d.Id == id));

        // Username must not be taken
        b.Property(x => x.Username)
            .NotEmpty()
            .DbNotExists<string?, UserEntity>(
                (name, sp, q) => q.Where(u => u.Username == name));

        // Max 5 active orders per user
        b.Property(x => x.UserId)
            .DbCount<int, OrderEntity>(
                (userId, sp, q) => q.Where(o => o.UserId == userId && o.IsActive),
                min: 0, max: 5);

        // At least one admin must exist in the system
        b.Property(x => x.Role)
            .DbAny<string?, UserEntity>(
                (sp, q) => q.Where(u => u.Role == "Admin"));
    }
}
```

### Update Scenario (Exclude Self)

Filter out the current entity inside the query builder:

```csharp
b.Property(x => x.Email)
    .DbUnique<string?, UserEntity>(
        (email, sp, q) => q.Where(u => u.Email == email && u.Id != currentId));
```

`currentId` is captured from the outer scope — your responsibility to provide it.

## Custom Constraint Extensions

Add your own — just extend `IPropertyConstraintBuilder<TValue>`:

```csharp
public static class MyConstraints
{
    public static IPropertyConstraintBuilder<string?> NoProfanity<T>(
        this IPropertyConstraintBuilder<string?> b)
        => b.AddCheck((ValidationCode)1001, (v, sp, ct) =>
        {
            var filter = sp.GetRequiredService<IProfanityFilter>();
            return new ValueTask<bool>(v is null || !filter.Contains(v));
        });
}
```

## Validation Codes

`ValidationCode` is a `struct` wrapping `uint`. Built-in: 0–999. Your codes: 1000+.

```csharp
public static readonly ValidationCode MyCustomError = (ValidationCode)1001u;
```

## Presenter (Localization)

```csharp
public sealed class MyResolver : IValidationMessageResolver
{
    public string Resolve(ValidationCode code, string propertyName)
        => code.Value switch
        {
            100 => $"'{propertyName}' cannot be empty.",
            200 => $"'{propertyName}' is too short.",
            _   => $"'{propertyName}' didn't passed the test ({code})."
        };
}

services.AddSingleton<IValidationMessageResolver, MyResolver>();
```

```csharp
var presenter = sp.GetRequiredService<IValidationPresenter>();
var messages = presenter.Format(report);       // IReadOnlyList<string>
var single = presenter.FormatSingle(report);   // joined string
var grouped = presenter.FormatGrouped(report); // Dictionary<property, string[]>
```

## Architecture

```
MyLib.Validation.Abstractions  ← reference from Domain/Application
  IAsyncValidator<T>           ← single contract in DI
  IValidationProvider          ← scoped, resolves by context key
  IConstraintBuilder<T>        ← fluent rule definition
  IPropertyConstraintBuilder   ← per-property checks
  Constraints as extensions    ← no classes, just methods

MyLib.Validation               ← reference from Composition Root
  ValidationProvider           ← DI resolution + collector selection
  ConstraintBuilder<T>         ← compiles rules into delegates
  DI registration              ← AddValidation() + AddValidators()
```