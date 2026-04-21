FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["shared/Shared.Models/Shared.Models.csproj", "shared/Shared.Models/"]
COPY ["shared/Shared.Services/Shared.Services.csproj", "shared/Shared.Services/"]
COPY ["services/IdentityService/src/IdentityService.csproj", "services/IdentityService/src/"]
COPY ["services/IdentityService/tests/IdentityService.Tests/IdentityService.Tests.csproj", "services/IdentityService/tests/IdentityService.Tests/"]
RUN dotnet restore "services/IdentityService/src/IdentityService.csproj"
RUN dotnet restore "services/IdentityService/tests/IdentityService.Tests/IdentityService.Tests.csproj"

COPY ["shared/Shared.Models/", "shared/Shared.Models/"]
COPY ["shared/Shared.Services/", "shared/Shared.Services/"]
COPY ["services/IdentityService/src/", "services/IdentityService/src/"]
COPY ["services/IdentityService/tests/IdentityService.Tests/", "services/IdentityService/tests/IdentityService.Tests/"]

FROM build AS test
RUN dotnet test "services/IdentityService/tests/IdentityService.Tests/IdentityService.Tests.csproj" \
    --no-restore \
    --logger "console;verbosity=normal"

FROM test AS publish
RUN dotnet publish "services/IdentityService/src/IdentityService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=publish /app/publish ./
ENTRYPOINT ["dotnet", "IdentityService.dll"]
