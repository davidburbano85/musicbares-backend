# -------------------------
# Imagen base para ejecución
# -------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# -------------------------
# Imagen para compilación
# -------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos csproj
COPY ["MusicBares.API/MusicBares.API.csproj", "MusicBares.API/"]
COPY ["MusicBares.Application/MusicBares.Application.csproj", "MusicBares.Application/"]
COPY ["MusicBares.DTOs/MusicBares.DTOs.csproj", "MusicBares.DTOs/"]
COPY ["MusicBares.Entidades/MusicBares.Entidades.csproj", "MusicBares.Entidades/"]
COPY ["MusicBares.Infrastructure/MusicBares.Infrastructure.csproj", "MusicBares.Infrastructure/"]

# Restaurar dependencias
RUN dotnet restore "MusicBares.API/MusicBares.API.csproj"

# Copiar todo el código
COPY . .

WORKDIR "/src/MusicBares.API"
RUN dotnet build "MusicBares.API.csproj" -c Release -o /app/build

# -------------------------
# Publicación
# -------------------------
FROM build AS publish
RUN dotnet publish "MusicBares.API.csproj" -c Release -o /app/publish

# -------------------------
# Imagen final
# -------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "MusicBares.API.dll"]
