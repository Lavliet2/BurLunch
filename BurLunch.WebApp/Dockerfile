# Этап для быстрого режима отладки (по умолчанию для конфигурации Debug)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Этап сборки проекта
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BurLunch.WebApp/BurLunch.WebApp.csproj", "BurLunch.WebApp/"]
RUN dotnet restore "./BurLunch.WebApp/BurLunch.WebApp.csproj"
COPY . .
WORKDIR "/src/BurLunch.WebApp"
RUN dotnet build "./BurLunch.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этап публикации проекта
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BurLunch.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный этап для создания рабочего контейнера
FROM base AS final
WORKDIR /app
RUN apt update && apt install -y curl
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BurLunch.WebApp.dll"]
