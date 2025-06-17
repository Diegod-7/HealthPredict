import SwiftUI

struct SyncView: View {
    @ObservedObject var healthKitManager: HealthKitManager
    @ObservedObject var apiService: APIService
    
    @State private var syncStatus: SyncStatus = .idle
    @State private var selectedDataTypes: Set<String> = []
    @State private var syncFromDate = Calendar.current.date(byAdding: .day, value: -7, to: Date()) ?? Date()
    @State private var showingDatePicker = false
    @State private var isLoading = false
    
    @Environment(\.dismiss) private var dismiss
    
    var body: some View {
        NavigationView {
            VStack(spacing: 20) {
                // Configuración de sincronización
                VStack(alignment: .leading, spacing: 16) {
                    Text("Configuración de Sincronización")
                        .font(.headline)
                    
                    // Selector de fecha
                    HStack {
                        Text("Sincronizar desde:")
                        Spacer()
                        Button(action: {
                            showingDatePicker = true
                        }) {
                            Text(syncFromDate, style: .date)
                                .padding(.horizontal, 12)
                                .padding(.vertical, 6)
                                .background(Color(.systemGray6))
                                .cornerRadius(8)
                        }
                    }
                    
                    // Tipos de datos disponibles
                    Text("Tipos de datos disponibles:")
                        .font(.subheadline)
                        .foregroundColor(.secondary)
                    
                    ScrollView {
                        LazyVGrid(columns: [
                            GridItem(.flexible()),
                            GridItem(.flexible())
                        ], spacing: 8) {
                            ForEach(getAvailableDataTypes(), id: \.self) { dataType in
                                DataTypeToggle(
                                    dataType: dataType,
                                    isSelected: selectedDataTypes.contains(dataType)
                                ) {
                                    toggleDataType(dataType)
                                }
                            }
                        }
                    }
                    .frame(maxHeight: 200)
                }
                .padding()
                .background(Color(.systemGray6))
                .cornerRadius(12)
                
                // Estado de sincronización
                VStack(spacing: 12) {
                    HStack {
                        Image(systemName: getStatusIcon())
                            .foregroundColor(getStatusColor())
                            .font(.title2)
                        
                        Text(syncStatus.description)
                            .font(.headline)
                        
                        Spacer()
                    }
                    
                    if case .syncing = syncStatus {
                        ProgressView()
                            .scaleEffect(1.2)
                    }
                }
                .padding()
                .background(Color(.systemGray6))
                .cornerRadius(12)
                
                // Datos recientes
                if !healthKitManager.healthData.isEmpty {
                    VStack(alignment: .leading, spacing: 8) {
                        Text("Datos Recientes")
                            .font(.headline)
                        
                        List(healthKitManager.healthData.prefix(10)) { dataPoint in
                            HealthDataRow(dataPoint: dataPoint)
                        }
                        .frame(height: 200)
                        .listStyle(PlainListStyle())
                    }
                }
                
                Spacer()
                
                // Botones de acción
                VStack(spacing: 12) {
                    Button(action: {
                        Task {
                            await loadHealthData()
                        }
                    }) {
                        HStack {
                            Image(systemName: "arrow.clockwise")
                            Text("Cargar Datos de Health")
                        }
                        .frame(maxWidth: .infinity)
                        .padding()
                        .background(Color.blue)
                        .foregroundColor(.white)
                        .cornerRadius(12)
                    }
                    .disabled(isLoading)
                    
                    Button(action: {
                        Task {
                            await performSync()
                        }
                    }) {
                        HStack {
                            Image(systemName: "icloud.and.arrow.up")
                            Text("Sincronizar con API")
                        }
                        .frame(maxWidth: .infinity)
                        .padding()
                        .background(canSync() ? Color.green : Color.gray)
                        .foregroundColor(.white)
                        .cornerRadius(12)
                    }
                    .disabled(!canSync() || isLoading)
                }
                .padding()
            }
            .navigationTitle("Sincronización")
            .navigationBarTitleDisplayMode(.inline)
            .navigationBarItems(
                leading: Button("Cerrar") {
                    dismiss()
                }
            )
            .onAppear {
                setupInitialState()
            }
            .sheet(isPresented: $showingDatePicker) {
                DatePicker(
                    "Sincronizar desde",
                    selection: $syncFromDate,
                    in: ...Date(),
                    displayedComponents: .date
                )
                .datePickerStyle(WheelDatePickerStyle())
                .padding()
                .presentationDetents([.medium])
            }
        }
    }
    
    // MARK: - Métodos privados
    private func setupInitialState() {
        // Seleccionar todos los tipos de datos por defecto
        selectedDataTypes = Set(getAvailableDataTypes())
        
        // Cargar datos si ya están autorizados
        if healthKitManager.isAuthorized {
            Task {
                await loadHealthData()
            }
        }
    }
    
    private func getAvailableDataTypes() -> [String] {
        return [
            "Frecuencia Cardíaca",
            "Presión Sistólica",
            "Presión Diastólica",
            "Temperatura Corporal",
            "Saturación de Oxígeno",
            "Pasos",
            "Distancia Caminada",
            "Calorías Activas",
            "Peso",
            "Altura"
        ]
    }
    
    private func toggleDataType(_ dataType: String) {
        if selectedDataTypes.contains(dataType) {
            selectedDataTypes.remove(dataType)
        } else {
            selectedDataTypes.insert(dataType)
        }
    }
    
    private func loadHealthData() async {
        isLoading = true
        syncStatus = .syncing
        
        let data = await healthKitManager.fetchHealthData(from: syncFromDate)
        
        await MainActor.run {
            isLoading = false
            if data.isEmpty {
                syncStatus = .error("No se encontraron datos en el rango seleccionado")
            } else {
                syncStatus = .success(data.count)
            }
        }
    }
    
    private func performSync() async {
        guard canSync() else { return }
        
        isLoading = true
        syncStatus = .syncing
        
        // Filtrar datos según tipos seleccionados
        let filteredData = healthKitManager.healthData.filter { dataPoint in
            let displayName = HealthKitConfiguration.getDisplayName(for: dataPoint.type)
            return selectedDataTypes.contains(displayName)
        }
        
        // Convertir a formato de API
        let apiData = healthKitManager.convertToAPIFormat(filteredData, usuarioId: 1)
        
        do {
            let response = try await apiService.syncHealthKitData(apiData)
            
            await MainActor.run {
                isLoading = false
                syncStatus = .success(response.cantidadProcesada)
                healthKitManager.lastSyncDate = Date()
            }
        } catch {
            await MainActor.run {
                isLoading = false
                syncStatus = .error(error.localizedDescription)
            }
        }
    }
    
    private func canSync() -> Bool {
        return healthKitManager.isAuthorized &&
               apiService.isConnected &&
               !healthKitManager.healthData.isEmpty &&
               !selectedDataTypes.isEmpty
    }
    
    private func getStatusIcon() -> String {
        switch syncStatus {
        case .idle:
            return "circle"
        case .syncing:
            return "arrow.triangle.2.circlepath"
        case .success:
            return "checkmark.circle.fill"
        case .error:
            return "exclamationmark.triangle.fill"
        }
    }
    
    private func getStatusColor() -> Color {
        switch syncStatus {
        case .idle:
            return .gray
        case .syncing:
            return .blue
        case .success:
            return .green
        case .error:
            return .red
        }
    }
}

struct DataTypeToggle: View {
    let dataType: String
    let isSelected: Bool
    let onToggle: () -> Void
    
    var body: some View {
        Button(action: onToggle) {
            HStack {
                Image(systemName: isSelected ? "checkmark.circle.fill" : "circle")
                    .foregroundColor(isSelected ? .green : .gray)
                
                Text(dataType)
                    .font(.caption)
                    .foregroundColor(.primary)
                    .lineLimit(2)
                
                Spacer()
            }
            .padding(8)
            .background(isSelected ? Color.green.opacity(0.1) : Color(.systemGray6))
            .cornerRadius(8)
        }
        .buttonStyle(PlainButtonStyle())
    }
}

struct HealthDataRow: View {
    let dataPoint: HealthDataPoint
    
    var body: some View {
        HStack {
            VStack(alignment: .leading, spacing: 2) {
                Text(HealthKitConfiguration.getDisplayName(for: dataPoint.type))
                    .font(.headline)
                    .lineLimit(1)
                
                Text("\(dataPoint.value, specifier: "%.1f") \(dataPoint.unit)")
                    .font(.subheadline)
                    .foregroundColor(.secondary)
                
                Text(dataPoint.date, style: .time)
                    .font(.caption)
                    .foregroundColor(.secondary)
            }
            
            Spacer()
            
            Text(dataPoint.source)
                .font(.caption)
                .foregroundColor(.secondary)
                .lineLimit(1)
        }
        .padding(.vertical, 4)
    }
}

#Preview {
    SyncView(
        healthKitManager: HealthKitManager(),
        apiService: APIService()
    )
} 