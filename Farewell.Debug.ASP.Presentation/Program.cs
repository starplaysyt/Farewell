using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.StateRules;
using Farewell.Compatibility.MS.DependencyInjection;
using Farewell.Compatibility.MS.Logging;
using Farewell.Debug.ASP.Application;
using Farewell.Debug.ASP.Infrastructure;
using Farewell.Infrastructure.Extensions;
using Farewell.Logging;
using Farewell.StateRules;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var serviceBuilder = new FarewellBuilder(builder.Services);

serviceBuilder.AddApplication();
serviceBuilder.AddInfrastructure();

// serviceBuilder.AddLogger<ConsoleLogger>();
serviceBuilder.AddCustomCreateOrMigrateRule<PersistenceRules>();

// builder.Logging.ClearProviders();
builder.Logging.AddBackwardFarewellLogger();

var app = builder.Build();

var scopeProvider = new FarewellScopeProvider(app.Services);

scopeProvider.UseStateRules();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();