using Farewell.Abstractions.Validation;
using Farewell.Application.Validation.Constraints;

namespace Farewell.Debug.Tests.Validation;

public sealed class StringConstraintsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NotEmpty_Fails_OnEmptyOrWhitespace(string? value)
    {
        var constraint = new StringConstraints.NotEmptyConstraint();
        Assert.False(constraint.Check(value));
    }

    [Theory]
    [InlineData("a")]
    [InlineData("hello")]
    [InlineData("  hello  ")]
    public void NotEmpty_Passes_OnNonEmpty(string value)
    {
        var constraint = new StringConstraints.NotEmptyConstraint();
        Assert.True(constraint.Check(value));
    }

    [Fact]
    public void NotEmpty_Code_IsNotEmpty()
    {
        var constraint = new StringConstraints.NotEmptyConstraint();
        Assert.Equal(ValidationCode.NotEmpty, constraint.Code);
    }

    [Theory]
    [InlineData("ab", 2, true)]
    [InlineData("abc", 2, true)]
    [InlineData("a", 2, false)]
    [InlineData(null, 2, false)]
    public void MinLength_Check(string? value, int min, bool expected)
    {
        var constraint = new StringConstraints.MinLengthConstraint(min);
        Assert.Equal(expected, constraint.Check(value));
    }

    [Fact]
    public void MinLength_Code_IsMinLength()
    {
        var constraint = new StringConstraints.MinLengthConstraint(2);
        Assert.Equal(ValidationCode.MinLength, constraint.Code);
    }

    [Theory]
    [InlineData("ab", 5, true)]
    [InlineData("abcde", 5, true)]
    [InlineData("abcdef", 5, false)]
    [InlineData(null, 5, true)]
    public void MaxLength_Check(string? value, int max, bool expected)
    {
        var constraint = new StringConstraints.MaxLengthConstraint(max);
        Assert.Equal(expected, constraint.Check(value));
    }

    [Fact]
    public void MaxLength_Code_IsMaxLength()
    {
        var constraint = new StringConstraints.MaxLengthConstraint(5);
        Assert.Equal(ValidationCode.MaxLength, constraint.Code);
    }

    [Theory]
    [InlineData("test@mail.com", @"^[^@\s]+@[^@\s]+\.[^@\s]+$", true)]
    [InlineData("notanemail", @"^[^@\s]+@[^@\s]+\.[^@\s]+$", false)]
    [InlineData(null, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", false)]
    public void Matches_Check(string? value, string pattern, bool expected)
    {
        var constraint = new StringConstraints.InvalidFormatConstraint(pattern);
        Assert.Equal(expected, constraint.Check(value));
    }

    [Fact]
    public void Matches_Code_IsInvalidFormat()
    {
        var constraint = new StringConstraints.InvalidFormatConstraint(@"\d+");
        Assert.Equal(ValidationCode.InvalidFormat, constraint.Code);
    }
}