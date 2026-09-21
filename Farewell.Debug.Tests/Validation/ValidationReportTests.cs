using Farewell.Abstractions.Validation;

namespace Farewell.Debug.Tests.Validation;

public sealed class ValidationReportTests
{
    [Fact]
    public void Ok_IsSuccess()
    {
        Assert.True(ValidationReport.Ok.IsSuccess);
        Assert.Equal(ValidationCode.Ok, ValidationReport.Ok.Status);
    }

    [Fact]
    public void Ok_Errors_IsNull()
    {
        Assert.Null(ValidationReport.Ok.Errors);
    }

    [Fact]
    public void SingleError_Status_EqualsErrorCode()
    {
        var errors = new[] { ValidationStatus.Error(ValidationCode.NotEmpty, "Name") };
        var report = new ValidationReport(errors);

        Assert.Equal(ValidationCode.NotEmpty, report.Status);
        Assert.False(report.IsSuccess);
    }

    [Fact]
    public void MultipleErrors_Status_IsMultipleErrors()
    {
        var errors = new[]
        {
            ValidationStatus.Error(ValidationCode.NotEmpty, "Name"),
            ValidationStatus.Error(ValidationCode.InvalidFormat, "Email")
        };
        var report = new ValidationReport(errors);

        Assert.Equal(ValidationCode.MultipleErrors, report.Status);
        Assert.False(report.IsSuccess);
    }

    [Fact]
    public void MultipleErrors_Errors_ContainsAll()
    {
        var errors = new[]
        {
            ValidationStatus.Error(ValidationCode.NotEmpty, "Name"),
            ValidationStatus.Error(ValidationCode.InvalidFormat, "Email")
        };
        var report = new ValidationReport(errors);

        Assert.Equal(2, report.Errors!.Count);
    }

    [Fact]
    public void NullErrors_Status_IsOk()
    {
        var report = new ValidationReport(null);
        Assert.Equal(ValidationCode.Ok, report.Status);
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public void EmptyErrors_Status_IsOk()
    {
        var report = new ValidationReport(Array.Empty<ValidationStatus>());
        Assert.Equal(ValidationCode.Ok, report.Status);
        Assert.True(report.IsSuccess);
    }
}