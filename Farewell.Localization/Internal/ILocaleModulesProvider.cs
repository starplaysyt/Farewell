namespace Farewell.Localization.Internal;

internal interface ILocaleModulesProvider
{
    IReadOnlyList<Type> Modules { get; }
}