using Farewell.Abstractions.Exceptions;

namespace Farewell.Localization;

public sealed class LocalizationModulesBuilder
{
    private readonly List<Type> _modules = [];

    public LocalizationModulesBuilder AddModule(Type moduleType)
    {
        if (_modules.Contains(moduleType))
            throw new LocalizationException(
                $"Module '{moduleType.Name}' is already registered"
            );

        _modules.Add(moduleType);
        return this;
    }

    internal IReadOnlyList<Type> Build() => _modules.AsReadOnly();
}