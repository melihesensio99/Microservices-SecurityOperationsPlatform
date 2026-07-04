FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/BuildingBlocks/SecurityPlatform.BuildingBlocks/SecurityPlatform.BuildingBlocks.csproj", "src/BuildingBlocks/SecurityPlatform.BuildingBlocks/"]
COPY ["src/SecurityCore/SecurityCore.Api/SecurityCore.Api.csproj", "src/SecurityCore/SecurityCore.Api/"]
RUN dotnet restore "src/SecurityCore/SecurityCore.Api/SecurityCore.Api.csproj"

COPY . .
WORKDIR /src/src/SecurityCore/SecurityCore.Api
RUN dotnet publish "SecurityCore.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SecurityCore.Api.dll"]
