using System.Text.RegularExpressions;
using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Constraints;

public static class StringConstraints
{
    public sealed class NotEmptyConstraint : IValidationConstraint<string?>
    {
        public ValidationCode Code => ValidationCode.NotEmpty;
        public bool Check(string? value) => !string.IsNullOrWhiteSpace(value);
    }

    public sealed class MinLengthConstraint : IValidationConstraint<string?>
    {
        private readonly int _min;
        public ValidationCode Code => ValidationCode.MinLength;

        public MinLengthConstraint(int min) => _min = min;

        public bool Check(string? value) => value is not null && value.Length >= _min;
    }

    public sealed class MaxLengthConstraint : IValidationConstraint<string?>
    {
        private readonly int _max;
        public ValidationCode Code => ValidationCode.MaxLength;

        public MaxLengthConstraint(int max) => _max = max;

        public bool Check(string? value) => value is null || value.Length <= _max;
    }

    public sealed class InvalidFormatConstraint : IValidationConstraint<string?>
    {
        private readonly Regex _regex;
        public ValidationCode Code => ValidationCode.InvalidFormat;

        public InvalidFormatConstraint(string pattern)
            => _regex = new Regex(pattern, RegexOptions.Compiled);

        public bool Check(string? value) => value is not null && _regex.IsMatch(value);
    }
}