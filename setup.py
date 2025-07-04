#!/usr/bin/env python3
"""
Script de configuración rápida para Google Drive Health Sync
"""

import subprocess
import sys
import os

def install_requirements():
    """Instala las dependencias necesarias"""
    print("🔧 Instalando dependencias...")
    try:
        subprocess.check_call([sys.executable, "-m", "pip", "install", "-r", "requirements.txt"])
        print("✅ Dependencias instaladas correctamente")
        return True
    except subprocess.CalledProcessError:
        print("❌ Error instalando dependencias")
        return False

def check_credentials():
    """Verifica si existe el archivo de credenciales"""
    if os.path.exists("credentials.json"):
        print("✅ Archivo credentials.json encontrado")
        return True
    else:
        print("❌ Archivo credentials.json no encontrado")
        print("📋 Instrucciones:")
        print("1. Ve a https://console.cloud.google.com/")
        print("2. Crea un proyecto y habilita la API de Google Drive")
        print("3. Crea credenciales OAuth 2.0")
        print("4. Descarga el archivo JSON y renómbralo a 'credentials.json'")
        print("5. Coloca el archivo en esta carpeta")
        return False

def main():
    """Función principal de configuración"""
    print("🚀 Configuración de Google Drive Health Sync")
    print("=" * 50)
    
    # Verificar Python
    print(f"🐍 Python version: {sys.version}")
    
    # Instalar dependencias
    if not install_requirements():
        return
    
    # Verificar credenciales
    if not check_credentials():
        print("\n⚠️  Configuración incompleta")
        print("Completa la configuración de credenciales y ejecuta el script nuevamente")
        return
    
    print("\n✅ Configuración completada")
    print("🚀 Puedes ejecutar el script con: python google-drive-sync.py")

if __name__ == "__main__":
    main() 