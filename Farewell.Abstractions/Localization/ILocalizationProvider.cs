namespace Farewell.Abstractions.Localization;

public interface ILocalizationProvider
{
    string this[string key] { get; }
    void Reload();
}