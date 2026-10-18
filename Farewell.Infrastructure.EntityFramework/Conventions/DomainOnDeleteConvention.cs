using System.Reflection;
using Farewell.Abstractions.Attributes.Domain;
using Farewell.Abstractions.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Farewell.Infrastructure.Conventions;

public sealed class DomainOnDeleteConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(
        IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (clrType is null) continue;

            var attr = clrType.GetCustomAttribute<OnDeleteAttribute>();
            if (attr is null) continue;

            var behavior = MapBehavior(attr.Behavior);
            
            foreach (var fk in entityType.GetReferencingForeignKeys())
            {
                if (behavior == DeleteBehavior.SetNull && fk.IsRequired)
                {
                    throw new InvalidOperationException(
                        $"[OnDelete(SetNull)] на {clrType.Name}: " +
                        $"FK {fk.DeclaringEntityType.ClrType.Name}." +
                        $"{fk.Properties[0].Name} не nullable. " +
                        $"SetNull невозможен для required-связей.");
                }
                
                fk.SetDeleteBehavior(behavior, fromDataAnnotation: true);
            }
        }
    }

    private static DeleteBehavior MapBehavior(OnDeleteBehavior behavior) => behavior switch
    {
        OnDeleteBehavior.Restrict => DeleteBehavior.Restrict,
        OnDeleteBehavior.Cascade  => DeleteBehavior.Cascade,
        OnDeleteBehavior.SetNull  => DeleteBehavior.SetNull,
        _ => throw new ArgumentOutOfRangeException(nameof(behavior))
    };
}