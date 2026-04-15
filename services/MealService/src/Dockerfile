FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["shared/Shared.Models/Shared.Models.csproj", "shared/Shared.Models/"]
COPY ["services/MealService/src/MealService.csproj", "services/MealService/src/"]
COPY ["services/MealService/tests/MealService.Tests/MealService.Tests.csproj", "services/MealService/tests/MealService.Tests/"]
RUN dotnet restore "services/MealService/src/MealService.csproj"
RUN dotnet restore "services/MealService/tests/MealService.Tests/MealService.Tests.csproj"

COPY ["shared/Shared.Models/", "shared/Shared.Models/"]
COPY ["services/MealService/src/", "services/MealService/src/"]
COPY ["services/MealService/tests/MealService.Tests/", "services/MealService/tests/MealService.Tests/"]

FROM build AS test
RUN dotnet test "services/MealService/tests/MealService.Tests/MealService.Tests.csproj" \
    --no-restore \
    --logger "console;verbosity=normal"

FROM test AS publish
RUN dotnet publish "services/MealService/src/MealService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=publish /app/publish ./
ENTRYPOINT ["dotnet", "MealService.dll"]
