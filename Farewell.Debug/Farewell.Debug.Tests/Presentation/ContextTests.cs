namespace Farewell.Debug.Tests.Presentation;

public class ContextTests
{
    [Fact]
    public void Different_contexts_can_have_different_templates()
    {
        var p = PresentationTestHelper.Build(b =>
        {
            b.Add<Entity>(Ctx.A, "A:{Name}");
            b.Add<Entity>(Ctx.B, "B:{Surname}");
        });

        var e = new Entity("John", "Doe", ResultCode.Ok);
        Assert.Equal("A:John", p.Present(e, Ctx.A));
        Assert.Equal("B:Doe", p.Present(e, Ctx.B));
    }

    [Fact]
    public void Second_registration_overwrites_first()
    {
        var p = PresentationTestHelper.Build(b =>
        {
            b.Add<Entity>(Ctx.A, "FIRST:{Name}");
            b.Add<Entity>(Ctx.A, "SECOND:{Name}");
        });

        Assert.Equal("SECOND:John",
            p.Present(new Entity("John", "Doe", ResultCode.Ok), Ctx.A));
    }

    [Fact]
    public void Contexts_are_isolated_in_recursive_lookup()
    {
        var p = PresentationTestHelper.Build(b =>
        {
            b.Add<SimpleItem>(Ctx.A, "[A:{Value}]");
            b.Add<SimpleItem>(Ctx.B, "[B:{Value}]");
            b.Add<Nested>(Ctx.A, "{$Inner}");
        });

        Assert.Equal("[A:v]", p.Present(new Nested(new SimpleItem("v"), ""), Ctx.A));
    }
}