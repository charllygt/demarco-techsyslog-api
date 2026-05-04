# syntax=docker/dockerfile:1.6
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["TechsysLog.sln", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["NuGet.config", "./"]
COPY ["global.json", "./"]
COPY ["src/TechsysLog.Domain/TechsysLog.Domain.csproj", "src/TechsysLog.Domain/"]
COPY ["src/TechsysLog.Application/TechsysLog.Application.csproj", "src/TechsysLog.Application/"]
COPY ["src/TechsysLog.Infrastructure/TechsysLog.Infrastructure.csproj", "src/TechsysLog.Infrastructure/"]
COPY ["src/TechsysLog.Api/TechsysLog.Api.csproj", "src/TechsysLog.Api/"]
RUN dotnet restore "src/TechsysLog.Api/TechsysLog.Api.csproj"

COPY src/ src/
RUN dotnet publish "src/TechsysLog.Api/TechsysLog.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && groupadd --system --gid 1001 app \
    && useradd --system --uid 1001 --gid app app

COPY --from=build --chown=app:app /app/publish .
USER app

EXPOSE 8080
HEALTHCHECK --interval=15s --timeout=5s --retries=5 --start-period=30s \
    CMD curl -f http://localhost:8080/swagger/v1/swagger.json || exit 1

ENTRYPOINT ["dotnet", "TechsysLog.Api.dll"]
