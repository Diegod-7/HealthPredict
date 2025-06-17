import Foundation
import HealthKit

// MARK: - Modelos para API
struct HealthKitDataRequest: Codable {
    let usuarioId: Int
    let fechaRegistro: Date
    let tipoHealthKit: String
    let valor: Double
    let unidad: String
}

struct SyncResponse: Codable {
    let mensaje: String
    let cantidadProcesada: Int
    let alertasGeneradas: Int
}

// MARK: - Modelo para datos locales
struct HealthDataPoint: Identifiable {
    let id = UUID()
    let type: HKQuantityTypeIdentifier
    let value: Double
    let unit: String
    let date: Date
    let source: String
}

// MARK: - Configuración de tipos de datos de HealthKit
struct HealthKitConfiguration {
    static let readTypes: Set<HKObjectType> = [
        // Signos vitales
        HKQuantityType.quantityType(forIdentifier: .heartRate)!,
        HKQuantityType.quantityType(forIdentifier: .bloodPressureSystolic)!,
        HKQuantityType.quantityType(forIdentifier: .bloodPressureDiastolic)!,
        HKQuantityType.quantityType(forIdentifier: .bodyTemperature)!,
        HKQuantityType.quantityType(forIdentifier: .oxygenSaturation)!,
        HKQuantityType.quantityType(forIdentifier: .respiratoryRate)!,
        
        // Actividad física
        HKQuantityType.quantityType(forIdentifier: .stepCount)!,
        HKQuantityType.quantityType(forIdentifier: .distanceWalkingRunning)!,
        HKQuantityType.quantityType(forIdentifier: .activeEnergyBurned)!,
        HKQuantityType.quantityType(forIdentifier: .basalEnergyBurned)!,
        
        // Métricas corporales
        HKQuantityType.quantityType(forIdentifier: .bodyMass)!,
        HKQuantityType.quantityType(forIdentifier: .height)!,
        HKQuantityType.quantityType(forIdentifier: .bodyMassIndex)!,
        HKQuantityType.quantityType(forIdentifier: .bodyFatPercentage)!,
        
        // Sueño y recuperación
        HKCategoryType.categoryType(forIdentifier: .sleepAnalysis)!,
        
        // Nutrición
        HKQuantityType.quantityType(forIdentifier: .dietaryEnergyConsumed)!,
        HKQuantityType.quantityType(forIdentifier: .dietaryWater)!
    ]
    
    static func getDisplayName(for identifier: HKQuantityTypeIdentifier) -> String {
        switch identifier {
        case .heartRate:
            return "Frecuencia Cardíaca"
        case .bloodPressureSystolic:
            return "Presión Sistólica"
        case .bloodPressureDiastolic:
            return "Presión Diastólica"
        case .bodyTemperature:
            return "Temperatura Corporal"
        case .oxygenSaturation:
            return "Saturación de Oxígeno"
        case .respiratoryRate:
            return "Frecuencia Respiratoria"
        case .stepCount:
            return "Pasos"
        case .distanceWalkingRunning:
            return "Distancia Caminada"
        case .activeEnergyBurned:
            return "Calorías Activas"
        case .basalEnergyBurned:
            return "Calorías Basales"
        case .bodyMass:
            return "Peso"
        case .height:
            return "Altura"
        case .bodyMassIndex:
            return "Índice de Masa Corporal"
        case .bodyFatPercentage:
            return "Porcentaje de Grasa Corporal"
        case .dietaryEnergyConsumed:
            return "Calorías Consumidas"
        case .dietaryWater:
            return "Agua Consumida"
        default:
            return identifier.rawValue
        }
    }
}

// MARK: - Estado de sincronización
enum SyncStatus {
    case idle
    case syncing
    case success(Int)
    case error(String)
    
    var description: String {
        switch self {
        case .idle:
            return "Listo para sincronizar"
        case .syncing:
            return "Sincronizando..."
        case .success(let count):
            return "Sincronizados \(count) registros"
        case .error(let message):
            return "Error: \(message)"
        }
    }
} 