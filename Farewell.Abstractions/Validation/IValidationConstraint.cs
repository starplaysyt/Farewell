namespace Farewell.Abstractions.Validation;

public interface IValidationConstraint<in TValue>
{
    ValidationCode Code { get; }
    bool Check(TValue value);
}