# Build stage

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore "src/backend/Ticketio.Auth.Api/Ticketio.Auth.Api.csproj"

RUN dotnet publish "src/backend/Ticketio.Auth.Api/Ticketio.Auth.Api.csproj" -c Release -o /app/publish --no-restore

# Runtime stage

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "Ticketio.Auth.Api.dll"]