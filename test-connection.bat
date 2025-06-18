@echo off
echo 🔍 Configurando variables de entorno...
set ORACLE_CONNECTION_STRING=Data Source=(description= (retry_count=20)(retry_delay=3)(address=(protocol=tcps)(port=1522)(host=adb.sa-saopaulo-1.oraclecloud.com))(connect_data=(service_name=g993e4289ace002_healthpredict_low.adb.oraclecloud.com))(security=(ssl_server_dn_match=yes)));User Id=ADMIN;Password=y4mti4edyxEWzwU;

echo ✅ Variable configurada
echo 🚀 Iniciando aplicación...
dotnet run --urls "http://localhost:5000" 