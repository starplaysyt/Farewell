namespace Farewell.Debug.Tests.Presentation;

public class RobustnessTests
{
    [Fact]
    public void Many_sequential_calls_do_not_corrupt_pool()
    {
        var p = PresentationTestHelper.Build(b =>
            b.Add<Entity>(Ctx.A, "{Name}-{Surname}-{Name}"));

        for (int i = 0; i < 10_000; i++)
        {
            var r = p.Present(new Entity($"N{i}", $"S{i}", ResultCode.Ok), Ctx.A);
            Assert.Equal($"N{i}-S{i}-N{i}", r);
        }
    }

    [Fact]
    public async Task Concurrent_calls_produce_correct_results()
    {
        var p = PresentationTestHelper.Build(b =>
            b.Add<Entity>(Ctx.A, "{Name}/{Surname}"));

        var tasks = Enumerable.Range(0, 100).Select(i => Task.Run(() =>
        {
            for (int k = 0; k < 1000; k++)
            {
                var r = p.Present(new Entity($"n{i}", $"s{k}", ResultCode.Ok), Ctx.A);
                Assert.Equal($"n{i}/s{k}", r);
            }
        }));

        await Task.WhenAll(tasks);
    }

    [Fact]
    public void Very_long_template_works()
    {
        var tpl = string.Join("", Enumerable.Range(0, 50).Select(_ => "{Name}"));
        var p = PresentationTestHelper.Build(b => b.Add<Entity>(Ctx.A, tpl));

        var expected = string.Concat(Enumerable.Repeat("X", 50));
        Assert.Equal(expected, p.Present(new Entity("X", "", ResultCode.Ok), Ctx.A));
    }
}