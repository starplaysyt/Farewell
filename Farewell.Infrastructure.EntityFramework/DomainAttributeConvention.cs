using System.Reflection;
using Farewell.Abstractions.Attributes.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Farewell.Infrastructure;

public sealed class DomainAttributeConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(
        IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        var model = (IMutableModel)modelBuilder.Metadata;

        foreach (var entityType in model.GetEntityTypes())
        {
            ApplyIdentity(entityType);
            ApplyOnDelete(entityType);
            ApplyInheritance(entityType);
        }
    }

    private static void ApplyIdentity(IMutableEntityType entityType)
    {
        foreach (var property in entityType.GetProperties())
        {
            var member = (MemberInfo?)property.PropertyInfo ?? property.FieldInfo;
            if (member is null) continue;

            if (member.GetCustomAttribute<IdentityAttribute>() is null) continue;

            property.ValueGenerated = ValueGenerated.OnAdd;

            var pk = entityType.FindPrimaryKey();
            if (pk is not null && pk.Properties.Contains(property))
                continue;

            if (entityType.FindKey(property) is null)
            {
                entityType.AddKey(property);
            }
        }
    }

    private static void ApplyOnDelete(IMutableEntityType entityType)
    {
        var clrType = entityType.ClrType;

        var attributes = clrType
            .GetCustomAttributes<OnDeleteAttribute>(inherit: true)
            .ToList();

        if (attributes.Count == 0) return;

        var defaultAction = attributes
            .Where(a => string.IsNullOrEmpty(a.Navigation))
            .Select(a => (DeleteBehavior?)Map(a.Action))
            .FirstOrDefault();

        var overrides = attributes
            .Where(a => !string.IsNullOrEmpty(a.Navigation))
            .ToDictionary(
                a => a.Navigation!,
                a => Map(a.Action),
                StringComparer.OrdinalIgnoreCase);

        foreach (var fk in entityType.GetForeignKeys())
        {
            var depNav = fk.DependentToPrincipal?.Name;
            var princNav = fk.PrincipalToDependent?.Name;

            if (depNav is not null && overrides.TryGetValue(depNav, out var overrideAction))
            {
                fk.DeleteBehavior = overrideAction;
            }
            else if (princNav is not null && overrides.TryGetValue(princNav, out var princOverride))
            {
                fk.DeleteBehavior = princOverride;
            }
            else if (defaultAction.HasValue)
            {
                fk.DeleteBehavior = defaultAction.Value;
            }
        }
    }

    private static DeleteBehavior Map(DeleteAction action) => action switch
    {
        DeleteAction.Cascade => DeleteBehavior.Cascade,
        DeleteAction.Restrict => DeleteBehavior.Restrict,
        DeleteAction.SetNull => DeleteBehavior.SetNull,
        DeleteAction.NoAction => DeleteBehavior.NoAction,
        _ => throw new ArgumentOutOfRangeException(nameof(action))
    };

    private static void ApplyInheritance(IMutableEntityType entityType)
    {
        var clrType = entityType.ClrType;

        var attr = clrType.GetCustomAttribute<InheritanceAttribute>(inherit: false);
        if (attr is null) return;

        var root = entityType.GetRootType();

        var strategyString = attr.Strategy switch
        {
            InheritanceStrategy.Tph => "TPH",
            InheritanceStrategy.Tpt => "TPT",
            InheritanceStrategy.Tpc => "TPC",
            _ => throw new ArgumentOutOfRangeException()
        };

        root.SetAnnotation("Relational:MappingStrategy", strategyString);
    }
}