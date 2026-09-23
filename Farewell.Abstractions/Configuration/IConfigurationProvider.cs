namespace Farewell.Abstractions.Configuration;

public interface IConfigurationProvider<out T> where T : class
{
    T Get();
    void Reload();
}