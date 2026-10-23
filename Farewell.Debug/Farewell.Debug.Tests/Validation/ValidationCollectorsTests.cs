using Farewell.Abstractions.General;
using Farewell.Abstractions.Validation;

namespace Farewell.Debug.Tests.Validation;

public class ValidationCollectorsTests
{
    [Fact]
    public void CollectAll_GathersAllErrors()
    {
        var collector = new CollectAllCollector();

        Assert.True(collector.Collect(ValidationStatus.Ok));
        Assert.True(collector.Collect(
            ValidationStatus.Error(ResultCode.ValidationNotEmpty, "Name")));
        Assert.True(collector.Collect(
            ValidationStatus.Error(ResultCode.ValidationMinLength, "Email")));

        var report = collector.ToReport();
        Assert.False(report.IsSuccess);
        Assert.Equal(2, report.Errors!.Count);
    }

    [Fact]
    public void CollectAll_NoErrors_ReturnsOk()
    {
        var collector = new CollectAllCollector();
        collector.Collect(ValidationStatus.Ok);

        Assert.True(collector.ToReport().IsSuccess);
    }

    [Fact]
    public void BreakOnFirst_StopsAfterFirstError()
    {
        var collector = new BreakOnFirstCollector();

        Assert.True(collector.Collect(ValidationStatus.Ok));
        Assert.False(collector.Collect(
            ValidationStatus.Error(ResultCode.ValidationNotEmpty, "Name")));

        var report = collector.ToReport();
        Assert.False(report.IsSuccess);
        Assert.Single(report.Errors!);
        Assert.Equal(ResultCode.ValidationNotEmpty, report.Errors[0].Code);
    }

    [Fact]
    public void BreakOnFirst_NoErrors_ReturnsOk()
    {
        var collector = new BreakOnFirstCollector();
        collector.Collect(ValidationStatus.Ok);

        Assert.True(collector.ToReport().IsSuccess);
    }
}