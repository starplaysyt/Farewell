using Farewell.Abstractions.Validation;

namespace Farewell.Debug.Tests.Validation;

public sealed class ValidationCodeTests
{
    [Fact]
    public void Default_IsOk()
    {
        ValidationCode code = default;
        Assert.Equal(ValidationCode.Ok, code);
        Assert.Equal(0u, code.Value);
    }

    [Fact]
    public void Equality_SameValue_AreEqual()
    {
        var a = new ValidationCode(42);
        var b = new ValidationCode(42);
        Assert.Equal(a, b);
        Assert.True(a == b);
        Assert.False(a != b);
    }

    [Fact]
    public void Equality_DifferentValue_AreNotEqual()
    {
        var a = new ValidationCode(1);
        var b = new ValidationCode(2);
        Assert.NotEqual(a, b);
        Assert.True(a != b);
    }

    [Fact]
    public void ImplicitConversion_ToUInt()
    {
        uint value = ValidationCode.NotEmpty;
        Assert.Equal(100u, value);
    }

    [Fact]
    public void ExplicitConversion_FromUInt()
    {
        var code = (ValidationCode)1001u;
        Assert.Equal(1001u, code.Value);
    }

    [Fact]
    public void GetHashCode_SameForEqualCodes()
    {
        var a = new ValidationCode(500);
        var b = new ValidationCode(500);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void BuiltInCodes_HaveExpectedValues()
    {
        Assert.Equal(0u, ValidationCode.Ok.Value);
        Assert.Equal(1u, ValidationCode.MultipleErrors.Value);
        Assert.Equal(100u, ValidationCode.NotEmpty.Value);
        Assert.Equal(200u, ValidationCode.MinLength.Value);
        Assert.Equal(300u, ValidationCode.OutOfRange.Value);
        Assert.Equal(400u, ValidationCode.NotUnique.Value);
    }

    [Fact]
    public void UserCodes_StartFrom1000()
    {
        var custom = (ValidationCode)1000u;
        Assert.Equal(1000u, custom.Value);
        Assert.NotEqual(ValidationCode.Ok, custom);
    }
}