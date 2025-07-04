# Instrucciones para Configurar el Modal de Health Auto Export

## 📋 Resumen

Se ha creado un modal completo para cargar, previsualizar y enviar datos de Health Auto Export a tu API. El modal incluye:

- ✅ Carga de archivos JSON
- ✅ Editor de texto para JSON
- ✅ Previsualización de datos
- ✅ Validación de formato
- ✅ Envío a la API
- ✅ Manejo de errores
- ✅ Diseño responsive

## 🚀 Pasos para Activar el Modal

### 1. Instalar Angular Material

```bash
npm install @angular/material @angular/cdk @angular/animations
```

### 2. Configurar Angular Material en tu proyecto

#### a) Agregar al `app.module.ts`:

```typescript
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FormsModule } from '@angular/forms';

// En el array de imports
@NgModule({
  imports: [
    // ... otros imports
    BrowserAnimationsModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    FormsModule
  ],
  // ... resto de la configuración
})
```

#### b) Agregar el componente al módulo:

```typescript
import { HealthAutoExportModalComponent } from './components/health-auto-export-modal/health-auto-export-modal.component';

@NgModule({
  declarations: [
    // ... otros componentes
    HealthAutoExportModalComponent
  ],
  // ... resto de la configuración
})
```

### 3. Descomentar el código en `dashboard.component.ts`

Reemplaza las líneas comentadas:

```typescript
// ANTES (comentado):
// import { MatDialog } from '@angular/material/dialog';
// import { HealthAutoExportModalComponent } from '../health-auto-export-modal/health-auto-export-modal.component';

// DESPUÉS (descomentado):
import { MatDialog } from '@angular/material/dialog';
import { HealthAutoExportModalComponent } from '../health-auto-export-modal/health-auto-export-modal.component';
```

```typescript
// ANTES (comentado):
// private dialog: MatDialog

// DESPUÉS (descomentado):
private dialog: MatDialog
```

```typescript
// ANTES (comentado):
/*
const dialogRef = this.dialog.open(HealthAutoExportModalComponent, {
  width: '800px',
  maxWidth: '95vw',
  maxHeight: '90vh',
  disableClose: false,
  data: {}
});

dialogRef.afterClosed().subscribe(result => {
  if (result && result.success) {
    console.log('✅ Datos cargados exitosamente:', result.response);
    
    // Mostrar mensaje de éxito
    alert(`¡Datos cargados exitosamente!\n\nProcesados: ${result.response.processedRecords}\nOmitidos: ${result.response.skippedRecords}`);
    
    // Recargar datos del dashboard si es necesario
    this.recargarDatos();
  }
});
*/

// DESPUÉS (descomentado):
const dialogRef = this.dialog.open(HealthAutoExportModalComponent, {
  width: '800px',
  maxWidth: '95vw',
  maxHeight: '90vh',
  disableClose: false,
  data: {}
});

dialogRef.afterClosed().subscribe(result => {
  if (result && result.success) {
    console.log('✅ Datos cargados exitosamente:', result.response);
    
    // Mostrar mensaje de éxito
    alert(`¡Datos cargados exitosamente!\n\nProcesados: ${result.response.processedRecords}\nOmitidos: ${result.response.skippedRecords}`);
    
    // Recargar datos del dashboard si es necesario
    this.recargarDatos();
  }
});
```

### 4. Agregar un tema de Angular Material (opcional)

En `styles.css`:

```css
@import '~@angular/material/prebuilt-themes/indigo-pink.css';
```

O elige otro tema:
- `deeppurple-amber.css`
- `pink-bluegrey.css`
- `purple-green.css`

## 📁 Archivos Creados

### Componentes:
- `src/app/components/health-auto-export-modal/health-auto-export-modal.component.ts`
- `src/app/components/health-auto-export-modal/health-auto-export-modal.component.html`
- `src/app/components/health-auto-export-modal/health-auto-export-modal.component.scss`

### Servicios:
- `src/app/services/health-auto-export.service.ts`

### Archivos de prueba:
- `health-auto-export-test.json`

## 🎯 Cómo Usar el Modal

### 1. Desde el Dashboard

El botón "📱 Cargar Health Auto Export" ya está agregado al dashboard. Al hacer clic se abrirá el modal.

### 2. Desde Otros Componentes

```typescript
import { MatDialog } from '@angular/material/dialog';
import { HealthAutoExportModalComponent } from '../health-auto-export-modal/health-auto-export-modal.component';

constructor(private dialog: MatDialog) {}

abrirModal() {
  const dialogRef = this.dialog.open(HealthAutoExportModalComponent, {
    width: '800px',
    maxWidth: '95vw',
    maxHeight: '90vh',
    data: {} // Datos opcionales para el modal
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result && result.success) {
      console.log('Datos cargados:', result.response);
    }
  });
}
```

## 🔧 Funcionalidades del Modal

### ✅ Carga de Archivos
- Selecciona archivos `.json` desde tu computadora
- Validación automática del formato
- Preview del contenido

### ✅ Editor de Texto
- Pega JSON directamente
- Validación en tiempo real
- Sintaxis highlighting (monospace font)

### ✅ Previsualización
- Resumen de métricas y entrenamientos
- Conteo de puntos de datos
- Lista de tipos de datos detectados
- Vista previa de entrenamientos

### ✅ Validación
- Formato JSON válido
- Estructura de Health Auto Export
- Presencia de datos requeridos
- Mensajes de error descriptivos

### ✅ Envío a la API
- Conexión al endpoint `/api/HealthAutoExport/health-data`
- Manejo de respuestas y errores
- Indicador de carga
- Mensaje de éxito/error

## 📱 Ejemplo de Uso

1. **Clic en el botón**: "📱 Cargar Health Auto Export"
2. **Cargar datos**: Selecciona un archivo JSON o pega el contenido
3. **Previsualizar**: Revisa el resumen de datos detectados
4. **Cargar**: Haz clic en "Cargar Datos" para enviar a la API
5. **Confirmación**: El modal se cierra automáticamente tras el éxito

## 🎨 Diseño

- **Responsive**: Se adapta a móviles y tablets
- **Material Design**: Usa componentes de Angular Material
- **Accesible**: Navegación por teclado y screen readers
- **Tema oscuro**: Soporte automático según preferencias del sistema

## 🚀 API Endpoint

El modal envía datos al endpoint:
```
POST /api/HealthAutoExport/health-data
```

Con el formato estándar de Health Auto Export que ya implementaste en el backend.

## 🔍 Troubleshooting

### Error: Cannot find module '@angular/material/dialog'
```bash
npm install @angular/material @angular/cdk @angular/animations
```

### Error: No provider for MatDialog
Asegúrate de importar `MatDialogModule` en tu `app.module.ts`

### Error: Cannot find module './health-auto-export-modal.component'
Verifica que el componente esté declarado en `app.module.ts`

### El modal no se ve bien
Agrega un tema de Angular Material en `styles.css`

## 📊 Datos de Prueba

Usa el archivo `health-auto-export-test.json` para probar el modal con datos de ejemplo que incluyen:

- 14 tipos de métricas diferentes
- 2 entrenamientos (Running y Cycling)
- Datos de múltiples días
- Formatos específicos (presión arterial, frecuencia cardíaca, sueño)

¡El modal está listo para usar una vez que instales Angular Material! 🎉 