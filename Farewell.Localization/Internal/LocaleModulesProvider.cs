namespace Farewell.Localization.Internal;

internal sealed class LocaleModulesProvider(IReadOnlyList<Type> modules) : ILocaleModulesProvider
{
    public IReadOnlyList<Type> Modules { get; } = modules;
}