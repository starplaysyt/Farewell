using Farewell.Abstractions.General;
using Farewell.Abstractions.Validation;

namespace Farewell.Debug.Tests.Validation;

public sealed class ValidationStatusTests
{
    [Fact]
    public void Ok_IsSuccess()
    {
        Assert.True(ValidationStatus.Ok.IsSuccess);
        Assert.Equal(ResultCode.Ok, ValidationStatus.Ok.Code);
        Assert.Equal(string.Empty, ValidationStatus.Ok.PropertyName);
    }

    [Fact]
    public void Error_IsNotSuccess()
    {
        var status = ValidationStatus.Error(ResultCode.ValidationNotEmpty, "Name");
        Assert.False(status.IsSuccess);
        Assert.Equal(ResultCode.ValidationNotEmpty, status.Code);
        Assert.Equal("Name", status.PropertyName);
    }
}