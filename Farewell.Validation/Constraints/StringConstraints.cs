using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Constraints;

public static class StringConstraints
{
    public sealed class NotEmptyConstraint : IValidationConstraint<string?>
    {
        public ValidationCode Code => ValidationCode.NotEmpty;

        public ValueTask<bool> CheckAsync(string? value, IServiceProvider _, CancellationToken __)
            => new(!string.IsNullOrWhiteSpace(value));
    }

    public sealed class MinLengthConstraint(Func<IServiceProvider, int> min)
        : IValidationConstraint<string?>
    {
        public ValidationCode Code => ValidationCode.MinLength;

        public MinLengthConstraint(int min) : this(_ => min) { }

        public ValueTask<bool> CheckAsync(string? value, IServiceProvider sp, CancellationToken _)
            => new(value is not null && value.Length >= min(sp));
    }

    public sealed class MaxLengthConstraint(Func<IServiceProvider, int> max)
        : IValidationConstraint<string?>
    {
        public ValidationCode Code => ValidationCode.MaxLength;

        public MaxLengthConstraint(int max) : this(_ => max) { }

        public ValueTask<bool> CheckAsync(string? value, IServiceProvider sp, CancellationToken _)
            => new(value is null || value.Length <= max(sp));
    }

    public sealed class MatchesConstraint(
        [StringSyntax(StringSyntaxAttribute.Regex)] string pattern) : IValidationConstraint<string?>
    {
        private readonly Regex _regex = new(pattern, RegexOptions.Compiled);
        public ValidationCode Code => ValidationCode.InvalidFormat;

        public ValueTask<bool> CheckAsync(string? value, IServiceProvider _, CancellationToken __)
            => new(value is not null && _regex.IsMatch(value));
    }
}