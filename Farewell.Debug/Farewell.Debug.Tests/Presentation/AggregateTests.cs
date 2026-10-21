namespace Farewell.Debug.Tests.Presentation;

public class AggregateTests
{
    [Fact]
    public void Aggregate_item_uses_self_reference_to_registered_item_presenter()
    {
        var p = PresentationTestHelper.Build(b =>
        {
            b.Add<Entity>(Ctx.A, "{Name}:{Surname}");

            b.AddAggregate<Report, Entity>(
                Ctx.A,
                headerTemplate: "errors:\n",
                itemTemplate: "  - {$}\n",
                source: r => r.Errors);
        });

        var report = new Report(ResultCode.NotFound, [
            new("Alice", "A", ResultCode.Ok),
            new("Bob", "B", ResultCode.Ok)
        ]);

        var result = p.Present(report, Ctx.A);
        Assert.Equal(
            "errors:\n" +
            "  - Alice:A\n" +
            "  - Bob:B\n",
            result);
    }

    [Fact]
    public void Aggregate_header_cannot_self_reference_cycle_detected()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            PresentationTestHelper.Build(b =>
                b.AddAggregate<Report, Entity>(
                    Ctx.A,
                    headerTemplate: "{$}",
                    itemTemplate: "{Name}",
                    source: r => r.Errors)));

        Assert.Contains("ircular", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Aggregate_item_self_without_item_registration_throws()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            PresentationTestHelper.Build(b =>
                b.AddAggregate<Report, Entity>(
                    Ctx.A,
                    headerTemplate: "",
                    itemTemplate: "{$}",
                    source: r => r.Errors)));

        Assert.Contains("Entity", ex.Message);
    }
}