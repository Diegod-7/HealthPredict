import Foundation

class APIService: ObservableObject {
    private let baseURL = "https://tu-api-healthpredict.com/api" // Cambiar por tu URL real
    private let session = URLSession.shared
    
    @Published var isConnected = false
    
    // MARK: - Configuración de usuario
    private let usuarioId = 1 // En una app real, esto vendría del login
    
    init() {
        checkConnection()
    }
    
    // MARK: - Verificar conexión con la API
    func checkConnection() {
        Task {
            do {
                let url = URL(string: "\(baseURL)/DatosVitales/Usuario/\(usuarioId)")!
                let (_, response) = try await session.data(from: url)
                
                if let httpResponse = response as? HTTPURLResponse,
                   httpResponse.statusCode == 200 {
                    await MainActor.run {
                        self.isConnected = true
                    }
                }
            } catch {
                print("Error al verificar conexión: \(error)")
                await MainActor.run {
                    self.isConnected = false
                }
            }
        }
    }
    
    // MARK: - Sincronizar datos con HealthKit
    func syncHealthKitData(_ healthData: [HealthKitDataRequest]) async throws -> SyncResponse {
        guard !healthData.isEmpty else {
            throw APIError.noDataToSync
        }
        
        let url = URL(string: "\(baseURL)/DatosVitales/Sync/HealthKit")!
        var request = URLRequest(url: url)
        request.httpMethod = "POST"
        request.setValue("application/json", forHTTPHeaderField: "Content-Type")
        
        // Configurar encoder para fechas
        let encoder = JSONEncoder()
        encoder.dateEncodingStrategy = .iso8601
        
        do {
            let jsonData = try encoder.encode(healthData)
            request.httpBody = jsonData
            
            let (data, response) = try await session.data(for: request)
            
            guard let httpResponse = response as? HTTPURLResponse else {
                throw APIError.invalidResponse
            }
            
            if httpResponse.statusCode == 200 {
                let decoder = JSONDecoder()
                let syncResponse = try decoder.decode(SyncResponse.self, from: data)
                return syncResponse
            } else {
                // Intentar decodificar mensaje de error
                if let errorData = try? JSONSerialization.jsonObject(with: data) as? [String: Any],
                   let errorMessage = errorData["error"] as? String {
                    throw APIError.serverError(errorMessage)
                } else {
                    throw APIError.httpError(httpResponse.statusCode)
                }
            }
        } catch let error as APIError {
            throw error
        } catch {
            throw APIError.networkError(error.localizedDescription)
        }
    }
    
    // MARK: - Obtener última fecha de sincronización
    func getLastSyncDate() async throws -> Date? {
        let url = URL(string: "\(baseURL)/DatosVitales/LastSync/\(usuarioId)")!
        
        do {
            let (data, response) = try await session.data(from: url)
            
            guard let httpResponse = response as? HTTPURLResponse,
                  httpResponse.statusCode == 200 else {
                throw APIError.invalidResponse
            }
            
            let decoder = JSONDecoder()
            decoder.dateDecodingStrategy = .iso8601
            
            // La API devuelve la fecha o null
            if let dateString = try JSONSerialization.jsonObject(with: data) as? String {
                return ISO8601DateFormatter().date(from: dateString)
            }
            
            return nil
        } catch {
            throw APIError.networkError(error.localizedDescription)
        }
    }
    
    // MARK: - Obtener datos vitales del usuario
    func getUserHealthData() async throws -> [DatoVital] {
        let url = URL(string: "\(baseURL)/DatosVitales/Usuario/\(usuarioId)")!
        
        do {
            let (data, response) = try await session.data(from: url)
            
            guard let httpResponse = response as? HTTPURLResponse,
                  httpResponse.statusCode == 200 else {
                throw APIError.invalidResponse
            }
            
            let decoder = JSONDecoder()
            decoder.dateDecodingStrategy = .iso8601
            
            let datosVitales = try decoder.decode([DatoVital].self, from: data)
            return datosVitales
        } catch {
            throw APIError.networkError(error.localizedDescription)
        }
    }
    
    // MARK: - Obtener estadísticas
    func getStatistics(tipoDato: String, fechaInicio: Date, fechaFin: Date) async throws -> [String: Double] {
        var components = URLComponents(string: "\(baseURL)/DatosVitales/Estadisticas")!
        components.queryItems = [
            URLQueryItem(name: "usuarioId", value: String(usuarioId)),
            URLQueryItem(name: "tipoDato", value: tipoDato),
            URLQueryItem(name: "fechaInicio", value: ISO8601DateFormatter().string(from: fechaInicio)),
            URLQueryItem(name: "fechaFin", value: ISO8601DateFormatter().string(from: fechaFin))
        ]
        
        guard let url = components.url else {
            throw APIError.invalidURL
        }
        
        do {
            let (data, response) = try await session.data(from: url)
            
            guard let httpResponse = response as? HTTPURLResponse,
                  httpResponse.statusCode == 200 else {
                throw APIError.invalidResponse
            }
            
            let statistics = try JSONSerialization.jsonObject(with: data) as? [String: Double] ?? [:]
            return statistics
        } catch {
            throw APIError.networkError(error.localizedDescription)
        }
    }
}

// MARK: - Modelos de API
struct DatoVital: Codable, Identifiable {
    let id: Int
    let usuarioId: Int
    let fechaRegistro: Date
    let tipoDato: String
    let valor: Double
    let unidad: String
    let dispositivoOrigen: String?
    let notas: String?
}

// MARK: - Errores de API
enum APIError: LocalizedError {
    case invalidURL
    case invalidResponse
    case noDataToSync
    case networkError(String)
    case serverError(String)
    case httpError(Int)
    
    var errorDescription: String? {
        switch self {
        case .invalidURL:
            return "URL inválida"
        case .invalidResponse:
            return "Respuesta inválida del servidor"
        case .noDataToSync:
            return "No hay datos para sincronizar"
        case .networkError(let message):
            return "Error de red: \(message)"
        case .serverError(let message):
            return "Error del servidor: \(message)"
        case .httpError(let code):
            return "Error HTTP: \(code)"
        }
    }
} 