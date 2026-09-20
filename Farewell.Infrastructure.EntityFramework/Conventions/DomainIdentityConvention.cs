using System.Reflection;
using Farewell.Abstractions.Attributes.Domain;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Farewell.Infrastructure.Conventions;

public sealed class DomainIdentityConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(
        IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        var model = (IMutableModel)modelBuilder.Metadata;

        foreach (var entityType in model.GetEntityTypes())
            ApplyIdentity(entityType);
    }

    private static void ApplyIdentity(IMutableEntityType entityType)
    {
        foreach (var property in entityType.GetProperties())
        {
            var member = (MemberInfo?)property.PropertyInfo ?? property.FieldInfo;
            if (member is null) continue;

            var attr = member.GetCustomAttribute<IdentityAttribute>();
            if (attr is null) continue;

            if (attr.AutoGenerate)
                property.ValueGenerated = ValueGenerated.OnAdd;

            var pk = entityType.FindPrimaryKey();
            if (pk is not null && pk.Properties.Contains(property))
                continue;

            var hasIndex = entityType.GetIndexes()
                .Any(i => i.Properties.Count == 1 && i.Properties[0] == property);

            if (hasIndex) continue;

            var index = entityType.AddIndex(property);
            index.IsUnique = true;
        }
    }
}