using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Farewell.Infrastructure.Extensions;

public sealed class AutoConfigurationOptionsExtension : IDbContextOptionsExtension
{
    private readonly Assembly _assembly;

    public AutoConfigurationOptionsExtension(Assembly assembly)
    {
        _assembly = assembly;
        Info = new ExtensionInfo(this);
    }

    public DbContextOptionsExtensionInfo Info { get; }

    public void ApplyServices(IServiceCollection services)
    {
        services.AddSingleton<IModelCustomizer>(sp =>
            new AutoConfigurationModelCustomizer(
                sp.GetRequiredService<ModelCustomizerDependencies>(),
                _assembly));
    }

    public void Validate(IDbContextOptions options) { }

    private sealed class ExtensionInfo(AutoConfigurationOptionsExtension extension)
        : DbContextOptionsExtensionInfo(extension)
    {
        private readonly Assembly _assembly = extension._assembly;

        public override bool IsDatabaseProvider => false;

        public override string LogFragment =>
            $"AutoConfigurations({_assembly.GetName().Name}) ";

        public override int GetServiceProviderHashCode() =>
            _assembly.GetHashCode();

        public override bool ShouldUseSameServiceProvider(
            DbContextOptionsExtensionInfo other) =>
            other is ExtensionInfo info && info._assembly == _assembly;

        public override void PopulateDebugInfo(
            IDictionary<string, string> debugInfo) =>
            debugInfo["AutoConfigurations"] = _assembly.GetName().Name!;
    }
}