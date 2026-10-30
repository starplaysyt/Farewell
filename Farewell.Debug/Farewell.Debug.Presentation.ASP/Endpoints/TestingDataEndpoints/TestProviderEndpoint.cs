using Farewell.Abstractions.DI;
using Farewell.Compatibility.MS.DependencyInjection;
using Farewell.Debug.Presentation.ASP.TestingServices;
using FastEndpoints;

namespace Farewell.Debug.Presentation.ASP.Endpoints.TestingDataEndpoints;

public class TestProviderEndpointResponse
{
    public string Mode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    // Unkeyed
    public UnkeyedCheck RootUnkeyed { get; set; } = new();
    public UnkeyedCheck Scope1Unkeyed { get; set; } = new();
    public UnkeyedCheck Scope2Unkeyed { get; set; } = new();

    // Keyed
    public KeyedCheck RootKeyed { get; set; } = new();
    public KeyedCheck Scope1Keyed { get; set; } = new();
    public KeyedCheck Scope2Keyed { get; set; } = new();
}

public class UnkeyedCheck
{
    public string? SingletonId { get; set; }
    public string? ScopedId { get; set; }
    public string? TransientId1 { get; set; }
    public string? TransientId2 { get; set; }
    public bool SingletonSameAsRoot { get; set; }
    public bool ScopedSameWithinScope { get; set; }
    public bool TransientDifferentEachResolve { get; set; }
}

public class KeyedCheck
{
    public string? SingletonKeyedId { get; set; }
    public string? ScopedKeyedId { get; set; }
    public string? TransientKeyedId1 { get; set; }
    public string? TransientKeyedId2 { get; set; }
    public bool SingletonKeyedSameAsRoot { get; set; }
    public bool ScopedKeyedSameWithinScope { get; set; }
    public bool TransientKeyedDifferentEachResolve { get; set; }
    public string? RequiredKeyedTypeName { get; set; }
}

public class TestProviderEndpoint : EndpointWithoutRequest<TestProviderEndpointResponse>
{
    public override void Configure()
    {
        Get("api/test/provider");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "DI compatibility check: Farewell abstractions over MS.Extensions.DependencyInjection";
            s.Description = "Resolves unkeyed and keyed services (Singleton/Scoped/Transient) via Farewell.Abstractions.DI interfaces through FarewellScopeProvider";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = new TestProviderEndpointResponse
        {
            Mode = "Farewell.Abstractions.DI via Farewell.Compatibility.MS.DependencyInjection",
            Message = "Checking IScopeProvider/IKeyedServiceProvider/GetKeyedService/GetRequiredKeyedService/CreateScope"
        };

        // Оборачиваем ASP RequestServices (MS IServiceProvider) в адаптер Farewell
        var msRoot = HttpContext.RequestServices;
        IScopeProvider farewellRoot = new FarewellScopeProvider(msRoot);

        // Также можно получить MS IKeyedServiceProvider напрямую для наглядности
        var msKeyed = msRoot as Microsoft.Extensions.DependencyInjection.IKeyedServiceProvider;

        // ---- ROOT (unkeyed) ----
        var rootSingleton1 = farewellRoot.GetService(typeof(IUnkeyedSingletonTest)) as IUnkeyedSingletonTest;
        var rootScoped1 = farewellRoot.GetService(typeof(IUnkeyedScopedTest)) as IUnkeyedScopedTest; // в корневом скоупе MS разрешение scoped из root провайдера формально разрешимо, но по смыслу scoped лучше в CreateScope()
        var rootTransient1 = farewellRoot.GetService(typeof(IUnkeyedTransientTest)) as IUnkeyedTransientTest;
        var rootTransient2 = farewellRoot.GetService(typeof(IUnkeyedTransientTest)) as IUnkeyedTransientTest;

        response.RootUnkeyed = new UnkeyedCheck
        {
            SingletonId = rootSingleton1?.Id.ToString(),
            ScopedId = rootScoped1?.Id.ToString(),
            TransientId1 = rootTransient1?.Id.ToString(),
            TransientId2 = rootTransient2?.Id.ToString(),
            SingletonSameAsRoot = rootSingleton1 != null,
            ScopedSameWithinScope = rootScoped1 != null && rootScoped1 == rootScoped1, // placeholder
            TransientDifferentEachResolve = rootTransient1 != null && rootTransient2 != null && rootTransient1.Id != rootTransient2.Id
        };

        // ---- ROOT (keyed) ----
        var rootKeyedSingleton = farewellRoot.GetKeyedService(typeof(IKeyedTest), "keyed-singleton") as IKeyedTest;
        var rootKeyedScoped = farewellRoot.GetKeyedService(typeof(IKeyedTest), "keyed-scoped") as IKeyedTest;
        var rootKeyedTransient1 = farewellRoot.GetKeyedService(typeof(IKeyedTest), "keyed-transient") as IKeyedTest;
        var rootKeyedTransient2 = farewellRoot.GetKeyedService(typeof(IKeyedTest), "keyed-transient") as IKeyedTest;

        // Проверка GetRequiredKeyedService через MS IKeyedServiceProvider (чтобы убедиться что совместимость по MS-интерфейсу тоже ок)
        object? requiredViaMs = null;
        if (msKeyed != null)
            requiredViaMs = msKeyed.GetRequiredKeyedService(typeof(IKeyedTest), "keyed-singleton");

        response.RootKeyed = new KeyedCheck
        {
            SingletonKeyedId = rootKeyedSingleton?.Id.ToString(),
            ScopedKeyedId = rootKeyedScoped?.Id.ToString(),
            TransientKeyedId1 = rootKeyedTransient1?.Id.ToString(),
            TransientKeyedId2 = rootKeyedTransient2?.Id.ToString(),
            SingletonKeyedSameAsRoot = rootKeyedSingleton != null,
            ScopedKeyedSameWithinScope = rootKeyedScoped != null,
            TransientKeyedDifferentEachResolve = rootKeyedTransient1 != null && rootKeyedTransient2 != null && rootKeyedTransient1.Id != rootKeyedTransient2.Id,
            RequiredKeyedTypeName = requiredViaMs?.GetType().Name
        };

        // ---- SCOPE 1 ----
        using (var scope1 = farewellRoot.CreateScope())
        {
            var s1Singleton = scope1.GetService(typeof(IUnkeyedSingletonTest)) as IUnkeyedSingletonTest;
            var s1Scoped1 = scope1.GetService(typeof(IUnkeyedScopedTest)) as IUnkeyedScopedTest;
            var s1Scoped2 = scope1.GetService(typeof(IUnkeyedScopedTest)) as IUnkeyedScopedTest;
            var s1Transient1 = scope1.GetService(typeof(IUnkeyedTransientTest)) as IUnkeyedTransientTest;
            var s1Transient2 = scope1.GetService(typeof(IUnkeyedTransientTest)) as IUnkeyedTransientTest;

            response.Scope1Unkeyed = new UnkeyedCheck
            {
                SingletonId = s1Singleton?.Id.ToString(),
                ScopedId = s1Scoped1?.Id.ToString(),
                TransientId1 = s1Transient1?.Id.ToString(),
                TransientId2 = s1Transient2?.Id.ToString(),
                SingletonSameAsRoot = rootSingleton1 != null && s1Singleton != null && rootSingleton1.Id == s1Singleton.Id,
                ScopedSameWithinScope = s1Scoped1 != null && s1Scoped2 != null && s1Scoped1.Id == s1Scoped2.Id,
                TransientDifferentEachResolve = s1Transient1 != null && s1Transient2 != null && s1Transient1.Id != s1Transient2.Id
            };

            var ks1Singleton = scope1.GetKeyedService(typeof(IKeyedTest), "keyed-singleton") as IKeyedTest;
            var ks1Scoped1 = scope1.GetKeyedService(typeof(IKeyedTest), "keyed-scoped") as IKeyedTest;
            var ks1Scoped2 = scope1.GetKeyedService(typeof(IKeyedTest), "keyed-scoped") as IKeyedTest;
            var ks1Transient1 = scope1.GetKeyedService(typeof(IKeyedTest), "keyed-transient") as IKeyedTest;
            var ks1Transient2 = scope1.GetKeyedService(typeof(IKeyedTest), "keyed-transient") as IKeyedTest;
            var ks1Required = scope1.GetRequiredKeyedService(typeof(IKeyedTest), "keyed-scoped") as IKeyedTest;

            response.Scope1Keyed = new KeyedCheck
            {
                SingletonKeyedId = ks1Singleton?.Id.ToString(),
                ScopedKeyedId = ks1Scoped1?.Id.ToString(),
                TransientKeyedId1 = ks1Transient1?.Id.ToString(),
                TransientKeyedId2 = ks1Transient2?.Id.ToString(),
                SingletonKeyedSameAsRoot = rootKeyedSingleton != null && ks1Singleton != null && rootKeyedSingleton.Id == ks1Singleton.Id,
                ScopedKeyedSameWithinScope = ks1Scoped1 != null && ks1Scoped2 != null && ks1Scoped1.Id == ks1Scoped2.Id,
                TransientKeyedDifferentEachResolve = ks1Transient1 != null && ks1Transient2 != null && ks1Transient1.Id != ks1Transient2.Id,
                RequiredKeyedTypeName = ks1Required?.GetType().Name
            };
        }

        // ---- SCOPE 2 (проверка что scoped не переиспользуется между скоупами) ----
        using (var scope2 = farewellRoot.CreateScope())
        {
            var s2Singleton = scope2.GetService(typeof(IUnkeyedSingletonTest)) as IUnkeyedSingletonTest;
            var s2Scoped1 = scope2.GetService(typeof(IUnkeyedScopedTest)) as IUnkeyedScopedTest;
            var s2Scoped2 = scope2.GetService(typeof(IUnkeyedScopedTest)) as IUnkeyedScopedTest;
            var s2Transient1 = scope2.GetService(typeof(IUnkeyedTransientTest)) as IUnkeyedTransientTest;
            var s2Transient2 = scope2.GetService(typeof(IUnkeyedTransientTest)) as IUnkeyedTransientTest;

            response.Scope2Unkeyed = new UnkeyedCheck
            {
                SingletonId = s2Singleton?.Id.ToString(),
                ScopedId = s2Scoped1?.Id.ToString(),
                TransientId1 = s2Transient1?.Id.ToString(),
                TransientId2 = s2Transient2?.Id.ToString(),
                SingletonSameAsRoot = rootSingleton1 != null && s2Singleton != null && rootSingleton1.Id == s2Singleton.Id,
                ScopedSameWithinScope = s2Scoped1 != null && s2Scoped2 != null && s2Scoped1.Id == s2Scoped2.Id,
                TransientDifferentEachResolve = s2Transient1 != null && s2Transient2 != null && s2Transient1.Id != s2Transient2.Id
            };

            var ks2Singleton = scope2.GetKeyedService(typeof(IKeyedTest), "keyed-singleton") as IKeyedTest;
            var ks2Scoped1 = scope2.GetKeyedService(typeof(IKeyedTest), "keyed-scoped") as IKeyedTest;
            var ks2Scoped2 = scope2.GetKeyedService(typeof(IKeyedTest), "keyed-scoped") as IKeyedTest;
            var ks2Transient1 = scope2.GetKeyedService(typeof(IKeyedTest), "keyed-transient") as IKeyedTest;
            var ks2Transient2 = scope2.GetKeyedService(typeof(IKeyedTest), "keyed-transient") as IKeyedTest;
            var ks2Required = scope2.GetRequiredKeyedService(typeof(IKeyedTest), "keyed-transient") as IKeyedTest;

            response.Scope2Keyed = new KeyedCheck
            {
                SingletonKeyedId = ks2Singleton?.Id.ToString(),
                ScopedKeyedId = ks2Scoped1?.Id.ToString(),
                TransientKeyedId1 = ks2Transient1?.Id.ToString(),
                TransientKeyedId2 = ks2Transient2?.Id.ToString(),
                SingletonKeyedSameAsRoot = rootKeyedSingleton != null && ks2Singleton != null && rootKeyedSingleton.Id == ks2Singleton.Id,
                ScopedKeyedSameWithinScope = ks2Scoped1 != null && ks2Scoped2 != null && ks2Scoped1.Id == ks2Scoped2.Id,
                TransientKeyedDifferentEachResolve = ks2Transient1 != null && ks2Transient2 != null && ks2Transient1.Id != ks2Transient2.Id,
                RequiredKeyedTypeName = ks2Required?.GetType().Name
            };
        }

        await Send.OkAsync(response, ct);
    }
}