// Script para limpiar cache de HealthPredict PWA
// Ejecutar en la consola del navegador o usar como bookmarklet

console.log('🧹 Limpiando cache de HealthPredict...');

// Función para limpiar todo el cache
async function clearAllCache() {
  try {
    // 1. Limpiar cache del navegador
    if ('caches' in window) {
      const cacheNames = await caches.keys();
      console.log('Caches encontrados:', cacheNames);
      
      for (const cacheName of cacheNames) {
        await caches.delete(cacheName);
        console.log(`✅ Cache eliminado: ${cacheName}`);
      }
    }
    
    // 2. Desregistrar service worker
    if ('serviceWorker' in navigator) {
      const registrations = await navigator.serviceWorker.getRegistrations();
      
      for (const registration of registrations) {
        await registration.unregister();
        console.log('✅ Service Worker desregistrado');
      }
    }
    
    // 3. Limpiar localStorage y sessionStorage
    localStorage.clear();
    sessionStorage.clear();
    console.log('✅ Storage local limpiado');
    
    // 4. Limpiar IndexedDB si existe
    if ('indexedDB' in window) {
      try {
        const databases = await indexedDB.databases();
        for (const db of databases) {
          if (db.name) {
            indexedDB.deleteDatabase(db.name);
            console.log(`✅ IndexedDB eliminado: ${db.name}`);
          }
        }
      } catch (e) {
        console.log('⚠️ No se pudo limpiar IndexedDB:', e.message);
      }
    }
    
    console.log('🎉 Cache completamente limpiado!');
    console.log('🔄 Recargando página en 2 segundos...');
    
    setTimeout(() => {
      window.location.reload(true);
    }, 2000);
    
  } catch (error) {
    console.error('❌ Error al limpiar cache:', error);
  }
}

// Ejecutar la función
clearAllCache();

// También agregar función global para uso futuro
window.clearHealthPredictCache = clearAllCache; 