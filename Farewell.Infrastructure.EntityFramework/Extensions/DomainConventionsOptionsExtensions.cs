using Farewell.Infrastructure.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Farewell.Infrastructure.Extensions;

public sealed class DomainConventionsOptionsExtension : IDbContextOptionsExtension
{
    public DbContextOptionsExtensionInfo Info { get; }

    public DomainConventionsOptionsExtension()
    {
        Info = new ExtensionInfo(this);
    }

    public void ApplyServices(IServiceCollection services)
    {
        services.AddSingleton<IConventionSetPlugin, DomainConventionsPlugin>();
    }

    public void Validate(IDbContextOptions options) { }

    private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
    {
        public ExtensionInfo(IDbContextOptionsExtension extension)
            : base(extension) { }

        public override bool IsDatabaseProvider => false;

        public override string LogFragment => "DomainConventions ";

        public override int GetServiceProviderHashCode() => 0;

        public override bool ShouldUseSameServiceProvider(
            DbContextOptionsExtensionInfo other)
            => other is ExtensionInfo;

        public override void PopulateDebugInfo(
            IDictionary<string, string> debugInfo)
            => debugInfo["DomainConventions"] = "1";
    }
}