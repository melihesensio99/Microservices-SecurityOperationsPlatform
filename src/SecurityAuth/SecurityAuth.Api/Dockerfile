FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/BuildingBlocks/SecurityPlatform.BuildingBlocks/SecurityPlatform.BuildingBlocks.csproj", "src/BuildingBlocks/SecurityPlatform.BuildingBlocks/"]
COPY ["src/SecurityAuth/SecurityAuth.Api/SecurityAuth.Api.csproj", "src/SecurityAuth/SecurityAuth.Api/"]
RUN dotnet restore "src/SecurityAuth/SecurityAuth.Api/SecurityAuth.Api.csproj"

COPY . .
WORKDIR /src/src/SecurityAuth/SecurityAuth.Api
RUN dotnet publish "SecurityAuth.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SecurityAuth.Api.dll"]
