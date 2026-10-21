## Quick Start

```csharp
// Register
services.AddPresentation(b =>
{
    b.Add<User>(CoreContexts.Api, "{Name} ({Email})");
});

// Use
var presenter = sp.GetRequiredService<IPresentationProvider>();
string text = presenter.Present(user, CoreContexts.Api);
```

---

## Template Syntax

| Syntax | Meaning | Example |
|--------|---------|---------|
| `{Prop}` | `target.Prop.ToString()` | `{Name}` → `"John"` |
| `{$Prop}` | recursive `Present(target.Prop, ctx)` | `{$Status}` → nested presentation |
| `{$}` | recursive `Present(target, ctx)` | `{$}` → self-presentation |
| literal | as-is | `User: ` → `"User: "` |

**Null values** render as the string `"null"`.

---

## Registration

### Simple Type

```csharp
b.Add<Entity>(CoreContexts.Api, "{Name} {Surname} [{$Code}]");
```

### Custom Bindings

```csharp
b.Add<Person>(CoreContexts.Api, "{full}, age {Age}",
    ("full", p => (object?)$"{p.FirstName} {p.LastName}"));
```

Binding name must match the placeholder name in the template.

### Aggregate (Collection)

```csharp
b.Add<Entity>(CoreContexts.Api, "{Name}: {$Code}");  // item presenter required

b.AddAggregate<Report, Entity>(
    CoreContexts.Api,
    headerTemplate: "Report {$Code}:\n",
    itemTemplate:   "  - {$}\n",      // {$} = present current item
    source:         r => r.Errors,
    separator:      "");              // optional, between items
```

**Requirement:** the item type (`Entity`) must be registered separately.

### State-Driven

```csharp
b.AddStated<Order, OrderState>(CoreContexts.Api, o => o.State)
    .When(OrderState.Pending, "Pending {Id}")
    .When(OrderState.Done,    "Complete {Id}")
    .Else("Cancelled {Id}");                // optional
```

---

## Contexts

```csharp
public readonly record struct PresentationContext(int Id);

// System: 1000–1999
public static class CoreContexts
{
    public static readonly PresentationContext Api = new(1001);
    public static readonly PresentationContext Log = new(1002);
}

// User-defined: 10000+
public static class MyContexts
{
    public static readonly PresentationContext Telegram = new(10001);
}
```

Same type, different templates per context:

```csharp
b.Add<Entity>(CoreContexts.Api, "{Name}");
b.Add<Entity>(CoreContexts.Log, "[{Code}] {Name} {Surname}");
```

---

## Behavior

| Situation | Result |
|-----------|--------|
| No registration for `(Type, Context)` | `target.ToString()` |
| `Present(null, ctx)` | `"null"` |
| Duplicate registration for same `(Type, Context)` | **last wins** |
| `{$Prop}` but `PropType` not registered | **throws at build** |
| Circular dependency (`A → {$B} → {$A}`) | **throws at build** |
| `{$}` in simple registration (self-loop) | **throws at build** |
| `{$}` in aggregate header (self-loop) | **throws at build** |
| Placeholder `{Foo}` but no property `Foo` | **throws at build** |
| Stated with no `Else` and unknown state | **throws at runtime** |

---

## Extending from Modules

```csharp
// Module A
services.AddPresentation(b => {
    b.Add<Entity>(CoreContexts.Api, "{Name}");
});

// Module B — adds its own types, can override
services.AddPresentation(b => {
    b.Add<Order>(CoreContexts.Api, "{$State}: {Id}");
    b.Add<Entity>(CoreContexts.Api, "{Name} {Surname}");  // override
});
```

---

## Introspection

```csharp
presenter.CanPresent(typeof(Entity), CoreContexts.Api); // bool
```