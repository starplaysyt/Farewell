using Farewell.Compatibility.MS.DependencyInjection;
using Farewell.Compatibility.MS.Logging;
using Farewell.Debug.Infrastructure;
using FastEndpoints;
using FastEndpoints.Swagger;
using NSwag;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddFastEndpoints()
    .AddEndpointsApiExplorer()
    .AddSwaggerDocument();

// That should add all needed services.
new FarewellBuilder(builder.Services).AddInfrastructure();

builder.Logging.ClearProviders();
builder.Logging.AddForwardFarewellLogger();

var app = builder.Build();

// And that will execute infrastructure initialization.
new FarewellScopeProvider(app.Services).UseInfrastructure();

app.UseFastEndpoints();
app.UseSwaggerGen(c =>
{
    c.PostProcess = (doc, req) =>
    {
        if (req.Path.StartsWithSegments("/swagger"))
            File.WriteAllText("openapi.yml", doc.ToYaml());
    };
});

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