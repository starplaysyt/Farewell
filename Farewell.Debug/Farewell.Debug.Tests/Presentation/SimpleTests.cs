namespace Farewell.Debug.Tests.Presentation;

public class SimpleTests
{
    [Fact]
    public void Plain_properties_render_correctly()
    {
        var p = PresentationTestHelper.Build(b =>
            b.Add<Entity>(Ctx.A, "{Name} {Surname}"));

        var result = p.Present(new Entity("John", "Doe", ResultCode.Ok), Ctx.A);
        Assert.Equal("John Doe", result);
    }

    [Fact]
    public void Literal_only_template_works()
    {
        var p = PresentationTestHelper.Build(b =>
            b.Add<Entity>(Ctx.A, "literally nothing dynamic"));

        var result = p.Present(new Entity("x", "y", ResultCode.Ok), Ctx.A);
        Assert.Equal("literally nothing dynamic", result);
    }

    [Fact]
    public void Empty_template_returns_empty_string()
    {
        var p = PresentationTestHelper.Build(b =>
            b.Add<Entity>(Ctx.A, ""));

        Assert.Equal("", p.Present(new Entity("a", "b", ResultCode.Ok), Ctx.A));
    }

    [Fact]
    public void Only_placeholder_template()
    {
        var p = PresentationTestHelper.Build(b =>
            b.Add<Entity>(Ctx.A, "{Name}"));

        Assert.Equal("Solo", p.Present(new Entity("Solo", "x", ResultCode.Ok), Ctx.A));
    }

    [Fact]
    public void Adjacent_placeholders()
    {
        var p = PresentationTestHelper.Build(b =>
            b.Add<Entity>(Ctx.A, "{Name}{Surname}"));

        Assert.Equal("JohnDoe", p.Present(new Entity("John", "Doe", ResultCode.Ok), Ctx.A));
    }

    [Fact]
    public void Value_type_properties_render()
    {
        var p = PresentationTestHelper.Build(b =>
            b.Add<WithValueType>(Ctx.A, "{Number} {Price}"));

        var result = p.Present(new WithValueType(42, 3.14, DateTime.MinValue), Ctx.A);
        Assert.StartsWith("42 ", result);
        Assert.Contains("3", result);
    }
}