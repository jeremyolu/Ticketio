var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(
    $"Auth destination: {builder.Configuration["ReverseProxy:Clusters:AuthCluster:Destinations:AuthApi:Address"]}"
);

Console.WriteLine(
    $"Auth route: {builder.Configuration["ReverseProxy:Routes:AuthRoute:Match:Path"]}"
);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapReverseProxy();

app.Run();