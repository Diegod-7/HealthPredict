// Service Worker para HealthPredict PWA - Versión mejorada
const CACHE_NAME = 'healthpredict-v2.0'; // Incrementar versión para forzar limpieza
const urlsToCache = [
  '/',
  '/index.html',
  '/manifest.json',
  '/assets/icons/icon-192x192.png',
  '/assets/icons/icon-512x512.png'
];

// Detectar si estamos en desarrollo
const isDevelopment = location.hostname === 'localhost' || location.hostname === '127.0.0.1';

// Instalación del Service Worker
self.addEventListener('install', function(event) {
  console.log('Service Worker instalándose... v2.0');
  
  // En desarrollo, skipWaiting inmediatamente
  if (isDevelopment) {
    self.skipWaiting();
  }
  
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then(function(cache) {
        console.log('Archivos agregados al cache');
        return cache.addAll(urlsToCache);
      })
      .catch(function(error) {
        console.error('Error al agregar archivos al cache:', error);
      })
  );
});

// Activación del Service Worker
self.addEventListener('activate', function(event) {
  console.log('Service Worker activándose... v2.0');
  
  event.waitUntil(
    Promise.all([
      // Limpiar caches antiguos
      caches.keys().then(function(cacheNames) {
        return Promise.all(
          cacheNames.map(function(cacheName) {
            if (cacheName !== CACHE_NAME) {
              console.log('Eliminando cache antiguo:', cacheName);
              return caches.delete(cacheName);
            }
          })
        );
      }),
      // En desarrollo, tomar control inmediatamente
      isDevelopment ? self.clients.claim() : Promise.resolve()
    ])
  );
});

// Interceptar requests con estrategia mejorada
self.addEventListener('fetch', function(event) {
  const request = event.request;
  const url = new URL(request.url);
  
  // No cachear en desarrollo para evitar problemas
  if (isDevelopment) {
    event.respondWith(
      fetch(request).catch(function() {
        // Solo usar cache como fallback en desarrollo
        if (request.destination === 'document') {
          return caches.match('/index.html');
        }
      })
    );
    return;
  }
  
  // Estrategia para archivos HTML: Network First
  if (request.destination === 'document') {
    event.respondWith(
      fetch(request)
        .then(function(response) {
          // Si la respuesta es exitosa, actualizar cache
          if (response.status === 200) {
            const responseClone = response.clone();
            caches.open(CACHE_NAME).then(function(cache) {
              cache.put(request, responseClone);
            });
          }
          return response;
        })
        .catch(function() {
          // Fallback al cache
          return caches.match(request).then(function(response) {
            return response || caches.match('/index.html');
          });
        })
    );
    return;
  }
  
  // Estrategia para archivos estáticos: Cache First
  if (request.destination === 'image' || request.destination === 'script' || request.destination === 'style') {
    event.respondWith(
      caches.match(request).then(function(response) {
        if (response) {
          return response;
        }
        
        return fetch(request).then(function(response) {
          if (response.status === 200) {
            const responseClone = response.clone();
            caches.open(CACHE_NAME).then(function(cache) {
              cache.put(request, responseClone);
            });
          }
          return response;
        });
      })
    );
    return;
  }
  
  // Para todo lo demás: Network First
  event.respondWith(
    fetch(request).catch(function() {
      return caches.match(request);
    })
  );
});

// Manejar mensajes del cliente para forzar actualizaciones
self.addEventListener('message', function(event) {
  if (event.data && event.data.type === 'SKIP_WAITING') {
    console.log('Forzando actualización del Service Worker');
    self.skipWaiting();
  }
  
  if (event.data && event.data.type === 'CLEAR_CACHE') {
    console.log('Limpiando cache por solicitud del cliente');
    event.waitUntil(
      caches.keys().then(function(cacheNames) {
        return Promise.all(
          cacheNames.map(function(cacheName) {
            console.log('Eliminando cache:', cacheName);
            return caches.delete(cacheName);
          })
        );
      })
    );
  }
}); 