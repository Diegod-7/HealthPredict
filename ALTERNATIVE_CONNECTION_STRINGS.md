# 🔗 Connection Strings Alternativos para Render

## Opción 1: Formato Simplificado
```
ORACLE_CONNECTION_STRING=Data Source=adb.sa-saopaulo-1.oraclecloud.com:1522/g993e4289ace002_healthpredict_low.adb.oraclecloud.com;User Id=ADMIN;Password=y4mti4edyxEWzwU;SSL Mode=Require;
```

## Opción 2: Service TP (Transaction Processing)
```
ORACLE_CONNECTION_STRING=Data Source=(description= (retry_count=20)(retry_delay=3)(address=(protocol=tcps)(port=1522)(host=adb.sa-saopaulo-1.oraclecloud.com))(connect_data=(service_name=g993e4289ace002_healthpredict_tp.adb.oraclecloud.com))(security=(ssl_server_dn_match=yes)));User Id=ADMIN;Password=y4mti4edyxEWzwU;
```

## Opción 3: Service Medium
```
ORACLE_CONNECTION_STRING=Data Source=(description= (retry_count=20)(retry_delay=3)(address=(protocol=tcps)(port=1522)(host=adb.sa-saopaulo-1.oraclecloud.com))(connect_data=(service_name=g993e4289ace002_healthpredict_medium.adb.oraclecloud.com))(security=(ssl_server_dn_match=yes)));User Id=ADMIN;Password=y4mti4edyxEWzwU;
```

## Opción 4: Sin SSL Verification
```
ORACLE_CONNECTION_STRING=Data Source=(description= (retry_count=20)(retry_delay=3)(address=(protocol=tcps)(port=1522)(host=adb.sa-saopaulo-1.oraclecloud.com))(connect_data=(service_name=g993e4289ace002_healthpredict_low.adb.oraclecloud.com)));User Id=ADMIN;Password=y4mti4edyxEWzwU;
```

## 🚀 Instrucciones:
1. Prueba cada opción en Render (Environment Variables)
2. Redeploy después de cada cambio
3. Revisa los logs para ver cuál funciona 