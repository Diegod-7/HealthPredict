# 🐘 Migración Rápida a PostgreSQL

## ¿Por qué PostgreSQL?
- ✅ **Funciona inmediatamente** en Render
- ✅ **Gratis** y confiable
- ✅ **Sin configuración SSL compleja**
- ✅ **Deploy en 5 minutos**

## Pasos de Migración:

### 1. Crear PostgreSQL en Render
1. **Render Dashboard** → **New** → **PostgreSQL**
2. **Name**: `healthpredict-db`
3. **Plan**: **Free**
4. **Create Database**
5. **Copiar External Database URL**

### 2. Actualizar Código .NET

#### Cambiar paquete NuGet:
```bash
cd HealthPredict.API
dotnet remove package Oracle.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

#### Actualizar Program.cs:
```csharp
// Cambiar de Oracle a PostgreSQL
builder.Services.AddDbContext<HealthPredictContext>(options => 
    options.UseNpgsql(connectionString));
```

#### Variable de entorno en Render:
```
POSTGRESQL_CONNECTION_STRING=postgresql://user:password@host:port/database
```

### 3. Recrear Migraciones
```bash
rm -rf Migrations/
dotnet ef migrations add InitialCreate
```

### 4. Deploy
- Push a GitHub
- Render redeploya automáticamente
- ¡Funciona inmediatamente!

## ¿Prefieres continuar con Oracle o cambiar a PostgreSQL? 