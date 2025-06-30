# 🐘 Opción PostgreSQL (MÁS FÁCIL)

## ¿Por qué PostgreSQL?
- ✅ **Gratuito en Render** (sin configuración compleja)
- ✅ **Deploy automático** con tu aplicación
- ✅ **Más compatible** con servicios cloud modernos
- ✅ **Mismo Entity Framework** (cambio mínimo de código)

## Pasos para Cambiar a PostgreSQL:

### 1. Crear PostgreSQL en Render
1. **Render Dashboard** → **New** → **PostgreSQL**
2. **Name**: `healthpredict-db`
3. **Plan**: **Free**
4. **Crear** → Copiar **External Database URL**

### 2. Actualizar el Proyecto .NET

#### Cambiar paquete NuGet:
```bash
# Remover Oracle
dotnet remove package Oracle.EntityFrameworkCore

# Agregar PostgreSQL
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

#### Actualizar Program.cs:
```csharp
// Cambiar de Oracle a PostgreSQL
builder.Services.AddDbContext<HealthPredictContext>(options => 
    options.UseNpgsql(connectionString));
```

#### Actualizar appsettings.Production.json:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```

#### Actualizar variables de entorno en Render:
```
POSTGRESQL_CONNECTION_STRING=postgresql://user:password@host:port/database
```

### 3. Recrear Migraciones
```bash
# Eliminar migraciones existentes
rm -rf Migrations/

# Crear nueva migración para PostgreSQL
dotnet ef migrations add InitialCreate

# Aplicar migración
dotnet ef database update
```

## Ventajas de PostgreSQL:
- 🚀 **Deploy en 5 minutos**
- 💰 **Completamente gratis**
- 🔄 **Backup automático**
- 📊 **Excelente rendimiento**
- 🛠️ **Herramientas web incluidas** 