import SwiftUI

struct ContentView: View {
    @StateObject private var healthKitManager = HealthKitManager()
    @StateObject private var apiService = APIService()
    @State private var showingSyncView = false
    @State private var quickStats: [String: String] = [:]
    
    var body: some View {
        NavigationView {
            VStack(spacing: 20) {
                // Header
                VStack {
                    Image(systemName: "heart.fill")
                        .font(.system(size: 60))
                        .foregroundColor(.red)
                    
                    Text("HealthPredict Sync")
                        .font(.largeTitle)
                        .fontWeight(.bold)
                    
                    Text("Sincroniza tus datos de Apple Health")
                        .font(.subheadline)
                        .foregroundColor(.secondary)
                        .multilineTextAlignment(.center)
                }
                .padding()
                
                // Estado de conexiones
                VStack(spacing: 12) {
                    ConnectionStatusRow(
                        title: "Apple Health",
                        isConnected: healthKitManager.isAuthorized,
                        icon: "heart.circle.fill"
                    )
                    
                    ConnectionStatusRow(
                        title: "HealthPredict API",
                        isConnected: apiService.isConnected,
                        icon: "cloud.fill"
                    )
                }
                .padding()
                .background(Color(.systemGray6))
                .cornerRadius(12)
                
                // Estadísticas rápidas
                if !quickStats.isEmpty {
                    VStack(alignment: .leading, spacing: 8) {
                        Text("Datos Recientes (7 días)")
                            .font(.headline)
                            .padding(.horizontal)
                        
                        LazyVGrid(columns: [
                            GridItem(.flexible()),
                            GridItem(.flexible())
                        ], spacing: 8) {
                            ForEach(Array(quickStats.keys.sorted()), id: \.self) { key in
                                StatCard(title: key, value: quickStats[key] ?? "")
                            }
                        }
                        .padding(.horizontal)
                    }
                }
                
                Spacer()
                
                // Botones de acción
                VStack(spacing: 12) {
                    if !healthKitManager.isAuthorized {
                        Button(action: {
                            Task {
                                await healthKitManager.requestHealthKitAuthorization()
                                if healthKitManager.isAuthorized {
                                    await loadQuickStats()
                                }
                            }
                        }) {
                            HStack {
                                Image(systemName: "heart.circle")
                                Text("Autorizar Apple Health")
                            }
                            .frame(maxWidth: .infinity)
                            .padding()
                            .background(Color.blue)
                            .foregroundColor(.white)
                            .cornerRadius(12)
                        }
                    }
                    
                    Button(action: {
                        showingSyncView = true
                    }) {
                        HStack {
                            Image(systemName: "arrow.triangle.2.circlepath")
                            Text("Sincronizar Datos")
                        }
                        .frame(maxWidth: .infinity)
                        .padding()
                        .background(healthKitManager.isAuthorized ? Color.green : Color.gray)
                        .foregroundColor(.white)
                        .cornerRadius(12)
                    }
                    .disabled(!healthKitManager.isAuthorized)
                    
                    Button(action: {
                        apiService.checkConnection()
                    }) {
                        HStack {
                            Image(systemName: "network")
                            Text("Verificar Conexión")
                        }
                        .frame(maxWidth: .infinity)
                        .padding()
                        .background(Color.orange)
                        .foregroundColor(.white)
                        .cornerRadius(12)
                    }
                }
                .padding()
            }
            .navigationTitle("HealthPredict")
            .navigationBarTitleDisplayMode(.inline)
            .onAppear {
                if healthKitManager.isAuthorized {
                    Task {
                        await loadQuickStats()
                    }
                }
            }
            .sheet(isPresented: $showingSyncView) {
                SyncView(healthKitManager: healthKitManager, apiService: apiService)
            }
        }
    }
    
    private func loadQuickStats() async {
        let stats = await healthKitManager.getQuickStats()
        await MainActor.run {
            self.quickStats = stats
        }
    }
}

struct ConnectionStatusRow: View {
    let title: String
    let isConnected: Bool
    let icon: String
    
    var body: some View {
        HStack {
            Image(systemName: icon)
                .foregroundColor(isConnected ? .green : .red)
                .font(.title2)
            
            Text(title)
                .font(.headline)
            
            Spacer()
            
            Text(isConnected ? "Conectado" : "Desconectado")
                .font(.caption)
                .padding(.horizontal, 8)
                .padding(.vertical, 4)
                .background(isConnected ? Color.green.opacity(0.2) : Color.red.opacity(0.2))
                .foregroundColor(isConnected ? .green : .red)
                .cornerRadius(8)
        }
    }
}

struct StatCard: View {
    let title: String
    let value: String
    
    var body: some View {
        VStack(alignment: .leading, spacing: 4) {
            Text(title)
                .font(.caption)
                .foregroundColor(.secondary)
                .lineLimit(2)
            
            Text(value)
                .font(.headline)
                .fontWeight(.semibold)
        }
        .frame(maxWidth: .infinity, alignment: .leading)
        .padding(12)
        .background(Color(.systemBackground))
        .cornerRadius(8)
        .shadow(radius: 1)
    }
}

#Preview {
    ContentView()
} 