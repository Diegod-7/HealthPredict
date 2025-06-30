# Script para probar diferentes formatos de Oracle Connection String

Write-Host "Probando diferentes formatos de Oracle Connection String..." -ForegroundColor Green

# Formato 1: Simplificado con TP service
$format1 = 'Data Source=adb.sa-saopaulo-1.oraclecloud.com:1522/g993e4289ace002_healthpredict_tp.adb.oraclecloud.com;User Id=ADMIN;Password=y4mti4edyxEWzwU;SSL Mode=Require;'

# Formato 2: Simplificado con LOW service  
$format2 = 'Data Source=adb.sa-saopaulo-1.oraclecloud.com:1522/g993e4289ace002_healthpredict_low.adb.oraclecloud.com;User Id=ADMIN;Password=y4mti4edyxEWzwU;SSL Mode=Require;'

# Formato 3: TNS completo sin SSL verification
$format3 = 'Data Source=(description= (retry_count=20)(retry_delay=3)(address=(protocol=tcps)(port=1522)(host=adb.sa-saopaulo-1.oraclecloud.com))(connect_data=(service_name=g993e4289ace002_healthpredict_tp.adb.oraclecloud.com)));User Id=ADMIN;Password=y4mti4edyxEWzwU;'

Write-Host "Formatos a probar en Render:" -ForegroundColor Yellow
Write-Host "1. TP Service (Recomendado):" -ForegroundColor Cyan
Write-Host $format1
Write-Host ""
Write-Host "2. LOW Service:" -ForegroundColor Cyan  
Write-Host $format2
Write-Host ""
Write-Host "3. TNS sin SSL:" -ForegroundColor Cyan
Write-Host $format3
Write-Host ""
Write-Host "Copia uno de estos formatos y pruebalo en Render" -ForegroundColor Green 