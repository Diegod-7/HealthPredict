#!/bin/bash
set -e

echo "🚀 Iniciando build para Render..."

# Restaurar dependencias
echo "📦 Restaurando dependencias..."
dotnet restore HealthPredict.API/HealthPredict.API.csproj

# Build de la aplicación
echo "🔨 Compilando aplicación..."
dotnet build HealthPredict.API/HealthPredict.API.csproj -c Release

echo "✅ Build completado exitosamente!" 