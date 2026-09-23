using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Internal;

internal static class ValidatorCompiler
{
    public static CompiledValidator<T> Compile<T>(Func<T, ValidationStatus?>[] evaluators)
    {
        return new CompiledValidator<T>(
            validateAll: BuildValidateAll(evaluators),
            validateBreak: BuildValidateBreak(evaluators));
    }

    private static Func<T, ValidationReport> BuildValidateAll<T>(
        Func<T, ValidationStatus?>[] evaluators)
    {
        return instance =>
        {
            List<ValidationStatus>? errors = null;

            foreach (var evaluator in evaluators)
            {
                var status = evaluator(instance);
                if (status is null) continue;

                errors ??= new List<ValidationStatus>();
                errors.Add(status);
            }

            return errors is null
                ? ValidationReport.Ok
                : new ValidationReport(errors.ToArray());
        };
    }

    private static Func<T, ValidationStatus> BuildValidateBreak<T>(
        Func<T, ValidationStatus?>[] evaluators)
    {
        return instance =>
        {
            foreach (var evaluator in evaluators)
            {
                var status = evaluator(instance);
                if (status is not null) return status;
            }

            return ValidationStatus.Ok;
        };
    }
}