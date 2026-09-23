namespace Farewell.Abstractions.Configuration;

public interface IConfigResolver
{
    T Resolve<T>(string absolutePath) where T : class;
    void SaveDefaults<T>(string absolutePath, T instance) where T : class;
}