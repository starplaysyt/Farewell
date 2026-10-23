using Farewell.Abstractions.General;

namespace Farewell.Debug.Tests.Validation;

public sealed class ValidationCodeTests
{
    [Fact]
    public void Default_IsOk()
    {
        ResultCode code = new();
        Assert.Equal(ResultCode.Ok, code);
        Assert.Equal(0, code.Code);
    }

    [Fact]
    public void Equality_SameValue_AreEqual()
    {
        var a = new ResultCode(42);
        var b = new ResultCode(42);
        Assert.Equal(a, b);
        Assert.True(a == b);
        Assert.False(a != b);
    }

    [Fact]
    public void Equality_DifferentValue_AreNotEqual()
    {
        var a = new ResultCode(1);
        var b = new ResultCode(2);
        Assert.NotEqual(a, b);
        Assert.True(a != b);
    }

    [Fact]
    public void ImplicitConversion_ToUInt()
    {
        int value = ResultCode.ValidationNotEmpty;
        Assert.Equal(3_002, value);
    }

    [Fact]
    public void ExplicitConversion_FromUInt()
    {
        var code = (ResultCode)1001;
        Assert.Equal(1001, code.Code);
    }

    [Fact]
    public void GetHashCode_SameForEqualCodes()
    {
        var a = new ResultCode(500);
        var b = new ResultCode(500);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void BuiltInCodes_HaveExpectedValues()
    {
        Assert.Equal(0, ResultCode.Ok.Code);
        Assert.Equal(3_001, ResultCode.ValidationMultipleErrors.Code);
        Assert.Equal(3_002, ResultCode.ValidationNotEmpty.Code);
        Assert.Equal(3_005, ResultCode.ValidationMinLength.Code);
        Assert.Equal(3_011, ResultCode.ValidationOutOfRange.Code);
        Assert.Equal(1_409, ResultCode.Conflict.Code);
    }

    [Fact]
    public void UserCodes_StartFrom1000()
    {
        var custom = (ResultCode)1000;
        Assert.Equal(1000, custom.Code);
        Assert.NotEqual(ResultCode.Ok, custom);
    }
}