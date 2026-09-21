using Farewell.Abstractions.Validation;

namespace Farewell.Debug.Tests.Validation;

public sealed class ValidationStatusTests
{
    [Fact]
    public void Ok_IsSuccess()
    {
        var status = ValidationStatus.Ok;
        Assert.True(status.IsSuccess);
        Assert.Equal(ValidationCode.Ok, status.Code);
    }

    [Fact]
    public void Ok_PropertyName_IsEmpty()
    {
        Assert.Equal(string.Empty, ValidationStatus.Ok.PropertyName);
    }

    [Fact]
    public void Error_IsNotSuccess()
    {
        var status = new ValidationStatus(ValidationCode.NotEmpty, "Name");
        Assert.False(status.IsSuccess);
    }

    [Fact]
    public void Error_HasCorrectCode()
    {
        var status = new ValidationStatus(ValidationCode.MinLength, "Name");
        Assert.Equal(ValidationCode.MinLength, status.Code);
    }

    [Fact]
    public void Error_HasCorrectPropertyName()
    {
        var status = new ValidationStatus(ValidationCode.NotEmpty, "Email");
        Assert.Equal("Email", status.PropertyName);
    }

    [Theory]
    [InlineData(ValidationCode.NotEmpty)]
    [InlineData(ValidationCode.MinLength)]
    [InlineData(ValidationCode.MaxLength)]
    [InlineData(ValidationCode.OutOfRange)]
    [InlineData(ValidationCode.InvalidFormat)]
    public void Error_AllCodes_Supported(ValidationCode code)
    {
        var status = new ValidationStatus(code, "Field");
        Assert.Equal(code, status.Code);
        Assert.False(status.IsSuccess);
    }
}