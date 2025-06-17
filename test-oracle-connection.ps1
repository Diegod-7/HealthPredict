# Script para probar conexión a Oracle Cloud
$connectionString = "Data Source=(description= (retry_count=20)(retry_delay=3)(address=(protocol=tcps)(port=1522)(host=adb.sa-saopaulo-1.oraclecloud.com))(connect_data=(service_name=g993e4289ace002_healthpredict_low.adb.oraclecloud.com))(security=(ssl_server_dn_match=yes)));User Id=ADMIN;Password=y4mti4edyxEWzwU;"

Write-Host "🔍 Probando conexión a Oracle Cloud..."
Write-Host "Connection String configurado: $($connectionString.Length) caracteres"

# Configurar variable de entorno temporalmente
$env:ORACLE_CONNECTION_STRING = $connectionString

Write-Host "✅ Variable de entorno configurada"
Write-Host "🚀 Ejecutando aplicación para probar conexión..."

# Ejecutar la aplicación
dotnet run 