using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SecurityAuth.Api.Features.Auth.Login;
using SecurityAuth.Api.Features.Auth.Register;
using SecurityAuth.Api.Infrastructure.Errors;
using SecurityAuth.Api.Infrastructure.Persistence;
using SecurityAuth.Api.Infrastructure.UserContext;
using SecurityPlatform.BuildingBlocks.DependencyInjection;
using System.Text;

namespace SecurityAuth.Api.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecurityAuthServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("AuthDb")));
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IUserStore, EfUserStore>();
        services.AddSingleton<PasswordHasher<ApplicationUser>>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwt = configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });
        services.AddAuthorization();
        services.AddSecurityPlatformMediatR(typeof(RegisterUserCommand).Assembly);

        return services;
    }
}
