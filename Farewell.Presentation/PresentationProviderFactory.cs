using Farewell.Abstractions.Presentation;
using Farewell.Presentation.Internal;

namespace Farewell.Presentation;

public static class PresentationProviderFactory
{
    public static IPresentationProvider Build(Action<IPresentationBuilder> configure)
    {
        var builder = new PresentationBuilder();
        configure(builder);
        var compiler = new PresentationCompiler(builder.Registrations);
        var compiled = compiler.CompileAll();
        return new PresentationProvider(compiled);
    }
}