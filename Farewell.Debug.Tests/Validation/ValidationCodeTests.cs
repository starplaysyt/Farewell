using Farewell.Abstractions.Validation;

namespace Farewell.Debug.Tests.Validation;

public sealed class ValidationCodeTests
{
    [Fact]
    public void Ok_HasValueZero()
    {
        Assert.Equal(0u, ValidationCode.Ok.Value);
    }

    [Fact]
    public void Equality_SameValue_ReturnsTrue()
    {
        var a = new ValidationCode(100);
        var b = new ValidationCode(100);
        Assert.True(a == b);
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equality_DifferentValue_ReturnsFalse()
    {
        var a = new ValidationCode(100);
        var b = new ValidationCode(101);
        Assert.True(a != b);
        Assert.False(a.Equals(b));
    }

    [Fact]
    public void CustomCode_InUserRange_Works()
    {
        var custom = new ValidationCode(1000);
        Assert.Equal(1000u, custom.Value);
        Assert.NotEqual(ValidationCode.NotEmpty, custom);
    }

    [Fact]
    public void ImplicitConversion_ToUint_Works()
    {
        uint value = ValidationCode.NotEmpty;
        Assert.Equal(100u, value);
    }

    [Fact]
    public void ExplicitConversion_FromUint_Works()
    {
        var code = (ValidationCode)1500;
        Assert.Equal(1500u, code.Value);
    }

    [Fact]
    public void HashCode_SameValue_SameHash()
    {
        var a = new ValidationCode(42);
        var b = new ValidationCode(42);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
}