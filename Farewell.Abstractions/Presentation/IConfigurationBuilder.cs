using Farewell.Abstractions.DI;

namespace Farewell.Abstractions.Presentation;

public interface IConfigurationBuilder
{
    public IConfigurationBuilder AddServiceBuilder<TBuilder>()
        where TBuilder : IServiceBuilder, new();

    public IConfigurationBuilder AddServices(Action<IServiceBuilder> serviceSetup);
}