#!/usr/bin/env python3
"""
Script de diagnóstico para verificar la configuración de OAuth con Google Drive
"""

import os
import json
import sys

def verificar_archivo_credenciales():
    """Verifica si el archivo credentials.json existe y es válido"""
    print("🔍 Verificando archivo credentials.json...")
    
    if not os.path.exists("credentials.json"):
        print("❌ El archivo credentials.json no existe")
        print("📋 Instrucciones:")
        print("1. Ve a https://console.cloud.google.com/")
        print("2. Ve a APIs y servicios → Credenciales")
        print("3. Descarga tu archivo OAuth 2.0 JSON")
        print("4. Renómbralo a 'credentials.json'")
        return False
    
    try:
        with open("credentials.json", "r") as f:
            creds_data = json.load(f)
        
        # Verificar estructura
        if "installed" not in creds_data:
            print("❌ El archivo credentials.json no tiene la estructura correcta")
            print("⚠️ Asegúrate de descargar las credenciales de 'Aplicación de escritorio'")
            return False
        
        client_info = creds_data["installed"]
        
        print("✅ Archivo credentials.json encontrado y válido")
        print(f"📋 Client ID: {client_info.get('client_id', 'N/A')}")
        print(f"📋 Project ID: {client_info.get('project_id', 'N/A')}")
        
        # Verificar redirect URIs
        redirect_uris = client_info.get('redirect_uris', [])
        print(f"📋 Redirect URIs: {redirect_uris}")
        
        if 'http://localhost:8080/' not in redirect_uris:
            print("⚠️ Recomendación: Agrega http://localhost:8080/ como URI de redirección")
        
        return True
        
    except json.JSONDecodeError:
        print("❌ El archivo credentials.json no es un JSON válido")
        return False
    except Exception as e:
        print(f"❌ Error leyendo credentials.json: {e}")
        return False

def verificar_token_existente():
    """Verifica si existe un token previo"""
    print("\n🔍 Verificando token existente...")
    
    if os.path.exists("token.json"):
        print("⚠️ Existe un archivo token.json previo")
        print("💡 Recomendación: Elimínalo para forzar nueva autenticación")
        print("   Comando: rm token.json (Linux/Mac) o del token.json (Windows)")
        return True
    else:
        print("✅ No hay token previo - se creará uno nuevo")
        return False

def verificar_dependencias():
    """Verifica si las dependencias están instaladas"""
    print("\n🔍 Verificando dependencias de Python...")
    
    dependencias = [
        "google-api-python-client",
        "google-auth-httplib2", 
        "google-auth-oauthlib",
        "requests"
    ]
    
    faltantes = []
    
    for dep in dependencias:
        try:
            __import__(dep.replace('-', '_'))
            print(f"✅ {dep}")
        except ImportError:
            print(f"❌ {dep}")
            faltantes.append(dep)
    
    if faltantes:
        print(f"\n📦 Instalar dependencias faltantes:")
        print(f"pip install {' '.join(faltantes)}")
        return False
    
    return True

def mostrar_configuracion_recomendada():
    """Muestra la configuración recomendada para Google Cloud Console"""
    print("\n📋 Configuración Recomendada en Google Cloud Console:")
    print("=" * 60)
    
    print("\n🔧 Pantalla de Consentimiento OAuth:")
    print("   • Tipo de usuario: Externo")
    print("   • Nombre de la aplicación: HealthPredict Drive Sync")
    print("   • Correo de asistencia: [tu-email]")
    print("   • Ámbitos: https://www.googleapis.com/auth/drive.readonly")
    print("   • Usuarios de prueba: [tu-email] ⚠️ MUY IMPORTANTE")
    
    print("\n🔧 Credenciales OAuth 2.0:")
    print("   • Tipo: Aplicación de escritorio")
    print("   • Nombre: HealthPredict Desktop Client")
    print("   • URIs de redirección:")
    print("     - http://localhost:8080/")
    print("     - http://localhost:8090/")
    print("     - http://localhost:55992/")

def mostrar_pasos_solucion_403():
    """Muestra los pasos específicos para solucionar el error 403"""
    print("\n🚨 Solución Error 403: access_denied")
    print("=" * 60)
    
    print("\n1. 🔍 Verificar Usuarios de Prueba (CRÍTICO):")
    print("   • Ve a Google Cloud Console")
    print("   • APIs y servicios → Pantalla de consentimiento OAuth")
    print("   • Busca 'Usuarios de prueba'")
    print("   • Haz clic en 'AGREGAR USUARIOS'")
    print("   • Agrega tu email de Google")
    print("   • GUARDA los cambios")
    
    print("\n2. 🔄 Limpiar Autenticación:")
    print("   • Elimina token.json si existe")
    print("   • Ejecuta el script nuevamente")
    
    print("\n3. 🌐 Proceso de Autenticación:")
    print("   • Se abrirá tu navegador")
    print("   • Selecciona tu cuenta de Google")
    print("   • Si aparece 'This app isn't verified':")
    print("     - Haz clic en 'Advanced'")
    print("     - Haz clic en 'Go to HealthPredict Drive Sync (unsafe)'")
    print("   • Acepta los permisos")

def main():
    """Función principal de diagnóstico"""
    print("🔍 Diagnóstico de Configuración OAuth - Google Drive")
    print("=" * 60)
    
    # Verificar archivo de credenciales
    creds_ok = verificar_archivo_credenciales()
    
    # Verificar token existente
    token_exists = verificar_token_existente()
    
    # Verificar dependencias
    deps_ok = verificar_dependencias()
    
    # Mostrar configuración recomendada
    mostrar_configuracion_recomendada()
    
    # Mostrar solución para error 403
    mostrar_pasos_solucion_403()
    
    print("\n📊 Resumen del Diagnóstico:")
    print("=" * 60)
    print(f"✅ Archivo credentials.json: {'OK' if creds_ok else 'ERROR'}")
    print(f"✅ Dependencias Python: {'OK' if deps_ok else 'ERROR'}")
    print(f"⚠️ Token existente: {'SÍ' if token_exists else 'NO'}")
    
    if not creds_ok or not deps_ok:
        print("\n❌ Hay problemas que deben solucionarse antes de continuar")
        return False
    
    if token_exists:
        print("\n💡 Recomendación: Elimina token.json para forzar nueva autenticación")
    
    print("\n🚀 Próximos pasos:")
    print("1. Verifica la configuración en Google Cloud Console")
    print("2. Asegúrate de agregar tu email como usuario de prueba")
    print("3. Ejecuta: python google-drive-sync.py")
    
    return True

if __name__ == "__main__":
    main()