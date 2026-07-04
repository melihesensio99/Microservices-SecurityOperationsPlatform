using SecurityCore.Api.Features.Incidents;
using SecurityCore.Api.Features.Incidents.AddNote;
using SecurityCore.Api.Features.Incidents.Abstractions;
using SecurityCore.Api.Features.Incidents.Create;
using SecurityCore.Api.Features.Incidents.GetById;
using SecurityCore.Api.Features.Incidents.GetList;
using SecurityCore.Api.Features.Incidents.GetNotes;
using SecurityCore.Api.Infrastructure.Persistence;
using SecurityCore.Api.Infrastructure.Errors;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddDbContext<SecurityCoreDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SecurityDb")));
builder.Services.AddScoped<IIncidentRepository, EfIncidentRepository>();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblyContaining<CreateIncidentCommand>();
    config.AddOpenBehavior(typeof(SecurityPlatform.BuildingBlocks.Diagnostics.LoggingPipelineBehavior<,>));
    config.AddOpenBehavior(typeof(SecurityPlatform.BuildingBlocks.Validation.ValidationPipelineBehavior<,>));
});
builder.Services.AddValidatorsFromAssemblyContaining<CreateIncidentRequestValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SecurityCoreDbContext>();
    dbContext.Database.EnsureCreated();
}

app.MapIncidentEndpoints();

app.Run();
