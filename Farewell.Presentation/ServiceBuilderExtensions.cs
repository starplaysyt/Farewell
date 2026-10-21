using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Presentation;
using Farewell.Presentation.Internal;

namespace Farewell.Presentation;

public static class ServiceBuilderExtensions
{
    public static IServiceBuilder AddPresentation(
        this IServiceBuilder services,
        Action<IPresentationBuilder> configure)
    {
        var builder = new PresentationBuilder();
        configure(builder);

        // Compile everything NOW (at registration time, not at first resolve).
        // This gives fail-fast on misconfiguration.
        var compiler = new PresentationCompiler(builder.Registrations);
        var compiled = compiler.CompileAll();
        var provider = new PresentationProvider(compiled);

        services.AddSingleton<IPresentationProvider>(provider);
        return services;
    }
}