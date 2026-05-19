# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos de proyecto
COPY Biblioteca/SistemaAcademico.csproj Biblioteca/
COPY Front/Front.csproj Front/

# Restaurar dependencias
RUN dotnet restore Front/Front.csproj

# Copiar todo el código
COPY . .

# Publicar la app
RUN dotnet publish Front/Front.csproj -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "Front.dll"]
