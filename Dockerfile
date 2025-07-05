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

# Instalar Python 3 y dependencias
RUN apt-get update && apt-get install -y \
    python3 \
    python3-pip \
    python3-venv \
    libgdiplus \
    libc6-dev \
    && rm -rf /var/lib/apt/lists/*

# Instalar dependencias de Python para el script
RUN pip3 install --no-cache-dir \
    google-auth \
    google-auth-oauthlib \
    google-auth-httplib2 \
    google-api-python-client \
    requests

ENTRYPOINT ["dotnet", "HealthPredict.API.dll"] 