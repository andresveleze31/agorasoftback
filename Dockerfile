# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar csproj y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar todo el código
COPY . ./
RUN dotnet publish -c Release -o out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Exponer el puerto que usa tu app (según launchSettings.json)
EXPOSE 5165

# Variable de entorno
ENV ASPNETCORE_URLS=http://+:5165
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "backend.dll"]
