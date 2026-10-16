using Farewell.Abstractions.Validation;

namespace Farewell.Debug.Tests.Validation;

public sealed class ValidationStatusTests
{
    [Fact]
    public void Ok_IsSuccess()
    {
        Assert.True(ValidationStatus.Ok.IsSuccess);
        Assert.Equal(ValidationCode.Ok, ValidationStatus.Ok.Code);
        Assert.Equal(string.Empty, ValidationStatus.Ok.PropertyName);
    }

    [Fact]
    public void Error_IsNotSuccess()
    {
        var status = ValidationStatus.Error(ValidationCode.NotEmpty, "Name");
        Assert.False(status.IsSuccess);
        Assert.Equal(ValidationCode.NotEmpty, status.Code);
        Assert.Equal("Name", status.PropertyName);
    }
}