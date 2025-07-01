# Configuración PWA para HealthPredict

## ✅ Configuraciones Implementadas

### 1. Metaetiquetas para iOS (index.html)
- `apple-mobile-web-app-capable="yes"` - Permite modo app standalone
- `apple-mobile-web-app-status-bar-style="default"` - Estilo de barra de estado
- `apple-mobile-web-app-title="HealthPredict"` - Título de la app
- `mobile-web-app-capable="yes"` - PWA para Android
- `theme-color="#007bff"` - Color de tema
- Referencias a iconos Apple Touch

### 2. Manifest PWA (manifest.json)
- Configuración completa de PWA
- `display: "standalone"` - **CLAVE para eliminar la barra de URL**
- Iconos para todos los tamaños requeridos
- Configuración de colores y orientación

### 3. Service Worker (sw.js)
- Cache básico para funcionamiento offline
- Registro automático en index.html
- Estrategia cache-first para recursos estáticos

### 4. Configuración Angular (angular.json)
- Incluye manifest.json y sw.js en assets
- Se copian automáticamente al build

## 🚀 Cómo Probar

### En iOS:
1. Abre la app en Safari en tu iPhone/iPad
2. Toca el botón "Compartir" (icono de compartir)
3. Selecciona "Agregar a pantalla de inicio"
4. Confirma el nombre y toca "Agregar"
5. **La app ahora debería abrir SIN la barra de URL de Safari**

### En Android:
1. Abre la app en Chrome
2. Aparecerá un banner "Agregar a pantalla de inicio"
3. O ve al menú ⋮ > "Agregar a pantalla de inicio"
4. La app abrirá en modo standalone

## 📱 Características PWA Activas

- ✅ **Modo Standalone** - Sin barra de URL
- ✅ **Icono en pantalla de inicio**
- ✅ **Splash screen** (generado automáticamente)
- ✅ **Cache offline** básico
- ✅ **Instalable** como app nativa
- ✅ **Responsive** y optimizado para móviles

## 🔧 Pasos Adicionales Recomendados

### 1. Generar Iconos Reales
Los archivos en `src/assets/icons/` son temporales. Para iconos reales:
```bash
# Ve a src/assets/icons/ y sigue las instrucciones en README.md
# Recomendado: usar https://favicon.io/ con el SVG creado
```

### 2. Optimizar Service Worker
- Agregar más recursos al cache
- Implementar estrategias avanzadas de cache
- Agregar notificaciones push (opcional)

### 3. Verificar Funcionalidad
1. Construir la aplicación: `ng build --prod`
2. Servir desde un servidor HTTPS (requisito PWA)
3. Probar en dispositivos reales

## 🔍 Verificación PWA

Puedes verificar que todo esté funcionando:
1. Abre Chrome DevTools
2. Ve a "Application" > "Manifest"
3. Ve a "Application" > "Service Workers"
4. Ejecuta "Lighthouse" audit para PWA

## ⚠️ Notas Importantes

- **HTTPS es requerido** para PWA en producción
- Los iconos temporales deben ser reemplazados por PNG reales
- El service worker puede necesitar actualizaciones según tus necesidades
- Prueba siempre en dispositivos reales, no solo en simuladores

## 🎯 Solución Específica para tu Problema

La clave para eliminar la barra de URL en iOS es:
1. ✅ `apple-mobile-web-app-capable="yes"`
2. ✅ `display: "standalone"` en manifest.json
3. ✅ Agregar la app desde Safari usando "Agregar a pantalla de inicio"

Una vez agregada correctamente, la app debería abrir en pantalla completa sin la barra de URL de Safari. 