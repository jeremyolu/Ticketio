using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Ticketio.Auth.Api.Config;
using Ticketio.Auth.Api.Interfaces.Repositories;
using Ticketio.Auth.Api.Interfaces.Services;
using Ticketio.Auth.Api.Repositories;
using Ticketio.Auth.Api.Services;
using Ticketio.Core.Factories;
using Ticketio.Core.Interfaces.Factories;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;

        var connString = configuration.GetConnectionString("TicketioDb") ??
            throw new InvalidOperationException("Connectionstring section for TicketioDb is missing.");

        var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ??
            throw new InvalidOperationException("Jwt configuration section is missing.");

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.AddSingleton(jwtSettings);

        builder.Services.AddSingleton<IDbConnectionFactory>(x => new SqlConnectionFactory(connString));

        builder.Services.AddScoped<IAuthRepository, AuthRepository>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddAuthentication(options =>
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

        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressMapClientErrors = true;
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}