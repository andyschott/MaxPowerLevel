FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY Destiny2/Destiny2.csproj Destiny2/
COPY MaxPowerLevel/MaxPowerLevel.csproj MaxPowerLevel/
RUN dotnet restore MaxPowerLevel/MaxPowerLevel.csproj
COPY . .
WORKDIR /src/MaxPowerLevel/
RUN dotnet build MaxPowerLevel.csproj -c Release -o /app

FROM build as publish
RUN dotnet publish MaxPowerLevel.csproj -c Release -o /app

FROM base AS final
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MaxPowerLevel.dll"]
