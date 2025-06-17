import Foundation
import HealthKit
import Combine

class HealthKitManager: ObservableObject {
    private let healthStore = HKHealthStore()
    @Published var isAuthorized = false
    @Published var healthData: [HealthDataPoint] = []
    @Published var lastSyncDate: Date?
    
    init() {
        checkHealthKitAvailability()
    }
    
    // MARK: - Verificar disponibilidad de HealthKit
    private func checkHealthKitAvailability() {
        guard HKHealthStore.isHealthDataAvailable() else {
            print("HealthKit no está disponible en este dispositivo")
            return
        }
    }
    
    // MARK: - Solicitar autorización
    func requestHealthKitAuthorization() async {
        do {
            try await healthStore.requestAuthorization(
                toShare: [],
                read: HealthKitConfiguration.readTypes
            )
            
            await MainActor.run {
                self.isAuthorized = true
            }
            
            print("Autorización de HealthKit concedida")
        } catch {
            print("Error al solicitar autorización de HealthKit: \(error)")
            await MainActor.run {
                self.isAuthorized = false
            }
        }
    }
    
    // MARK: - Obtener datos de HealthKit
    func fetchHealthData(from startDate: Date? = nil) async -> [HealthDataPoint] {
        guard isAuthorized else {
            print("HealthKit no está autorizado")
            return []
        }
        
        let calendar = Calendar.current
        let endDate = Date()
        let fromDate = startDate ?? calendar.date(byAdding: .day, value: -30, to: endDate) ?? endDate
        
        var allData: [HealthDataPoint] = []
        
        // Obtener datos para cada tipo de cantidad
        for readType in HealthKitConfiguration.readTypes {
            if let quantityType = readType as? HKQuantityType {
                let data = await fetchQuantityData(for: quantityType, from: fromDate, to: endDate)
                allData.append(contentsOf: data)
            }
        }
        
        // Ordenar por fecha
        allData.sort { $0.date > $1.date }
        
        await MainActor.run {
            self.healthData = allData
        }
        
        return allData
    }
    
    // MARK: - Obtener datos de cantidad específicos
    private func fetchQuantityData(for quantityType: HKQuantityType, from startDate: Date, to endDate: Date) async -> [HealthDataPoint] {
        return await withCheckedContinuation { continuation in
            let predicate = HKQuery.predicateForSamples(withStart: startDate, end: endDate, options: .strictStartDate)
            let sortDescriptor = NSSortDescriptor(key: HKSampleSortIdentifierStartDate, ascending: false)
            
            let query = HKSampleQuery(
                sampleType: quantityType,
                predicate: predicate,
                limit: HKObjectQueryNoLimit,
                sortDescriptors: [sortDescriptor]
            ) { query, samples, error in
                
                guard let samples = samples as? [HKQuantitySample], error == nil else {
                    print("Error al obtener datos para \(quantityType.identifier): \(error?.localizedDescription ?? "Error desconocido")")
                    continuation.resume(returning: [])
                    return
                }
                
                let dataPoints = samples.compactMap { sample -> HealthDataPoint? in
                    guard let identifier = HKQuantityTypeIdentifier(rawValue: quantityType.identifier) else {
                        return nil
                    }
                    
                    let unit = self.getPreferredUnit(for: identifier)
                    let value = sample.quantity.doubleValue(for: unit)
                    
                    return HealthDataPoint(
                        type: identifier,
                        value: value,
                        unit: unit.unitString,
                        date: sample.startDate,
                        source: sample.sourceRevision.source.name
                    )
                }
                
                continuation.resume(returning: dataPoints)
            }
            
            healthStore.execute(query)
        }
    }
    
    // MARK: - Obtener unidad preferida para cada tipo de dato
    private func getPreferredUnit(for identifier: HKQuantityTypeIdentifier) -> HKUnit {
        switch identifier {
        case .heartRate:
            return HKUnit.count().unitDivided(by: .minute())
        case .bloodPressureSystolic, .bloodPressureDiastolic:
            return HKUnit.millimeterOfMercury()
        case .bodyTemperature:
            return HKUnit.degreeCelsius()
        case .oxygenSaturation:
            return HKUnit.percent()
        case .respiratoryRate:
            return HKUnit.count().unitDivided(by: .minute())
        case .stepCount:
            return HKUnit.count()
        case .distanceWalkingRunning:
            return HKUnit.meter()
        case .activeEnergyBurned, .basalEnergyBurned, .dietaryEnergyConsumed:
            return HKUnit.kilocalorie()
        case .bodyMass:
            return HKUnit.gramUnit(with: .kilo)
        case .height:
            return HKUnit.meter()
        case .bodyMassIndex:
            return HKUnit.count()
        case .bodyFatPercentage:
            return HKUnit.percent()
        case .dietaryWater:
            return HKUnit.liter()
        default:
            return HKUnit.count()
        }
    }
    
    // MARK: - Convertir datos para API
    func convertToAPIFormat(_ healthData: [HealthDataPoint], usuarioId: Int) -> [HealthKitDataRequest] {
        return healthData.map { dataPoint in
            HealthKitDataRequest(
                usuarioId: usuarioId,
                fechaRegistro: dataPoint.date,
                tipoHealthKit: dataPoint.type.rawValue,
                valor: dataPoint.value,
                unidad: dataPoint.unit
            )
        }
    }
    
    // MARK: - Configurar sincronización automática
    func enableBackgroundDelivery() {
        for readType in HealthKitConfiguration.readTypes {
            if let quantityType = readType as? HKQuantityType {
                healthStore.enableBackgroundDelivery(for: quantityType, frequency: .daily) { success, error in
                    if let error = error {
                        print("Error al habilitar entrega en segundo plano para \(quantityType.identifier): \(error)")
                    } else {
                        print("Entrega en segundo plano habilitada para \(quantityType.identifier)")
                    }
                }
            }
        }
    }
    
    // MARK: - Obtener estadísticas rápidas
    func getQuickStats() async -> [String: String] {
        guard isAuthorized else { return [:] }
        
        var stats: [String: String] = [:]
        
        // Obtener datos recientes
        let recentData = await fetchHealthData(from: Calendar.current.date(byAdding: .day, value: -7, to: Date()))
        
        // Contar registros por tipo
        let groupedData = Dictionary(grouping: recentData) { $0.type }
        
        for (type, data) in groupedData {
            let displayName = HealthKitConfiguration.getDisplayName(for: type)
            stats[displayName] = "\(data.count) registros"
        }
        
        return stats
    }
} 