namespace Farewell.Debug.Tests.Presentation;

public class CustomBindingTests
{
    [Fact]
    public void Custom_binding_overrides_property_lookup()
    {
        var p = PresentationTestHelper.Build(b =>
            b.Add<Person>(Ctx.A, "{full} ({Age})",
                ("full", pers => (object?)(pers.FirstName + " " + pers.LastName))));

        Assert.Equal("John Doe (30)",
            p.Present(new Person("John", "Doe", 30), Ctx.A));
    }

    [Fact]
    public void Custom_binding_can_perform_computation()
    {
        var p = PresentationTestHelper.Build(b =>
            b.Add<WithValueType>(Ctx.A, "doubled={doubled}",
                ("doubled", v => (object?)(v.Number * 2))));

        Assert.Equal("doubled=84", p.Present(new WithValueType(42, 0, DateTime.MinValue), Ctx.A));
    }

    [Fact]
    public void Custom_binding_returning_null_renders_null_literal()
    {
        var p = PresentationTestHelper.Build(b =>
            b.Add<Entity>(Ctx.A, "x={x}",
                ("x", _ => null)));

        Assert.Equal("x=null", p.Present(new Entity("a", "b", ResultCode.Ok), Ctx.A));
    }
}