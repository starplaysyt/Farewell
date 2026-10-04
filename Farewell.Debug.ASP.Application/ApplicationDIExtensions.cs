using System.Reflection;
using Farewell.Abstractions.CQRS;
using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.Debug.ASP.Application.Commands;

namespace Farewell.Debug.ASP.Application;

public static class ApplicationDIExtensions
{
    public static IServiceBuilder AddApplication(this IServiceBuilder builder)
    {
        builder.AddMediator();
        builder.AddCQRSHandlers(Assembly.GetAssembly(typeof(AddFirstEntityCommand)) ?? throw 
            new InvalidOperationException("Failed to get assembly"));

        builder.AddScoped<AsyncHandler<AddFirstEntityCommand>, AddFirstEntityHandler>();
        
        return builder;
    }
}