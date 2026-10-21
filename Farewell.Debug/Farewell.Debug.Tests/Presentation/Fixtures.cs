using Farewell.Abstractions.Presentation;
using Farewell.Presentation;
using Farewell.Presentation.Internal;

namespace Farewell.Debug.Tests.Presentation;

public enum ResultCode
{
    Ok,
    NotFound,
    NotAllowed,
    ValidationFailed
}

public enum OrderState
{
    Pending,
    Processing,
    Done,
    Cancelled
}

public record Entity(string Name, string Surname, ResultCode Code);

public record SimpleItem(string Value);

public record Report(ResultCode Code, List<Entity> Errors);

public record Nullable(string? MaybeNull, int? MaybeInt);

public record WithValueType(int Number, double Price, DateTime When);

public record Order(string Id, OrderState State);

public record Nested(SimpleItem Inner, string Label);

public record SelfRef(string Name, SelfRef? Next);

public record Person(string FirstName, string LastName, int Age);

public static class Ctx
{
    public static readonly PresentationContext A = new(10001);
    public static readonly PresentationContext B = new(10002);
}

internal static class PresentationTestHelper
{
    public static IPresentationProvider Build(Action<IPresentationBuilder> configure)
    {
        return PresentationProviderFactory.Build(configure);
    }
}