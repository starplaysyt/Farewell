using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Presentation;
using Farewell.DI;
using Farewell.Presentation;

namespace Farewell.Debug.Tests.Presentation;

public class ServiceBuilderIntegrationTests
{
    [Fact]
    public void AddPresentation_registers_singleton()
    {
        var services = new ServiceBuilder();
        services.AddPresentation(b =>
            b.Add<Entity>(Ctx.A, "{Name}"));

        var sp = services.Build();

        var p1 = sp.GetRequiredService<IPresentationProvider>();
        var p2 = sp.GetRequiredService<IPresentationProvider>();

        Assert.Same(p1, p2);
    }

    [Fact]
    public void AddPresentation_fails_fast_on_bad_registration()
    {
        var services = new ServiceBuilder();

        Assert.Throws<InvalidOperationException>(() =>
            services.AddPresentation(b =>
                b.Add<Entity>(Ctx.A, "{NoSuch}")));
    }

    [Fact]
    public void Multiple_AddPresentation_calls_additive_last_wins()
    {
        var services = new ServiceBuilder();

        services.AddPresentation(b => b.Add<Entity>(Ctx.A, "ONE:{Name}"));

        var sp = services.Build();
        var p = sp.GetRequiredService<IPresentationProvider>();

        Assert.Equal("ONE:X", p.Present(new Entity("X", "Y", ResultCode.Ok), Ctx.A));
    }

    [Fact]
    public void Resolved_provider_works_end_to_end()
    {
        var services = new ServiceBuilder();
        services.AddPresentation(b =>
        {
            b.Add<ResultCode>(Ctx.A, "code:{0}", ("0", c => c));
            b.Add<Entity>(Ctx.A, "{Name} [{$Code}]");
            b.AddAggregate<Report, Entity>(
                Ctx.A,
                headerTemplate: "Report({$Code}):\n",
                itemTemplate: "  * {$}\n",
                source: r => r.Errors);
        });
    }

    [Fact]
    public void Resolved_provider_works_end_to_end_v2()
    {
        var services = new ServiceBuilder();
        services.AddPresentation(b =>
        {
            b.Add<ResultCode>(Ctx.A, "{0}", ("0", c => c));
            b.Add<Entity>(Ctx.A, "{Name} [{$Code}]");
            b.AddAggregate<Report, Entity>(
                Ctx.A,
                headerTemplate: "Report({$Code}):\n",
                itemTemplate: "  * {Name} [{$Code}]\n",
                source: r => r.Errors);
        });

        var sp = services.Build();
        var p = sp.GetRequiredService<IPresentationProvider>();

        var report = new Report(ResultCode.NotFound, [
            new("Alice", "A", ResultCode.NotAllowed),
            new("Bob", "B", ResultCode.ValidationFailed)
        ]);

        var result = p.Present(report, Ctx.A);
        Assert.Equal(
            "Report(NotFound):\n" +
            "  * Alice [NotAllowed]\n" +
            "  * Bob [ValidationFailed]\n",
            result);
    }
}