# base (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Installer openssl for å generere sertifikater
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

# Kopier entrypoint script til en plassering vi kan bruke senere
RUN cp "/src/OBLIG1-Prosjekt/docker-entrypoint.sh" /tmp/docker-entrypoint.sh && chmod +x /tmp/docker-entrypoint.sh

# publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OBLIG1-Prosjekt.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# final
FROM base AS final
WORKDIR /app

# Kopier entrypoint script fra build stage
COPY --from=build /tmp/docker-entrypoint.sh /usr/local/bin/docker-entrypoint.sh
RUN chmod +x /usr/local/bin/docker-entrypoint.sh

COPY --from=publish /app/publish .
ENTRYPOINT ["/usr/local/bin/docker-entrypoint.sh", "OBLIG1-Prosjekt.dll"]
