using System.Collections.Immutable;
using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Extensions;

public static class ValidationPresenterExtensions
{
    public static string FormatSingle(this IValidationPresenter presenter, ValidationReport report)
        => string.Join(Environment.NewLine, presenter.Format(report));

    public static IReadOnlyDictionary<string, string[]> FormatGrouped(
        this IValidationPresenter presenter,
        ValidationReport report)
    {
        if (report.IsSuccess || report.Errors is null)
            return ImmutableDictionary<string, string[]>.Empty;

        return report.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(presenter.Format).ToArray());
    }
}