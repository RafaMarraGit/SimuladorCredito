FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Garante permissão de escrita para o usuário da aplicação
ARG APP_UID
RUN chown -R ${APP_UID:-1000}:${APP_UID:-1000} /app

USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
WORKDIR "/src"
RUN dotnet restore "SimuladorCredito.csproj"
RUN dotnet publish "SimuladorCredito.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SimuladorCredito.dll"]
