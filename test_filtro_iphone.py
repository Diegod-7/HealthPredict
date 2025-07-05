#!/usr/bin/env python3
"""
Script para probar que el filtro de iPhone funciona correctamente
"""
import requests
import json

# Datos de prueba que simulan el JSON que envía el script
test_data = {
    "data": {
        "metrics": [
            {
                "units": "count",
                "name": "step_count",
                "data": [
                    {
                        "source": "Mi Fitness",
                        "qty": 100,
                        "date": "2025-07-05 10:00:00 -0400"
                    },
                    {
                        "source": "iPhone|Mi Fitness", 
                        "qty": 200,
                        "date": "2025-07-05 10:01:00 -0400"
                    },
                    {
                        "source": "Apple Watch",
                        "qty": 50,
                        "date": "2025-07-05 10:02:00 -0400"
                    },
                    {
                        "source": "iPhone",
                        "qty": 300,
                        "date": "2025-07-05 10:03:00 -0400"
                    },
                    {
                        "source": "Samsung Health",
                        "qty": 75,
                        "date": "2025-07-05 10:04:00 -0400"
                    }
                ]
            }
        ]
    }
}

def test_filtro_iphone():
    """Probar que solo se guardan datos con 'iPhone' en el source"""
    api_url = "https://healthpredict-l1hu.onrender.com/api/HealthAutoExport/pasos"
    
    print("=== Test del Filtro de iPhone ===")
    print("Enviando datos de prueba...")
    print("Datos que deberían guardarse (contienen 'iPhone'):")
    print("- iPhone|Mi Fitness: 200 pasos")
    print("- iPhone: 300 pasos")
    print("\nDatos que NO deberían guardarse (no contienen 'iPhone'):")
    print("- Mi Fitness: 100 pasos")
    print("- Apple Watch: 50 pasos") 
    print("- Samsung Health: 75 pasos")
    
    try:
        headers = {
            'Content-Type': 'application/json',
            'User-Agent': 'TestFiltroiPhone/1.0'
        }
        
        response = requests.post(api_url, json=test_data, headers=headers, timeout=30)
        
        print(f"\nStatus Code: {response.status_code}")
        
        if response.status_code == 200:
            result = response.json()
            print("Respuesta exitosa:")
            print(json.dumps(result, indent=2))
            
            pasos_guardados = result.get('pasosGuardados', 0)
            print(f"\nPasos guardados: {pasos_guardados}")
            
            if pasos_guardados == 2:
                print("✅ ÉXITO: El filtro funciona correctamente. Solo se guardaron 2 registros (los que contienen 'iPhone')")
            else:
                print(f"❌ ERROR: Se esperaban 2 registros guardados, pero se guardaron {pasos_guardados}")
                
        else:
            print(f"❌ Error en la API: {response.status_code}")
            print(response.text)
            
    except Exception as e:
        print(f"❌ Error durante la prueba: {e}")

if __name__ == "__main__":
    test_filtro_iphone() 