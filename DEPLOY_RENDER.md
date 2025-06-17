# 🚀 Deploy de HealthPredict Backend en Render

## Pasos para Deploy

### 1. Preparar el Repositorio

```bash
# Commit todos los cambios
git add .
git commit -m "Configurar para deploy en Render"
git push origin main
```

### 2. Crear cuenta en Render

1. Ve a [render.com](https://render.com)
2. Regístrate con tu cuenta de GitHub
3. Conecta tu repositorio

### 3. Configurar Web Service

1. **New** → **Web Service**
2. **Connect** tu repositorio HealthPredict
3. Configurar:
   - **Name**: `healthpredict-api`
   - **Environment**: `Docker`
   - **Region**: `Oregon (US West)`
   - **Branch**: `main`
   - **Dockerfile Path**: `./Dockerfile`

### 4. Variables de Entorno

En la sección **Environment Variables** agregar:

```
ASPNETCORE_ENVIRONMENT=Production
ORACLE_CONNECTION_STRING=tu_string_de_conexion_oracle
```

### 5. Configurar Base de Datos

**Opción A: Oracle Cloud (Recomendado)**
```
ORACLE_CONNECTION_STRING=Data Source=tu_oracle_cloud_instance;User Id=tu_usuario;Password=tu_password;
```

**Opción B: PostgreSQL en Render (Alternativa)**
1. **New** → **PostgreSQL**
2. **Name**: `healthpredict-db`
3. Copiar la **External Database URL**
4. Modificar el código para usar PostgreSQL

### 6. Deploy

1. Click **Create Web Service**
2. Render automáticamente:
   - Clona tu repositorio
   - Construye la imagen Docker
   - Despliega la aplicación

### 7. Verificar Deploy

- URL de la API: `https://healthpredict-api.onrender.com`
- Swagger UI: `https://healthpredict-api.onrender.com/swagger`

## 🔧 Configuración Adicional

### Actualizar CORS

En `appsettings.Production.json`, actualizar:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://tu-frontend-url.com",
      "https://healthpredict-api.onrender.com"
    ]
  }
}
```

### Monitoreo

- **Logs**: Panel de Render → Services → healthpredict-api → Logs
- **Métricas**: Panel de Render → Services → healthpredict-api → Metrics

## 🆘 Troubleshooting

### Error de Puerto
- Render asigna automáticamente el puerto via variable `PORT`
- El código ya está configurado para esto

### Error de Base de Datos
- Verificar string de conexión
- Verificar que la base de datos esté accesible desde internet

### Error de Build
- Revisar logs en Render
- Verificar que el Dockerfile sea correcto

## 📝 Notas Importantes

1. **Plan Gratuito**: 750 horas/mes, se duerme después de 15 min de inactividad
2. **Dominio**: `https://tu-servicio.onrender.com`
3. **SSL**: Automático y gratuito
4. **Deploy Automático**: Se redeploya automáticamente en cada push a main 