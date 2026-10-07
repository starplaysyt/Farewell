using Farewell.Abstractions.Validation;
using Farewell.Validation.Collectors;

namespace Farewell.Debug.Tests.Validation;

public sealed class CollectorsTests
{
    [Fact]
    public void CollectAll_NoErrors_ReportIsOk()
    {
        var c = new CollectAllCollector();
        c.Collect(ValidationStatus.Ok);
        c.Collect(ValidationStatus.Ok);
        Assert.True(c.ToReport().IsSuccess);
    }

    [Fact]
    public void CollectAll_AlwaysContinues()
    {
        var c = new CollectAllCollector();
        Assert.True(c.Collect(ValidationStatus.Error(ValidationCode.NotEmpty, "A")));
        Assert.True(c.Collect(ValidationStatus.Error(ValidationCode.MinLength, "B")));
    }

    [Fact]
    public void CollectAll_CollectsAllErrors()
    {
        var c = new CollectAllCollector();
        c.Collect(ValidationStatus.Error(ValidationCode.NotEmpty, "A"));
        c.Collect(ValidationStatus.Error(ValidationCode.MinLength, "B"));
        var report = c.ToReport();
        Assert.Equal(2, report.Errors!.Count);
    }

    [Fact]
    public void BreakOnFirst_NoErrors_ReportIsOk()
    {
        var c = new BreakOnFirstCollector();
        c.Collect(ValidationStatus.Ok);
        Assert.True(c.ToReport().IsSuccess);
    }

    [Fact]
    public void BreakOnFirst_StopsOnFirstError()
    {
        var c = new BreakOnFirstCollector();
        Assert.False(c.Collect(ValidationStatus.Error(ValidationCode.NotEmpty, "A")));
    }

    [Fact]
    public void BreakOnFirst_ContinuesOnSuccess()
    {
        var c = new BreakOnFirstCollector();
        Assert.True(c.Collect(ValidationStatus.Ok));
    }

    [Fact]
    public void BreakOnFirst_ReportContainsFirstErrorOnly()
    {
        var c = new BreakOnFirstCollector();
        c.Collect(ValidationStatus.Error(ValidationCode.NotEmpty, "A"));
        var report = c.ToReport();
        Assert.Single(report.Errors!);
        Assert.Equal(ValidationCode.NotEmpty, report.Status);
    }
}