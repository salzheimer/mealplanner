FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["shared/Shared.Models/Shared.Models.csproj", "shared/Shared.Models/"]
COPY ["services/MealRecipeService/src/MealRecipeService.csproj", "services/MealRecipeService/src/"]
COPY ["services/MealRecipeService/tests/MealRecipeService.Tests/MealRecipeService.Tests.csproj", "services/MealRecipeService/tests/MealRecipeService.Tests/"]
RUN dotnet restore "services/MealRecipeService/src/MealRecipeService.csproj"
RUN dotnet restore "services/MealRecipeService/tests/MealRecipeService.Tests/MealRecipeService.Tests.csproj"

COPY ["shared/Shared.Models/", "shared/Shared.Models/"]
COPY ["services/MealRecipeService/src/", "services/MealRecipeService/src/"]
COPY ["services/MealRecipeService/tests/MealRecipeService.Tests/", "services/MealRecipeService/tests/MealRecipeService.Tests/"]

FROM build AS test
RUN dotnet test "services/MealRecipeService/tests/MealRecipeService.Tests/MealRecipeService.Tests.csproj" \
    --no-restore \
    --logger "console;verbosity=normal"

FROM test AS publish
RUN dotnet publish "services/MealRecipeService/src/MealRecipeService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=publish /app/publish ./
ENTRYPOINT ["dotnet", "MealRecipeService.dll"]
