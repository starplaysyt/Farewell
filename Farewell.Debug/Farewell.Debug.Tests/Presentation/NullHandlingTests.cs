namespace Farewell.Debug.Tests.Presentation;

public class NullHandlingTests
{
    [Fact]
    public void Null_reference_property_renders_null_literal()
    {
        var p = PresentationTestHelper.Build(b =>
            b.Add<Nullable>(Ctx.A, "val={MaybeNull}"));

        var result = p.Present(new Nullable(null, null), Ctx.A);
        Assert.Equal("val=null", result);
    }

    [Fact]
    public void Null_nullable_value_type_renders_null_literal()
    {
        var p = PresentationTestHelper.Build(b =>
            b.Add<Nullable>(Ctx.A, "n={MaybeInt}"));

        Assert.Equal("n=null", p.Present(new Nullable("x", null), Ctx.A));
    }

    [Fact]
    public void Non_null_string_renders_as_is()
    {
        var p = PresentationTestHelper.Build(b =>
            b.Add<Nullable>(Ctx.A, "{MaybeNull}"));

        Assert.Equal("hello", p.Present(new Nullable("hello", null), Ctx.A));
    }

    [Fact]
    public void Present_null_target_returns_null_literal()
    {
        var p = PresentationTestHelper.Build(_ => { });
        Assert.Equal("null", p.Present(null!, Ctx.A));
    }
}