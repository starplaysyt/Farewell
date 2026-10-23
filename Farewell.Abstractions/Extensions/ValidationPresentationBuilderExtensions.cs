using Farewell.Abstractions.Presentation;
using Farewell.Abstractions.Validation;

namespace Farewell.Abstractions.Extensions;

public static class ValidationPresentationBuilderExtensions
{
    public static IPresentationBuilder AddValidationStatus(
        this IPresentationBuilder builder,
        PresentationContext context = default)
    {
        builder.Add<ValidationStatus>(context, "{PropertyName}: {$Code}");
        return builder;
    }

    public static IPresentationBuilder AddValidationReport(
        this IPresentationBuilder builder,
        PresentationContext context = default)
    {
        builder.AddAggregate<ValidationReport, ValidationStatus>(
            context,
            headerTemplate: "{$Status}\n",
            itemTemplate: "  - {$}\n",
            source: r => r.Errors ?? Array.Empty<ValidationStatus>(),
            separator: "");
        return builder;
    }
}