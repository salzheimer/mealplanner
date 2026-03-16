FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["shared/Shared.Models/Shared.Models.csproj", "shared/Shared.Models/"]
COPY ["services/PlanService/src/src.csproj", "services/PlanService/src/"]
RUN dotnet restore "services/PlanService/src/src.csproj"

COPY ["shared/Shared.Models/", "shared/Shared.Models/"]
COPY ["services/PlanService/src/", "services/PlanService/src/"]
RUN dotnet publish "services/PlanService/src/src.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "src.dll"]
