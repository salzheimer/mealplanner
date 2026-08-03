FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["shared/Shared.Models/Shared.Models.csproj", "shared/Shared.Models/"]
COPY ["shared/Shared.Services/Shared.Services.csproj", "shared/Shared.Services/"]
COPY ["services/GatewayBff/src/GatewayBff.csproj", "services/GatewayBff/src/"]
COPY ["services/GatewayBff/tests/GatewayBff.Tests/GatewayBff.Tests.csproj", "services/GatewayBff/tests/GatewayBff.Tests/"]
RUN dotnet restore "services/GatewayBff/src/GatewayBff.csproj"
RUN dotnet restore "services/GatewayBff/tests/GatewayBff.Tests/GatewayBff.Tests.csproj"

COPY ["shared/Shared.Models/", "shared/Shared.Models/"]
COPY ["shared/Shared.Services/", "shared/Shared.Services/"]
COPY ["services/GatewayBff/src/", "services/GatewayBff/src/"]
COPY ["services/GatewayBff/tests/GatewayBff.Tests/", "services/GatewayBff/tests/GatewayBff.Tests/"]

FROM build AS test
RUN dotnet test "services/GatewayBff/tests/GatewayBff.Tests/GatewayBff.Tests.csproj" \
    --no-restore \
    --logger "console;verbosity=normal"

FROM test AS publish
RUN dotnet publish "services/GatewayBff/src/GatewayBff.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=publish /app/publish ./
ENTRYPOINT ["dotnet", "GatewayBff.dll"]
