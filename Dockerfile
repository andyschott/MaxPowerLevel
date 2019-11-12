FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY MaxPowerLevel/MaxPowerLevel.csproj MaxPowerLevel/
RUN dotnet restore MaxPowerLevel/MaxPowerLevel.csproj

# copy everything else and build app
COPY MaxPowerLevel/. ./MaxPowerLevel/
WORKDIR /app/MaxPowerLevel
RUN dotnet publish MaxPowerLevel.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /app
COPY --from=build /app/MaxPowerLevel/out ./
ENTRYPOINT ["dotnet", "MaxPowerLevel.dll"]