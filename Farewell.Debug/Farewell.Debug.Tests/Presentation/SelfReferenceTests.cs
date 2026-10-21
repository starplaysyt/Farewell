namespace Farewell.Debug.Tests.Presentation;

public class SelfReferenceTests
{
    [Fact]
    public void Simple_registration_with_self_reference_throws_cycle()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            PresentationTestHelper.Build(b =>
                b.Add<Entity>(Ctx.A, "{$}")));

        Assert.Contains("ircular", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Self_reference_inside_item_template_resolves_to_item_presenter()
    {
        var p = PresentationTestHelper.Build(b =>
        {
            b.Add<SimpleItem>(Ctx.A, "<{Value}>");
            b.AddAggregate<Report, SimpleItem>(
                Ctx.A,
                headerTemplate: "H:",
                itemTemplate: "{$};",
                source: r => r.Errors.Cast<SimpleItem>()
                    .ToList());
        });
    }

    [Fact]
    public void Empty_placeholder_without_dollar_throws_at_parse()
    {
        Assert.Throws<FormatException>(() =>
            PresentationTestHelper.Build(b =>
                b.Add<Entity>(Ctx.A, "{}")));
    }
}