FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
RUN apt-get update && apt-get install -y ruby-sass
WORKDIR /app

# copy everything and build app
COPY MaxPowerLevel/. ./MaxPowerLevel/
WORKDIR /app/MaxPowerLevel
RUN dotnet publish MaxPowerLevel.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/MaxPowerLevel/out ./
ENTRYPOINT ["dotnet", "MaxPowerLevel.dll"]
