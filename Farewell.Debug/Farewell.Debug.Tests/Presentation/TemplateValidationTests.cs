namespace Farewell.Debug.Tests.Presentation;

public class TemplateValidationTests
{
    [Fact]
    public void Nonexistent_property_throws_at_build()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            PresentationTestHelper.Build(b =>
                b.Add<Entity>(Ctx.A, "{NoSuchProperty}")));

        Assert.Contains("NoSuchProperty", ex.Message);
    }

    [Fact]
    public void Unclosed_placeholder_throws_at_build()
    {
        Assert.Throws<FormatException>(() =>
            PresentationTestHelper.Build(b =>
                b.Add<Entity>(Ctx.A, "{Name")));
    }

    [Fact]
    public void Empty_placeholder_throws_at_build()
    {
        Assert.Throws<FormatException>(() =>
            PresentationTestHelper.Build(b =>
                b.Add<Entity>(Ctx.A, "{}")));
    }
}