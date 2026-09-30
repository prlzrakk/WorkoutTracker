FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY WorkoutTracker.Api/WorkoutTracker.Api.csproj WorkoutTracker.Api/
COPY WorkoutTracker.Core/WorkoutTracker.Core.csproj WorkoutTracker.Core/

RUN dotnet restore WorkoutTracker.Api/WorkoutTracker.Api.csproj

COPY . .

RUN dotnet publish WorkoutTracker.Api/WorkoutTracker.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "WorkoutTracker.Api.dll"]