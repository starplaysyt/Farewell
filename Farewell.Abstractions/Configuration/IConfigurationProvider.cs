namespace Farewell.Abstractions.Configuration;

public interface IConfigurationProvider
{
    /// <summary>
    /// Returns primitive value by path.
    /// </summary>
    /// <example>Get&lt;int&gt;("server:port")</example>
    T? Get<T>(string path) where T : IParsable<T>;
    
    /// <summary>
    /// Returns configuration section to the class object.
    /// </summary>
    /// <example>GetSection&lt;DatabaseConfig&gt;("database:connection")</example>
    T? GetSection<T>(string path) where T : class;
    
    /// <summary>
    /// Returns configuration section, path is getting from attribute.
    /// </summary>
    /// <example>GetSection&lt;DatabaseConfig&gt;()</example>
    T? GetSection<T>() where T : class;
    
    /// <summary>
    /// Reloads config from the source.
    /// </summary>
    void Reload();
}