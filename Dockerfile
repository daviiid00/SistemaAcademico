# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Biblioteca/SistemaAcademico.csproj Biblioteca/
COPY Front/Front.csproj Front/
RUN dotnet restore Front/Front.csproj

COPY . .
RUN dotnet publish Front/Front.csproj -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

# CMD en shell form para que $PORT se expanda en tiempo de ejecucion
CMD dotnet Front.dll --urls "http://0.0.0.0:${PORT:-8080}"
