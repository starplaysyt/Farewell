namespace Farewell.Debug.Tests.Presentation;

public class StatedTests
{
    [Fact]
    public void Stated_selects_template_by_state()
    {
        var p = PresentationTestHelper.Build(b =>
            b.AddStated<Order, OrderState>(Ctx.A, o => o.State)
                .When(OrderState.Pending, "pending:{Id}")
                .When(OrderState.Done, "done:{Id}")
                .Else("unknown:{Id}"));

        Assert.Equal("pending:O1", p.Present(new Order("O1", OrderState.Pending), Ctx.A));
        Assert.Equal("done:O2", p.Present(new Order("O2", OrderState.Done), Ctx.A));
    }

    [Fact]
    public void Stated_fallback_on_unknown_state()
    {
        var p = PresentationTestHelper.Build(b =>
            b.AddStated<Order, OrderState>(Ctx.A, o => o.State)
                .When(OrderState.Pending, "p:{Id}")
                .Else("fallback:{Id}"));

        Assert.Equal("fallback:O1", p.Present(new Order("O1", OrderState.Done), Ctx.A));
    }

    [Fact]
    public void Stated_without_fallback_throws_on_unknown_state()
    {
        var p = PresentationTestHelper.Build(b =>
            b.AddStated<Order, OrderState>(Ctx.A, o => o.State)
                .When(OrderState.Pending, "p:{Id}"));

        var ex = Assert.Throws<InvalidOperationException>(() =>
            p.Present(new Order("x", OrderState.Done), Ctx.A));

        Assert.Contains("stated", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}