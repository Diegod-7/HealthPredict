# 🔧 Troubleshooting Oracle Cloud Autonomous Database

## Problemas Comunes y Soluciones

### 1. **Wallet/Certificados SSL**
Oracle Autonomous Database requiere certificados SSL específicos.

**Solución:**
- Descargar el wallet desde Oracle Cloud Console
- Configurar certificados en la aplicación

### 2. **Access Control List (ACL)**
Oracle Cloud puede estar bloqueando conexiones desde Render.

**Verificar en Oracle Cloud Console:**
1. Database → Autonomous Database → Tu DB
2. **Network** → **Access Control List**
3. Verificar que esté configurado como "Secure access from everywhere"

### 3. **Service Name Incorrecto**
El service name puede necesitar ajustes.

**Probar estos service names alternativos:**
- `healthpredict_tp` (Transaction Processing)
- `healthpredict_medium` (Medium priority)
- `healthpredict_high` (High priority)

### 4. **Formato del Connection String**
Probar formato alternativo:

```
Data Source=adb.sa-saopaulo-1.oraclecloud.com:1522/g993e4289ace002_healthpredict_low.adb.oraclecloud.com;User Id=ADMIN;Password=y4mti4edyxEWzwU;SSL Mode=Require;
```

### 5. **Usuario y Permisos**
Verificar que el usuario ADMIN tenga permisos correctos.

## Solución Rápida: Cambiar a PostgreSQL

Si Oracle sigue dando problemas, cambiar a PostgreSQL en Render:

1. **Crear PostgreSQL Database en Render**
2. **Cambiar el paquete NuGet** a Npgsql
3. **Actualizar connection string**
4. **Deploy automático**

¿Cuál prefieres: seguir con Oracle o cambiar a PostgreSQL? 