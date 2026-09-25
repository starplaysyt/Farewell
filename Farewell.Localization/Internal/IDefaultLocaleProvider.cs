using System.Collections.Frozen;
using Farewell.Abstractions.Localization;

namespace Farewell.Localization.Internal;

internal interface IDefaultLocaleProvider : ILocalizationProvider
{
    FrozenDictionary<string, string> Data { get; }
    IReadOnlyList<string> OrderedKeys { get; }
}