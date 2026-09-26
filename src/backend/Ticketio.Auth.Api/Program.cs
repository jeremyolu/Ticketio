using Ticketio.Auth.Api.Interfaces.Repositories;
using Ticketio.Auth.Api.Interfaces.Services;
using Ticketio.Auth.Api.Repositories;
using Ticketio.Auth.Api.Services;
using Ticketio.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddDatabase(configuration);
builder.Services.AddApiOptions();
builder.Services.AddTokenConfig(configuration);

builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();