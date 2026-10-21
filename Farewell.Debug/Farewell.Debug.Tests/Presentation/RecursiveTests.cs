namespace Farewell.Debug.Tests.Presentation;

public class RecursiveTests
{
    [Fact]
    public void Recursive_slot_calls_child_presenter()
    {
        var p = PresentationTestHelper.Build(b =>
        {
            b.Add<ResultCode>(Ctx.A, "<code:{0}>",
                ("0", c => c));
        });

        p = PresentationTestHelper.Build(b =>
        {
            b.Add<SimpleItem>(Ctx.A, "[{Value}]");
            b.Add<Nested>(Ctx.A, "{Label}: {$Inner}");
        });

        var result = p.Present(new Nested(new SimpleItem("hello"), "L"), Ctx.A);
        Assert.Equal("L: [hello]", result);
    }

    [Fact]
    public void Deep_recursion_three_levels()
    {
        var p = PresentationTestHelper.Build(b =>
        {
            b.Add<SimpleItem>(Ctx.A, "<<{Value}>>");
            b.Add<Nested>(Ctx.A, "N({$Inner})");
            b.Add<Entity>(Ctx.A, "E:{Name}");
            b.Add<ResultCode>(Ctx.A, "code={0}", ("0", c => c));
        });

        Assert.Equal("N(<<v>>)", p.Present(new Nested(new SimpleItem("v"), "ignore"), Ctx.A));
    }

    [Fact]
    public void Recursive_on_null_renders_null_literal()
    {
        var p = PresentationTestHelper.Build(b =>
        {
            b.Add<SimpleItem>(Ctx.A, "[{Value}]");
            b.Add<Nested>(Ctx.A, "{$Inner}");
        });
    }

    [Fact]
    public void Self_reference_throws_at_build()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            PresentationTestHelper.Build(b =>
                b.Add<SelfRef>(Ctx.A, "{Name} -> {$Next}")));

        Assert.Contains("ircular", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Recursive_with_missing_registration_throws_at_build()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            PresentationTestHelper.Build(b => { b.Add<Nested>(Ctx.A, "{$Inner}"); }));

        Assert.Contains("SimpleItem", ex.Message);
    }
}