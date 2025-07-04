#!/usr/bin/env python3
"""
Script de prueba para verificar el endpoint de sincronización
"""

import requests
import json

def test_sync_endpoint():
    """Prueba el endpoint de sincronización"""
    url = "https://healthpredict-l1hu.onrender.com/api/HealthAutoExport/sync-google-drive"
    
    print(f"🔄 Probando endpoint: {url}")
    
    try:
        headers = {
            'Content-Type': 'application/json',
            'User-Agent': 'TestScript/1.0'
        }
        
        response = requests.post(url, json={}, headers=headers, timeout=120)
        
        print(f"📊 Status Code: {response.status_code}")
        print(f"📋 Response Headers: {dict(response.headers)}")
        
        if response.status_code == 200:
            print("✅ Endpoint responde correctamente")
            try:
                result = response.json()
                print(f"📄 Response JSON: {json.dumps(result, indent=2)}")
            except:
                print(f"📄 Response Text: {response.text}")
        else:
            print(f"❌ Error: {response.status_code}")
            print(f"📄 Response: {response.text}")
            
    except requests.exceptions.Timeout:
        print("⏰ Timeout - El endpoint tardó demasiado en responder")
    except requests.exceptions.ConnectionError:
        print("🔌 Error de conexión - No se pudo conectar al endpoint")
    except Exception as e:
        print(f"❌ Error: {e}")

def test_ultima_sincronizacion():
    """Prueba el endpoint de última sincronización"""
    url = "https://healthpredict-l1hu.onrender.com/api/HealthAutoExport/ultima-sincronizacion"
    
    print(f"\n🔄 Probando endpoint: {url}")
    
    try:
        response = requests.get(url, timeout=30)
        
        print(f"📊 Status Code: {response.status_code}")
        
        if response.status_code == 200:
            print("✅ Endpoint responde correctamente")
            try:
                result = response.json()
                print(f"📄 Response JSON: {json.dumps(result, indent=2)}")
            except:
                print(f"📄 Response Text: {response.text}")
        else:
            print(f"❌ Error: {response.status_code}")
            print(f"📄 Response: {response.text}")
            
    except Exception as e:
        print(f"❌ Error: {e}")

if __name__ == "__main__":
    print("🧪 Iniciando pruebas de endpoints...")
    
    # Probar endpoint de última sincronización (más rápido)
    test_ultima_sincronizacion()
    
    # Probar endpoint de sincronización (puede tardar más)
    test_sync_endpoint()
    
    print("\n✅ Pruebas completadas") 