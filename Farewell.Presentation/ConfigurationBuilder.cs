using System.Reflection;
using Farewell.Abstractions.DI;
using Farewell.Abstractions.Presentation;

namespace Farewell.Presentation;

public class ConfigurationBuilder : IConfigurationBuilder
{
    private IServiceBuilder? _serviceBuilder;
    private List<string>? serviceNamespaces;
    
    internal Type[] ReflectedTypes { get; set; }

    public IConfigurationBuilder AddServiceScope(string scopeName)
    {
        var assembly = Assembly.Load(scopeName);

        foreach (var type in assembly.GetTypes())
        {
            Console.WriteLine("type: " + type.FullName);
        }

        return this;
    }
    
    public IConfigurationBuilder AddServiceBuilder<TBuilder>()
        where TBuilder : IServiceBuilder, new()
    {
        if (_serviceBuilder is not null)
            throw new InvalidOperationException("ServiceBuilder has already been added.");
        _serviceBuilder = new TBuilder();
        return this;
    }

    public IConfigurationBuilder AddServices(Action<IServiceBuilder> serviceSetup)
    {
        serviceSetup.Invoke(_serviceBuilder ??
                            throw new InvalidOperationException(
                                "ServiceBuilder has not been added."));
        return this;
    }
}