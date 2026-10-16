using System.Linq.Expressions;

namespace Farewell.Abstractions.Validation;

public interface IConstraintBuilder<T>
{
    IPropertyConstraintBuilder<TValue?> Property<TValue>(
        Expression<Func<T, TValue>> property,
        Func<T, bool>? when = null);

    ICompiledValidator<T> Compile();
}