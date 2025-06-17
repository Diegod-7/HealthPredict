# Usar la imagen base de .NET 7.0
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Usar la imagen SDK para build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["HealthPredict.API/HealthPredict.API.csproj", "HealthPredict.API/"]
COPY ["HealthPredict.BLL/HealthPredict.BLL.csproj", "HealthPredict.BLL/"]
COPY ["HealthPredict.DAL/HealthPredict.DAL.csproj", "HealthPredict.DAL/"]
COPY ["HealthPredict.Models/HealthPredict.Models.csproj", "HealthPredict.Models/"]

RUN dotnet restore "HealthPredict.API/HealthPredict.API.csproj"

# Copiar todo el código fuente
COPY . .

# Build de la aplicación
WORKDIR "/src/HealthPredict.API"
RUN dotnet build "HealthPredict.API.csproj" -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish "HealthPredict.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Instalar dependencias para wkhtmltopdf si es necesario
RUN apt-get update && apt-get install -y \
    libgdiplus \
    libc6-dev \
    && rm -rf /var/lib/apt/lists/*

ENTRYPOINT ["dotnet", "HealthPredict.API.dll"] 