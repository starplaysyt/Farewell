namespace Farewell.Abstractions.Presentation;

public readonly record struct PresentationContext(int Id)
{
    // 0 - Default
    // 1..999 - reserved
    // 1000..1999 - core system contexts
    // 2000..9999 - first-party extensions
    // 10000+ - user-defined

    public static readonly PresentationContext Default = new(0);
}

public static class CoreContexts
{
    public static readonly PresentationContext Api = new(1001);
    public static readonly PresentationContext Log = new(1002);
    public static readonly PresentationContext Ui = new(1003);
}