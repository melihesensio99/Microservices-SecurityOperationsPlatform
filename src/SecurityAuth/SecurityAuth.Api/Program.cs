using SecurityAuth.Api.Infrastructure;
using SecurityPlatform.BuildingBlocks.DependencyInjection;
using SecurityAuth.Api.Infrastructure.Persistence;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSecurityPlatformTracing(builder.Configuration, "SecurityAuth");
builder.Services.AddSecurityAuthServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    dbContext.Database.EnsureCreated();
}

app.MapAuthEndpoints();
app.MapSecurityPlatformHealthEndpoints();

app.Run();
