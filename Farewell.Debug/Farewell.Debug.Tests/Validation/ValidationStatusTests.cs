using Farewell.Abstractions.Validation;

namespace Farewell.Debug.Tests.Validation;

public sealed class ValidationStatusTests
{
    [Fact]
    public void Ok_IsSuccess()
    {
        Assert.True(ValidationStatus.Ok.IsSuccess);
        Assert.Equal(ValidationCode.Ok, ValidationStatus.Ok.Code);
    }

    [Fact]
    public void Error_IsNotSuccess()
    {
        var s = ValidationStatus.Error(ValidationCode.NotEmpty, "Name");
        Assert.False(s.IsSuccess);
        Assert.Equal("Name", s.PropertyName);
        Assert.Equal(ValidationCode.NotEmpty, s.Code);
    }

    [Fact]
    public void Error_WithCustomCode_Works()
    {
        var custom = new ValidationCode(1234);
        var s = ValidationStatus.Error(custom, "Field");
        Assert.Equal(custom, s.Code);
    }
}