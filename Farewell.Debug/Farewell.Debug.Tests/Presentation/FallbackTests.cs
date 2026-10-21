using Farewell.Abstractions.Presentation;

namespace Farewell.Debug.Tests.Presentation;

public class FallbackTests
{
    [Fact]
    public void No_registration_falls_back_to_ToString()
    {
        var p = PresentationTestHelper.Build(_ => { });
        var e = new Entity("A", "B", ResultCode.Ok);

        Assert.Equal(e.ToString(), p.Present(e, Ctx.A));
    }

    [Fact]
    public void Registration_in_different_context_does_not_help()
    {
        var p = PresentationTestHelper.Build(b =>
            b.Add<Entity>(Ctx.A, "{Name}"));

        var e = new Entity("A", "B", ResultCode.Ok);
        Assert.Equal(e.ToString(), p.Present(e, Ctx.B));
        Assert.Equal("A", p.Present(e, Ctx.A));
    }

    [Fact]
    public void Default_context_lookup()
    {
        var p = PresentationTestHelper.Build(b =>
            b.Add<Entity>(PresentationContext.Default, "{Name}"));

        Assert.Equal("X", p.Present(new Entity("X", "Y", ResultCode.Ok)));
    }

    [Fact]
    public void CanPresent_reports_registered_pairs()
    {
        var p = PresentationTestHelper.Build(b =>
            b.Add<Entity>(Ctx.A, "{Name}"));

        Assert.True(p.CanPresent(typeof(Entity), Ctx.A));
        Assert.False(p.CanPresent(typeof(Entity), Ctx.B));
        Assert.False(p.CanPresent(typeof(SimpleItem), Ctx.A));
    }
}