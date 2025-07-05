#!/usr/bin/env python3
"""
Script simple para sincronizar HealthAutoExport-2025-07-04.json desde Google Drive
Diseñado para ser llamado desde la aplicación Angular
"""

import os
import json
import requests
import sys
from datetime import datetime
from google.auth.transport.requests import Request
from google.oauth2.credentials import Credentials
from google_auth_oauthlib.flow import InstalledAppFlow
from googleapiclient.discovery import build
from googleapiclient.errors import HttpError
import io
from googleapiclient.http import MediaIoBaseDownload

# Configuración fija
SCOPES = ['https://www.googleapis.com/auth/drive.readonly']
CREDENTIALS_FILE = 'credentials.json'
TOKEN_FILE = 'token.json'
API_URL = 'https://healthpredict-l1hu.onrender.com/api/HealthAutoExport/pasos'
ARCHIVO_FIJO = "HealthAutoExport-2025-07-05.json"

class SyncPasosSimple:
    def __init__(self):
        self.service = None
        self.setup_drive_service()
    
    def setup_drive_service(self):
        """Configura el servicio de Google Drive"""
        creds = None
        
        if os.path.exists(TOKEN_FILE):
            creds = Credentials.from_authorized_user_file(TOKEN_FILE, SCOPES)
        
        if not creds or not creds.valid:
            if creds and creds.expired and creds.refresh_token:
                creds.refresh(Request())
            else:
                if not os.path.exists(CREDENTIALS_FILE):
                    print(json.dumps({
                        "success": False,
                        "error": "credentials_not_found",
                        "message": "Archivo credentials.json no encontrado"
                    }))
                    return
                
                flow = InstalledAppFlow.from_client_secrets_file(CREDENTIALS_FILE, SCOPES)
                creds = flow.run_local_server(port=0)
            
            with open(TOKEN_FILE, 'w') as token:
                token.write(creds.to_json())
        
        self.service = build('drive', 'v3', credentials=creds)
    
    def find_file_by_name(self, filename):
        """Busca un archivo por nombre en toda la unidad de Google Drive"""
        try:
            # Buscar el archivo por nombre en toda la unidad
            query = f"name='{filename}' and trashed=false"
            results = self.service.files().list(
                q=query,
                fields="files(id, name, modifiedTime, size, parents)"
            ).execute()
            
            files = results.get('files', [])
            if not files:
                return None
            
            # Si hay múltiples archivos con el mismo nombre, tomar el más reciente
            if len(files) > 1:
                files.sort(key=lambda x: x.get('modifiedTime', ''), reverse=True)
            
            return files[0]
            
        except HttpError as e:
            print(f"Error al buscar archivo: {e}")
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
            
            file_content.seek(0)
            content = file_content.read().decode('utf-8')
            return content
            
        except HttpError as e:
            print(f"Error al descargar archivo: {e}")
            return None
    
    def send_to_api(self, data):
        """Envía los datos de pasos a la API de HealthPredict"""
        try:
            headers = {
                'Content-Type': 'application/json',
                'User-Agent': 'GoogleDriveHealthSync/1.0'
            }
            
            response = requests.post(API_URL, json=data, headers=headers, timeout=60)
            
            if response.status_code == 200:
                result = response.json()
                return {
                    "success": True,
                    "message": "Datos sincronizados exitosamente",
                    "response": result
                }
            else:
                return {
                    "success": False,
                    "error": f"api_error_{response.status_code}",
                    "message": f"Error en la API: {response.status_code}",
                    "response": response.text
                }
                
        except requests.exceptions.Timeout:
            return {
                "success": False,
                "error": "timeout",
                "message": "La API tardó demasiado en responder"
            }
        except requests.exceptions.ConnectionError:
            return {
                "success": False,
                "error": "connection_error",
                "message": "No se pudo conectar a la API"
            }
        except Exception as e:
            return {
                "success": False,
                "error": "unknown_error",
                "message": str(e)
            }
    
    def sync(self):
        """Sincroniza el archivo de pasos específico"""
        try:
            if not self.service:
                return {
                    "success": False,
                    "error": "drive_service_error",
                    "message": "No se pudo configurar el servicio de Google Drive"
                }
            
            # Buscar archivo por nombre en toda la unidad
            file_info = self.find_file_by_name(ARCHIVO_FIJO)
            if not file_info:
                return {
                    "success": False,
                    "error": "file_not_found",
                    "message": f"No se encontró el archivo {ARCHIVO_FIJO}"
                }
            
            # Descargar contenido
            content = self.download_file_content(file_info['id'])
            if not content:
                return {
                    "success": False,
                    "error": "download_error",
                    "message": "No se pudo descargar el archivo"
                }
            
            # Procesar datos
            try:
                data = json.loads(content)
            except json.JSONDecodeError:
                return {
                    "success": False,
                    "error": "json_error",
                    "message": "El archivo no contiene JSON válido"
                }
            
            # Enviar a la API
            result = self.send_to_api(data)
            
            if result["success"]:
                return {
                    "success": True,
                    "message": f"Archivo {ARCHIVO_FIJO} sincronizado exitosamente",
                    "file_info": {
                        "name": file_info['name'],
                        "id": file_info['id'],
                        "modified": file_info.get('modifiedTime', 'N/A'),
                        "size": file_info.get('size', 'N/A')
                    },
                    "api_response": result["response"]
                }
            else:
                return result
                
        except Exception as e:
            return {
                "success": False,
                "error": "sync_error",
                "message": f"Error durante la sincronización: {str(e)}"
            }

def main():
    """Función principal"""
    try:
        syncer = SyncPasosSimple()
        result = syncer.sync()
        print(json.dumps(result, indent=2, ensure_ascii=False))
        
        # Código de salida basado en el resultado
        sys.exit(0 if result["success"] else 1)
        
    except KeyboardInterrupt:
        print(json.dumps({
            "success": False,
            "error": "interrupted",
            "message": "Operación cancelada por el usuario"
        }))
        sys.exit(1)
    except Exception as e:
        print(json.dumps({
            "success": False,
            "error": "unexpected_error",
            "message": f"Error inesperado: {str(e)}"
        }))
        sys.exit(1)

if __name__ == "__main__":
    main() 