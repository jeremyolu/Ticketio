using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Ticketio.Core.Configuration;
using Ticketio.Core.Factories;
using Ticketio.Core.Interfaces.Factories;

namespace Ticketio.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApiOptions(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressMapClientErrors = true;
        });
    }

    public static void AddTokenConfig(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        var section = configuration.GetSection(sectionName ?? "Jwt");

        services.Configure<JwtConfig>(section);

        var jwtSettings = section.Get<JwtConfig>() ?? 
            throw new InvalidOperationException("JWT configuration is missing.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                ClockSkew = TimeSpan.Zero
            };
        });
    }

    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        var connString = configuration.GetConnectionString(sectionName ?? "TicketioDb") ??
            throw new InvalidOperationException("Connectionstring section for TicketioDb is missing.");

        services.AddSingleton<IDbConnectionFactory>(x => new SqlConnectionFactory(connString));
    }
}
