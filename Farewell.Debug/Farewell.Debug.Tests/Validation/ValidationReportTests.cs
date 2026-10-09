using Farewell.Abstractions.Validation;

namespace Farewell.Debug.Tests.Validation;

public sealed class ValidationReportTests
{
    [Fact]
    public void Ok_Static_IsSuccess()
    {
        Assert.True(ValidationReport.Ok.IsSuccess);
        Assert.Null(ValidationReport.Ok.Errors);
    }

    [Fact]
    public void NullErrors_IsOk()
    {
        var report = new ValidationReport(null);
        Assert.True(report.IsSuccess);
        Assert.Equal(ValidationCode.Ok, report.Status);
    }

    [Fact]
    public void EmptyErrors_IsOk()
    {
        var report = new ValidationReport(Array.Empty<ValidationStatus>());
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public void SingleError_StatusEqualsErrorCode()
    {
        var errors = new[] { ValidationStatus.Error(ValidationCode.NotEmpty, "A") };
        var report = new ValidationReport(errors);
        Assert.Equal(ValidationCode.NotEmpty, report.Status);
    }

    [Fact]
    public void MultipleErrors_StatusIsMultipleErrors()
    {
        var errors = new[]
        {
            ValidationStatus.Error(ValidationCode.NotEmpty, "A"),
            ValidationStatus.Error(ValidationCode.MinLength, "B")
        };
        var report = new ValidationReport(errors);
        Assert.Equal(ValidationCode.MultipleErrors, report.Status);
        Assert.Equal(2, report.Errors!.Count);
    }
}