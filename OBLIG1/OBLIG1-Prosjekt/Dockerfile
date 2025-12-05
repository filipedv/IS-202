# base (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Installer openssl for sertifikatgenerering
RUN apt-get update && apt-get install -y openssl && rm -rf /var/lib/apt/lists/*

# Lytt på både HTTP (8080) og HTTPS (8443)
ENV ASPNETCORE_URLS=http://+:8080;https://+:8443

EXPOSE 8080
EXPOSE 8443

# build 
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Kopier kun csproj først for bedre cache
COPY ["OBLIG1-Prosjekt/OBLIG1-Prosjekt.csproj", "OBLIG1-Prosjekt/"]
RUN dotnet restore "OBLIG1-Prosjekt/OBLIG1-Prosjekt.csproj"

# Kopier resten og bygg
COPY . .
WORKDIR "/src/OBLIG1-Prosjekt"
RUN dotnet build "OBLIG1-Prosjekt.csproj" -c $BUILD_CONFIGURATION -o /app/build

# publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OBLIG1-Prosjekt.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# final
FROM base AS final
WORKDIR /app

# Kopier entrypoint script
COPY ["OBLIG1-Prosjekt/entrypoint.sh", "/entrypoint.sh"]
RUN chmod +x /entrypoint.sh

COPY --from=publish /app/publish .
ENTRYPOINT ["/entrypoint.sh"]
