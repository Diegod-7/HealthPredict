# Iconos PWA para HealthPredict

## Generación de Iconos

Para convertir el archivo `icon.svg` a los tamaños PNG necesarios, puedes usar cualquiera de estos métodos:

### Método 1: Online (Recomendado)
1. Ve a [favicon.io](https://favicon.io/favicon-converter/)
2. Sube el archivo `icon.svg`
3. Descarga el paquete de iconos
4. Renombra los archivos según los tamaños requeridos

### Método 2: Comando ImageMagick (si tienes instalado)
```bash
# Instalar ImageMagick si no lo tienes
# Windows: choco install imagemagick
# macOS: brew install imagemagick
# Ubuntu: sudo apt install imagemagick

# Generar todos los tamaños desde el SVG
magick icon.svg -resize 72x72 icon-72x72.png
magick icon.svg -resize 96x96 icon-96x96.png
magick icon.svg -resize 128x128 icon-128x128.png
magick icon.svg -resize 144x144 icon-144x144.png
magick icon.svg -resize 152x152 icon-152x152.png
magick icon.svg -resize 167x167 icon-167x167.png
magick icon.svg -resize 180x180 icon-180x180.png
magick icon.svg -resize 192x192 icon-192x192.png
magick icon.svg -resize 384x384 icon-384x384.png
magick icon.svg -resize 512x512 icon-512x512.png
```

### Método 3: Usar un servicio online
1. Ve a [realfavicongenerator.net](https://realfavicongenerator.net/)
2. Sube el archivo `icon.svg`
3. Configura las opciones para PWA
4. Descarga los archivos generados

## Tamaños Requeridos
- 72x72 - Android Chrome pequeño
- 96x96 - Android Chrome
- 128x128 - Android Chrome
- 144x144 - Windows tiles
- 152x152 - iPad touch icon
- 167x167 - iPad Pro touch icon  
- 180x180 - iPhone touch icon
- 192x192 - Android Chrome (mínimo PWA)
- 384x384 - Android splash screen
- 512x512 - Android Chrome (recomendado PWA)

Una vez que tengas los archivos PNG, simplemente cópialos en esta carpeta manteniendo los nombres especificados en el manifest.json. 