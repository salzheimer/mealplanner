FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["shared/Shared.Models/Shared.Models.csproj", "shared/Shared.Models/"]
COPY ["shared/Shared.Services/Shared.Services.csproj", "shared/Shared.Services/"]
COPY ["services/ApiGateway/src/ApiGateway.csproj", "services/ApiGateway/src/"]
COPY ["services/ApiGateway/tests/ApiGateway.Tests/ApiGateway.Tests.csproj", "services/ApiGateway/tests/ApiGateway.Tests/"]
RUN dotnet restore "services/ApiGateway/src/ApiGateway.csproj"
RUN dotnet restore "services/ApiGateway/tests/ApiGateway.Tests/ApiGateway.Tests.csproj"

COPY ["shared/Shared.Models/", "shared/Shared.Models/"]
COPY ["shared/Shared.Services/", "shared/Shared.Services/"]
COPY ["services/ApiGateway/src/", "services/ApiGateway/src/"]
COPY ["services/ApiGateway/tests/ApiGateway.Tests/", "services/ApiGateway/tests/ApiGateway.Tests/"]

FROM build AS test
RUN dotnet test "services/ApiGateway/tests/ApiGateway.Tests/ApiGateway.Tests.csproj" \
    --no-restore \
    --logger "console;verbosity=normal"

FROM test AS publish
RUN dotnet publish "services/ApiGateway/src/ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=publish /app/publish ./
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
