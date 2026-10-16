using Farewell.Abstractions.Validation;

namespace Farewell.Debug.Tests.Validation;

public sealed class ValidationReportTests
{
    [Fact]
    public void Ok_IsSuccess_NoErrors()
    {
        Assert.True(ValidationReport.Ok.IsSuccess);
        Assert.Equal(ValidationCode.Ok, ValidationReport.Ok.Status);
        Assert.Null(ValidationReport.Ok.Errors);
    }

    [Fact]
    public void SingleError_StatusIsThatCode()
    {
        var report = new ValidationReport(
            [ValidationStatus.Error(ValidationCode.NotEmpty, "Name")]);

        Assert.False(report.IsSuccess);
        Assert.Equal(ValidationCode.NotEmpty, report.Status);
        Assert.Single(report.Errors!);
    }

    [Fact]
    public void MultipleErrors_StatusIsMultipleErrors()
    {
        var report = new ValidationReport(
        [
            ValidationStatus.Error(ValidationCode.NotEmpty, "Name"),
            ValidationStatus.Error(ValidationCode.MinLength, "Email")
        ]);

        Assert.False(report.IsSuccess);
        Assert.Equal(ValidationCode.MultipleErrors, report.Status);
        Assert.Equal(2, report.Errors!.Count);
    }

    [Fact]
    public void EmptyArray_IsSuccess()
    {
        var report = new ValidationReport([]);
        Assert.True(report.IsSuccess);
    }
}