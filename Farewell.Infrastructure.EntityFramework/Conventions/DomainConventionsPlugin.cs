using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Farewell.Infrastructure.Conventions;

public sealed class DomainConventionsPlugin : IConventionSetPlugin
{
    public ConventionSet ModifyConventions(ConventionSet conventionSet)
    {
        conventionSet.ModelFinalizingConventions
            .Add(new DomainIdentityConvention());

        return conventionSet;
    }
}