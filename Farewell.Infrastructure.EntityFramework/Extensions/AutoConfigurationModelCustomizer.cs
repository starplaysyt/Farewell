using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Farewell.Infrastructure.Extensions;

public sealed class AutoConfigurationModelCustomizer : ModelCustomizer
{
    private readonly Assembly _configurationAssembly;

    public AutoConfigurationModelCustomizer(
        ModelCustomizerDependencies dependencies,
        Assembly configurationAssembly)
        : base(dependencies)
    {
        _configurationAssembly = configurationAssembly;
    }

    public override void Customize(ModelBuilder modelBuilder, DbContext context)
    {
        base.Customize(modelBuilder, context);
        modelBuilder.ApplyConfigurationsFromAssembly(_configurationAssembly);
    }
}