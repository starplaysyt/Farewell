using System.Text;
using Farewell.Abstractions.Validation;

namespace Farewell.Abstractions.Configuration.Exceptions;

public class ConfigurationValidationException(ValidationReport report) : Exception
{
    public override string ToString()
    {
        StringBuilder builder = new();
        builder.AppendLine("Configuration validation failed for fields: ");
        builder.AppendJoin("\n\t", report.Errors ?? []);
        builder.AppendLine("--------------------------------------------");
        return builder + base.ToString();
    }
}