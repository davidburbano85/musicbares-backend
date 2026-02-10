# Imagen base para ejecutar la app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Dependencia necesaria para PostgreSQL / Supabase
RUN apt-get update && apt-get install -y libgssapi-krb5-2



# Imagen de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solo el csproj primero (MEJORA CACHE Y RESTORE)
COPY *.csproj ./
RUN dotnet restore

# Copiar todo el proyecto
COPY . ./

# Publicar aplicación
RUN dotnet publish -c Release -o /app/publish



# Imagen final
FROM base AS final
WORKDIR /app

# Copiar build publicado
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MusicBares.dll"]
