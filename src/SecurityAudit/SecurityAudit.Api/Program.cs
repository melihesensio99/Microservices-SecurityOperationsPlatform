using SecurityAudit.Api.Infrastructure;
using SecurityPlatform.BuildingBlocks.DependencyInjection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSecurityPlatformTracing(builder.Configuration, "SecurityAudit");
builder.Services.AddSecurityAuditServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseMiddleware<CorrelationIdMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
    dbContext.Database.EnsureCreated();
}

app.MapAuditLogEndpoints();
app.MapSecurityPlatformHealthEndpoints();

app.Run();
