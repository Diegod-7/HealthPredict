# 🔗 Configuración Oracle Cloud para HealthPredict

## Connection Strings Disponibles:

### Para Producción (Recomendado):
```
Data Source=(description= (retry_count=20)(retry_delay=3)(address=(protocol=tcps)(port=1522)(host=adb.sa-saopaulo-1.oraclecloud.com))(connect_data=(service_name=g993e4289ace002_healthpredict_low.adb.oraclecloud.com))(security=(ssl_server_dn_match=yes)));User Id=ADMIN;Password=TU_PASSWORD_AQUI;
```

### Para Testing/Development:
```
Data Source=(description= (retry_count=20)(retry_delay=3)(address=(protocol=tcps)(port=1522)(host=adb.sa-saopaulo-1.oraclecloud.com))(connect_data=(service_name=g993e4289ace002_healthpredict_medium.adb.oraclecloud.com))(security=(ssl_server_dn_match=yes)));User Id=ADMIN;Password=TU_PASSWORD_AQUI;
```

## Variables de Entorno para Render:

```
ORACLE_CONNECTION_STRING=Data Source=(description= (retry_count=20)(retry_delay=3)(address=(protocol=tcps)(port=1522)(host=adb.sa-saopaulo-1.oraclecloud.com))(connect_data=(service_name=g993e4289ace002_healthpredict_low.adb.oraclecloud.com))(security=(ssl_server_dn_match=yes)));User Id=ADMIN;Password=TU_PASSWORD_REAL;
```

## Explicación de los Servicios:

- **healthpredict_low**: Menor prioridad, ideal para producción normal
- **healthpredict_medium**: Prioridad media, bueno para testing
- **healthpredict_high**: Alta prioridad, para cargas críticas
- **healthpredict_tp**: Transaction Processing optimizado
- **healthpredict_tpurgent**: Para transacciones urgentes

## Próximos Pasos:

1. Reemplazar `TU_PASSWORD_AQUI` con tu password real de ADMIN
2. Configurar la variable de entorno en Render
3. Redeploy de la aplicación 