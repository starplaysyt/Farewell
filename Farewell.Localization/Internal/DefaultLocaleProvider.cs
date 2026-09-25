using System.Collections.Frozen;
using System.Reflection;
using Farewell.Abstractions.Attributes.Localization;
using Farewell.Abstractions.Exceptions;

namespace Farewell.Localization.Internal;

internal sealed class DefaultLocaleProvider : IDefaultLocaleProvider
{
    private readonly List<Type> _modules;
    private FrozenDictionary<string, string> _data;
    private IReadOnlyList<string> _orderedKeys;

    public FrozenDictionary<string, string> Data => _data;
    public IReadOnlyList<string> OrderedKeys => _orderedKeys;

    public string this[string key] => _data.TryGetValue(key, out var value)
        ? value
        : throw new LocalizationException($"Key '{key}' not found in default locale provider");

    public DefaultLocaleProvider(IReadOnlyList<Type> modules)
    {
        _modules = modules.ToList();
        (_data, _orderedKeys) = BuildData();
    }

    public void Reload() => throw new NotSupportedException(
        "Default locale provider does not support reload"
    );

    private (FrozenDictionary<string, string> Data, IReadOnlyList<string> OrderedKeys) BuildData()
    {
        var dict = new Dictionary<string, string>();
        var orderedKeys = new List<string>();

        foreach (var module in _modules)
            ProcessModule(module, dict, orderedKeys);

        return (dict.ToFrozenDictionary(), orderedKeys.AsReadOnly());
    }

    private static void ProcessModule(Type module, Dictionary<string, string> dict, List<string> orderedKeys)
    {
        var fields = module
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .OrderBy(f => f.MetadataToken);

        foreach (var field in fields)
        {
            ValidateField(field, module);

            var key = (string)field.GetValue(null)!;
            var defaultValue = field.GetCustomAttribute<DefaultLocaleAttribute>()!.Value;

            if (!dict.TryAdd(key, defaultValue))
                throw new LocalizationException(
                    $"Duplicate key '{key}' found in module '{module.Name}'"
                );

            orderedKeys.Add(key);
        }
    }

    private static void ValidateField(FieldInfo field, Type module)
    {
        if (!field.IsLiteral)
            throw new LocalizationException(
                $"Field '{field.Name}' in '{module.Name}' must be const"
            );

        if (field.FieldType != typeof(string))
            throw new LocalizationException(
                $"Field '{field.Name}' in '{module.Name}' must be of type string"
            );

        if (field.GetCustomAttribute<DefaultLocaleAttribute>() is null)
            throw new LocalizationException(
                $"Field '{field.Name}' in '{module.Name}' must have [DefaultLocale] attribute"
            );
    }
}