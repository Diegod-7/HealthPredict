#!/usr/bin/env python3
"""
Script para automatizar la carga de datos de pasos desde Google Drive a HealthPredict API
Requiere: pip install google-api-python-client google-auth-httplib2 google-auth-oauthlib requests
"""

import os
import json
import requests
import time
from datetime import datetime, timedelta
from google.auth.transport.requests import Request
from google.oauth2.credentials import Credentials
from google_auth_oauthlib.flow import InstalledAppFlow
from googleapiclient.discovery import build
from googleapiclient.errors import HttpError
import io
from googleapiclient.http import MediaIoBaseDownload

# Configuración
SCOPES = ['https://www.googleapis.com/auth/drive.readonly']
CREDENTIALS_FILE = 'credentials.json'  # Archivo de credenciales de Google
TOKEN_FILE = 'token.json'  # Archivo de token guardado

# Configuración de la API
API_URL = 'https://healthpredict-l1hu.onrender.com/api/HealthAutoExport'
API_ENDPOINTS = {
    'simple': f'{API_URL}/simple',
    'batch': f'{API_URL}/batch',
    'health_data': f'{API_URL}/health-data',
    'json': f'{API_URL}/json',
    'pasos': f'{API_URL}/pasos'
}

class GoogleDriveHealthSync:
    def __init__(self):
        self.service = None
        self.setup_drive_service()
    
    def setup_drive_service(self):
        """Configura el servicio de Google Drive"""
        creds = None
        
        # El archivo token.json almacena los tokens de acceso y actualización del usuario
        if os.path.exists(TOKEN_FILE):
            creds = Credentials.from_authorized_user_file(TOKEN_FILE, SCOPES)
        
        # Si no hay credenciales válidas disponibles, permite al usuario autenticarse
        if not creds or not creds.valid:
            if creds and creds.expired and creds.refresh_token:
                creds.refresh(Request())
            else:
                if not os.path.exists(CREDENTIALS_FILE):
                    print("❌ Error: No se encontró el archivo credentials.json")
                    print("📋 Instrucciones:")
                    print("1. Ve a https://console.cloud.google.com/")
                    print("2. Crea un proyecto o selecciona uno existente")
                    print("3. Habilita la API de Google Drive")
                    print("4. Crea credenciales OAuth 2.0")
                    print("5. Descarga el archivo JSON y renómbralo a 'credentials.json'")
                    return
                
                flow = InstalledAppFlow.from_client_secrets_file(CREDENTIALS_FILE, SCOPES)
                creds = flow.run_local_server(port=0)
            
            # Guardar las credenciales para la próxima ejecución
            with open(TOKEN_FILE, 'w') as token:
                token.write(creds.to_json())
        
        self.service = build('drive', 'v3', credentials=creds)
        print("✅ Servicio de Google Drive configurado correctamente")
    
    def find_file_by_name(self, filename, folder_path=None):
        """Busca un archivo por nombre en Google Drive, opcionalmente en una carpeta específica"""
        try:
            # Si se especifica una ruta de carpeta, buscar el archivo en esa carpeta
            if folder_path:
                return self.find_file_in_folder(filename, folder_path)
            
            # Buscar en toda la unidad
            results = self.service.files().list(
                q=f"name='{filename}' and trashed=false",
                fields="files(id, name, modifiedTime, size, parents)"
            ).execute()
            
            files = results.get('files', [])
            if not files:
                print(f"❌ No se encontró el archivo: {filename}")
                return None
            
            # Tomar el primer archivo encontrado
            file_info = files[0]
            print(f"✅ Archivo encontrado: {file_info['name']}")
            print(f"📅 Última modificación: {file_info['modifiedTime']}")
            print(f"📏 Tamaño: {file_info.get('size', 'N/A')} bytes")
            
            return file_info
            
        except HttpError as error:
            print(f"❌ Error buscando archivo: {error}")
            return None
    
    def find_file_in_folder(self, filename, folder_path):
        """Busca un archivo en una ruta específica de carpetas"""
        try:
            print(f"🔍 Buscando archivo '{filename}' en ruta: {folder_path}")
            
            # Dividir la ruta en carpetas
            folders = [f.strip() for f in folder_path.split('/') if f.strip()]
            
            # Empezar desde la raíz
            current_folder_id = 'root'
            
            # Navegar por cada carpeta en la ruta
            for folder_name in folders:
                print(f"📁 Navegando a carpeta: {folder_name}")
                
                # Buscar la carpeta
                folder_query = f"name='{folder_name}' and parents in '{current_folder_id}' and mimeType='application/vnd.google-apps.folder' and trashed=false"
                folder_results = self.service.files().list(
                    q=folder_query,
                    fields="files(id, name)"
                ).execute()
                
                folders_found = folder_results.get('files', [])
                if not folders_found:
                    print(f"❌ No se encontró la carpeta: {folder_name}")
                    return None
                
                current_folder_id = folders_found[0]['id']
                print(f"✅ Carpeta encontrada: {folder_name}")
            
            # Buscar el archivo en la carpeta final
            file_query = f"name='{filename}' and parents in '{current_folder_id}' and trashed=false"
            file_results = self.service.files().list(
                q=file_query,
                fields="files(id, name, modifiedTime, size, parents)"
            ).execute()
            
            files = file_results.get('files', [])
            if not files:
                print(f"❌ No se encontró el archivo '{filename}' en la carpeta especificada")
                return None
            
            file_info = files[0]
            print(f"✅ Archivo encontrado: {file_info['name']}")
            print(f"📅 Última modificación: {file_info['modifiedTime']}")
            print(f"📏 Tamaño: {file_info.get('size', 'N/A')} bytes")
            
            return file_info
            
        except HttpError as error:
            print(f"❌ Error buscando archivo en carpeta: {error}")
            return None
    
    def find_latest_healthautoexport_file(self, folder_path="Mi unidad/HealthAutoExport/Health"):
        """Busca el archivo HealthAutoExport más reciente en la carpeta especificada"""
        try:
            print(f"🔍 Buscando archivos HealthAutoExport en: {folder_path}")
            
            # Dividir la ruta en carpetas
            folders = [f.strip() for f in folder_path.split('/') if f.strip()]
            
            # Empezar desde la raíz
            current_folder_id = 'root'
            
            # Navegar por cada carpeta en la ruta
            for folder_name in folders:
                print(f"📁 Navegando a carpeta: {folder_name}")
                
                # Buscar la carpeta
                folder_query = f"name='{folder_name}' and parents in '{current_folder_id}' and mimeType='application/vnd.google-apps.folder' and trashed=false"
                folder_results = self.service.files().list(
                    q=folder_query,
                    fields="files(id, name)"
                ).execute()
                
                folders_found = folder_results.get('files', [])
                if not folders_found:
                    print(f"❌ No se encontró la carpeta: {folder_name}")
                    return None
                
                current_folder_id = folders_found[0]['id']
                print(f"✅ Carpeta encontrada: {folder_name}")
            
            # Buscar archivos HealthAutoExport en la carpeta final
            file_query = f"name contains 'HealthAutoExport-' and name contains '.json' and parents in '{current_folder_id}' and trashed=false"
            file_results = self.service.files().list(
                q=file_query,
                fields="files(id, name, modifiedTime, size)",
                orderBy="modifiedTime desc"  # Ordenar por fecha de modificación descendente
            ).execute()
            
            files = file_results.get('files', [])
            if not files:
                print(f"❌ No se encontraron archivos HealthAutoExport en la carpeta especificada")
                return None
            
            # Mostrar todos los archivos encontrados
            print(f"📋 Archivos HealthAutoExport encontrados:")
            for i, file in enumerate(files):
                print(f"   {i+1}. {file['name']} - {file['modifiedTime']}")
            
            # Tomar el más reciente (primero en la lista)
            file_info = files[0]
            print(f"✅ Archivo más reciente seleccionado: {file_info['name']}")
            print(f"📅 Última modificación: {file_info['modifiedTime']}")
            print(f"📏 Tamaño: {file_info.get('size', 'N/A')} bytes")
            
            return file_info
            
        except HttpError as error:
            print(f"❌ Error buscando archivos HealthAutoExport: {error}")
            return None
    
    def download_file_content(self, file_id):
        """Descarga el contenido de un archivo desde Google Drive"""
        try:
            request = self.service.files().get_media(fileId=file_id)
            file_content = io.BytesIO()
            downloader = MediaIoBaseDownload(file_content, request)
            
            done = False
            while done is False:
                status, done = downloader.next_chunk()
                print(f"📥 Descargando... {int(status.progress() * 100)}%")
            
            file_content.seek(0)
            content = file_content.read().decode('utf-8')
            
            print("✅ Archivo descargado correctamente")
            return content
            
        except HttpError as error:
            print(f"❌ Error descargando archivo: {error}")
            return None
    
    def send_to_api(self, data, endpoint='pasos'):
        """Envía los datos de pasos a la API de HealthPredict"""
        try:
            # Siempre usar el endpoint de pasos
            url = API_ENDPOINTS['pasos']
            
            headers = {
                'Content-Type': 'application/json',
                'User-Agent': 'GoogleDriveHealthSync/1.0'
            }
            
            print(f"📤 Enviando datos de pasos a: {url}")
            
            response = requests.post(url, json=data, headers=headers, timeout=60)
            
            if response.status_code == 200:
                result = response.json()
                print("✅ Datos de pasos enviados exitosamente")
                print(f"📊 Respuesta: {json.dumps(result, indent=2)}")
                return True
            elif response.status_code == 503:
                print("❌ Error 503: Servicio no disponible")
                print("🔄 La API puede estar iniciándose o temporalmente no disponible")
                print("💡 Sugerencia: Espera unos minutos y reintenta")
                print(f"📄 Respuesta: {response.text}")
                return False
            else:
                print(f"❌ Error en la API: {response.status_code}")
                print(f"📄 Respuesta: {response.text}")
                return False
                
        except requests.exceptions.Timeout:
            print("❌ Error: Timeout - La API tardó demasiado en responder")
            print("💡 Sugerencia: Verifica que la API esté funcionando")
            return False
        except requests.exceptions.ConnectionError:
            print("❌ Error: No se pudo conectar a la API")
            print("💡 Sugerencia: Verifica la URL y tu conexión a internet")
            return False
        except requests.exceptions.RequestException as e:
            print(f"❌ Error de conexión: {e}")
            return False
    
    def process_health_data(self, json_content):
        """Procesa los datos de pasos para enviar al endpoint /pasos"""
        try:
            data = json.loads(json_content)
            
            print("📊 Formato: Datos de pasos")
            print("🎯 Endpoint: /pasos")
            
            # Siempre enviar al endpoint de pasos
            # Si es una lista, mantenerla como está
            if isinstance(data, list):
                return data, 'pasos'
            # Si es un objeto, extraer los datos o enviarlo tal como está
            else:
                return data, 'pasos'
            
        except json.JSONDecodeError as e:
            print(f"❌ Error parseando JSON: {e}")
            return None, None
    
    def sync_file(self, filename, folder_path=None, auto_detect=True):
        """Sincroniza un archivo específico desde Google Drive"""
        print(f"🔄 Iniciando sincronización de: {filename}")
        
        # Buscar archivo
        if folder_path:
            file_info = self.find_file_by_name(filename, folder_path)
        else:
            file_info = self.find_file_by_name(filename)
            
        if not file_info:
            return False
        
        # Descargar contenido
        content = self.download_file_content(file_info['id'])
        if not content:
            return False
        
        # Procesar datos
        data, endpoint = self.process_health_data(content)
        if not data:
            return False
        
        # Enviar a la API
        success = self.send_to_api(data, endpoint)
        
        if success:
            print(f"✅ Sincronización completada: {filename}")
            return True
        else:
            print(f"❌ Error en sincronización: {filename}")
            return False
    
    def sync_latest_healthautoexport(self, folder_path="Mi unidad/HealthAutoExport/Health"):
        """Sincroniza el archivo HealthAutoExport más reciente"""
        print(f"🔄 Sincronizando archivo HealthAutoExport más reciente...")
        
        # Buscar el archivo más reciente
        file_info = self.find_latest_healthautoexport_file(folder_path)
        if not file_info:
            return False
        
        # Descargar contenido
        content = self.download_file_content(file_info['id'])
        if not content:
            return False
        
        # Procesar datos
        data, endpoint = self.process_health_data(content)
        if not data:
            return False
        
        # Enviar a la API
        success = self.send_to_api(data, endpoint)
        
        if success:
            print(f"✅ Sincronización completada: {file_info['name']}")
            return True
        else:
            print(f"❌ Error en sincronización: {file_info['name']}")
            return False
    
    def monitor_file(self, filename, folder_path=None, interval_minutes=5):
        """Monitorea un archivo y sincroniza cuando cambia"""
        print(f"👁️ Monitoreando archivo: {filename}")
        if folder_path:
            print(f"📁 En carpeta: {folder_path}")
        print(f"⏰ Intervalo: {interval_minutes} minutos")
        
        last_modified = None
        
        while True:
            try:
                if folder_path:
                    file_info = self.find_file_by_name(filename, folder_path)
                else:
                    file_info = self.find_file_by_name(filename)
                    
                if file_info:
                    current_modified = file_info['modifiedTime']
                    
                    if last_modified is None:
                        last_modified = current_modified
                        print(f"📅 Archivo inicial detectado: {current_modified}")
                    elif current_modified != last_modified:
                        print(f"🔄 Archivo modificado detectado: {current_modified}")
                        if self.sync_file(filename, folder_path):
                            last_modified = current_modified
                        else:
                            print("❌ Error en sincronización, reintentando en el próximo ciclo")
                    else:
                        print(f"✅ Sin cambios detectados: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
                
                print(f"⏳ Esperando {interval_minutes} minutos...")
                time.sleep(interval_minutes * 60)
                
            except KeyboardInterrupt:
                print("\n🛑 Monitoreo detenido por el usuario")
                break
            except Exception as e:
                print(f"❌ Error en monitoreo: {e}")
                print(f"⏳ Reintentando en {interval_minutes} minutos...")
                time.sleep(interval_minutes * 60)
    
    def monitor_latest_healthautoexport(self, folder_path="Mi unidad/HealthAutoExport/Health", interval_minutes=5):
        """Monitorea y sincroniza el archivo HealthAutoExport más reciente"""
        print(f"👁️ Monitoreando archivos HealthAutoExport más recientes")
        print(f"📁 En carpeta: {folder_path}")
        print(f"⏰ Intervalo: {interval_minutes} minutos")
        
        last_file_name = None
        last_modified = None
        
        while True:
            try:
                file_info = self.find_latest_healthautoexport_file(folder_path)
                
                if file_info:
                    current_file_name = file_info['name']
                    current_modified = file_info['modifiedTime']
                    
                    if last_file_name is None:
                        # Primera ejecución
                        last_file_name = current_file_name
                        last_modified = current_modified
                        print(f"📅 Archivo inicial detectado: {current_file_name}")
                    elif current_file_name != last_file_name:
                        # Archivo nuevo detectado
                        print(f"🆕 Nuevo archivo detectado: {current_file_name}")
                        if self.sync_latest_healthautoexport(folder_path):
                            last_file_name = current_file_name
                            last_modified = current_modified
                        else:
                            print("❌ Error en sincronización, reintentando en el próximo ciclo")
                    elif current_modified != last_modified:
                        # Archivo existente modificado
                        print(f"🔄 Archivo modificado detectado: {current_file_name}")
                        if self.sync_latest_healthautoexport(folder_path):
                            last_modified = current_modified
                        else:
                            print("❌ Error en sincronización, reintentando en el próximo ciclo")
                    else:
                        print(f"✅ Sin cambios detectados: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
                
                print(f"⏳ Esperando {interval_minutes} minutos...")
                time.sleep(interval_minutes * 60)
                
            except KeyboardInterrupt:
                print("\n🛑 Monitoreo detenido por el usuario")
                break
            except Exception as e:
                print(f"❌ Error en monitoreo: {e}")
                print(f"⏳ Reintentando en {interval_minutes} minutos...")
                time.sleep(interval_minutes * 60)

def main():
    """Función principal"""
    print("🚀 Google Drive Health Sync - Iniciando...")
    print("=" * 50)
    
    # Crear instancia del sincronizador
    sync = GoogleDriveHealthSync()
    
    if not sync.service:
        print("❌ No se pudo configurar el servicio de Google Drive")
        return
    
    # Configuración fija
    ARCHIVO_FIJO = "HealthAutoExport-2025-07-04.json"
    CARPETA_FIJA = "Mi unidad/HealthAutoExport/Health"
    
    print(f"\n📁 Archivo configurado: {ARCHIVO_FIJO}")
    print(f"📂 Carpeta configurada: {CARPETA_FIJA}")
    
    # Opciones de uso simplificadas
    print("\n📋 Opciones disponibles:")
    print("1. Sincronizar una vez")
    print("2. Monitorear continuamente")
    print("3. Salir")
    
    while True:
        try:
            choice = input("\n👉 Selecciona una opción (1-3): ").strip()
            
            if choice == '1':
                print(f"🔄 Sincronizando {ARCHIVO_FIJO}...")
                sync.sync_file(ARCHIVO_FIJO, CARPETA_FIJA)
            
            elif choice == '2':
                try:
                    interval = int(input("⏰ Intervalo en minutos (default: 5): ") or "5")
                    print(f"👁️ Monitoreando {ARCHIVO_FIJO} cada {interval} minutos...")
                    sync.monitor_file(ARCHIVO_FIJO, CARPETA_FIJA, interval)
                except ValueError:
                    print("❌ Intervalo no válido, usando 5 minutos")
                    sync.monitor_file(ARCHIVO_FIJO, CARPETA_FIJA, 5)
            
            elif choice == '3':
                print("👋 ¡Hasta luego!")
                break
            
            else:
                print("❌ Opción no válida")
                
        except KeyboardInterrupt:
            print("\n👋 ¡Hasta luego!")
            break
        except Exception as e:
            print(f"❌ Error: {e}")

if __name__ == "__main__":
    main() 