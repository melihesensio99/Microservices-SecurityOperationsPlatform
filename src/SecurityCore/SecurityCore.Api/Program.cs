using SecurityCore.Api.Infrastructure;
using SecurityPlatform.BuildingBlocks.DependencyInjection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSecurityPlatformTracing(builder.Configuration, "SecurityCore");
builder.Services.AddSecurityCoreServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseMiddleware<CorrelationIdMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SecurityCoreDbContext>();
    dbContext.Database.EnsureCreated();
}

app.MapIncidentEndpoints();
app.MapSecurityPlatformHealthEndpoints();

app.Run();
