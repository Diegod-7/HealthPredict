import { Component, OnInit, OnDestroy } from '@angular/core';
import { HealthSensorService, HealthData } from '../../services/health-sensor.service';
import { DatoVitalService } from '../../services/dato-vital.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-health-sensor',
  template: `
    <div class="health-sensor-container">
      <div class="sensor-header">
        <h3>🏃‍♂️ Monitoreo en Tiempo Real</h3>
        <div class="status-indicator" [class.active]="isMonitoring">
          {{ isMonitoring ? '🟢 ACTIVO' : '🔴 INACTIVO' }}
        </div>
      </div>

      <div class="sensor-actions">
        <button 
          class="btn-primary" 
          (click)="toggleMonitoring()"
          [disabled]="isConnecting">
          {{ isMonitoring ? '⏹️ Detener' : '▶️ Iniciar' }}
          {{ isConnecting ? 'Conectando...' : '' }}
        </button>
        
        <button 
          class="btn-secondary" 
          (click)="connectBluetooth()"
          [disabled]="isConnecting">
          📱 Conectar Reloj
        </button>
        
        <button 
          class="btn-sync" 
          (click)="syncToBackend()"
          [disabled]="!currentData || isSyncing">
          {{ isSyncing ? '🔄 Sincronizando...' : '☁️ Sincronizar' }}
        </button>
      </div>

      <div class="health-metrics" *ngIf="currentData">
        <div class="metric-card heart-rate">
          <div class="metric-icon">❤️</div>
          <div class="metric-content">
            <div class="metric-value">{{ currentData.heartRate || '--' }}</div>
            <div class="metric-unit">BPM</div>
            <div class="metric-label">Frecuencia Cardíaca</div>
          </div>
          <div class="metric-pulse" *ngIf="currentData.heartRate"></div>
        </div>

        <div class="metric-card steps">
          <div class="metric-icon">👟</div>
          <div class="metric-content">
            <div class="metric-value">{{ currentData.steps || 0 | number }}</div>
            <div class="metric-unit">PASOS</div>
            <div class="metric-label">Hoy</div>
          </div>
        </div>

        <div class="metric-card activity">
          <div class="metric-icon">🔥</div>
          <div class="metric-content">
            <div class="metric-value">{{ estimatedCalories | number }}</div>
            <div class="metric-unit">KCAL</div>
            <div class="metric-label">Estimadas</div>
          </div>
        </div>
      </div>

      <div class="data-source" *ngIf="currentData">
        <small>
          📡 Fuente: {{ getSourceLabel(currentData.source) }} | 
          🕐 {{ currentData.timestamp | date:'HH:mm:ss' }}
        </small>
      </div>

      <div class="daily-stats" *ngIf="todayStats">
        <h4>📊 Resumen del Día</h4>
        <div class="stats-grid">
          <div class="stat-item">
            <span class="stat-label">Total Pasos</span>
            <span class="stat-value">{{ todayStats.totalSteps | number }}</span>
          </div>
          <div class="stat-item">
            <span class="stat-label">FC Promedio</span>
            <span class="stat-value">{{ todayStats.avgHeartRate }} BPM</span>
          </div>
          <div class="stat-item">
            <span class="stat-label">Minutos Activos</span>
            <span class="stat-value">{{ todayStats.activeMinutes }}</span>
          </div>
          <div class="stat-item">
            <span class="stat-label">Calorías</span>
            <span class="stat-value">{{ todayStats.caloriesBurned }}</span>
          </div>
        </div>
      </div>

      <div class="sync-history" *ngIf="syncHistory.length > 0">
        <h4>📤 Historial de Sincronización</h4>
        <div class="sync-item" *ngFor="let sync of syncHistory.slice(0, 3)">
          <span class="sync-time">{{ sync.timestamp | date:'HH:mm' }}</span>
          <span class="sync-status" [class]="sync.status">
            {{ sync.status === 'success' ? '✅' : '❌' }}
            {{ sync.message }}
          </span>
        </div>
      </div>

      <div class="permissions-info" *ngIf="!isMonitoring">
        <div class="info-box">
          <h5>🔐 Permisos Necesarios</h5>
          <ul>
            <li>📱 Acceso a sensores del dispositivo</li>
            <li>📍 Ubicación (para actividad física)</li>
            <li>🔵 Bluetooth (para relojes inteligentes)</li>
          </ul>
          <p>Al hacer clic en "Iniciar", se solicitarán automáticamente.</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .health-sensor-container {
      padding: 20px;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      border-radius: 15px;
      color: white;
      margin: 20px 0;
    }

    .sensor-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
    }

    .sensor-header h3 {
      margin: 0;
      font-size: 1.5rem;
    }

    .status-indicator {
      padding: 8px 15px;
      border-radius: 20px;
      background: rgba(255,255,255,0.2);
      font-weight: bold;
      font-size: 0.9rem;
    }

    .status-indicator.active {
      background: #4CAF50;
      animation: pulse 2s infinite;
    }

    @keyframes pulse {
      0% { opacity: 1; }
      50% { opacity: 0.7; }
      100% { opacity: 1; }
    }

    .sensor-actions {
      display: flex;
      gap: 10px;
      margin-bottom: 25px;
      flex-wrap: wrap;
    }

    .btn-primary, .btn-secondary, .btn-sync {
      padding: 12px 20px;
      border: none;
      border-radius: 8px;
      font-weight: bold;
      cursor: pointer;
      transition: all 0.3s;
    }

    .btn-primary {
      background: #4CAF50;
      color: white;
    }

    .btn-secondary {
      background: #2196F3;
      color: white;
    }

    .btn-sync {
      background: #FF9800;
      color: white;
    }

    .btn-primary:hover, .btn-secondary:hover, .btn-sync:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 15px rgba(0,0,0,0.3);
    }

    .btn-primary:disabled, .btn-secondary:disabled, .btn-sync:disabled {
      opacity: 0.6;
      cursor: not-allowed;
      transform: none;
    }

    .health-metrics {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 15px;
      margin-bottom: 20px;
    }

    .metric-card {
      background: rgba(255,255,255,0.15);
      backdrop-filter: blur(10px);
      padding: 20px;
      border-radius: 12px;
      display: flex;
      align-items: center;
      position: relative;
      overflow: hidden;
    }

    .metric-icon {
      font-size: 2.5rem;
      margin-right: 15px;
    }

    .metric-content {
      flex: 1;
    }

    .metric-value {
      font-size: 2rem;
      font-weight: bold;
      line-height: 1;
    }

    .metric-unit {
      font-size: 0.8rem;
      opacity: 0.8;
      margin-top: 2px;
    }

    .metric-label {
      font-size: 0.9rem;
      opacity: 0.9;
      margin-top: 5px;
    }

    .metric-pulse {
      position: absolute;
      right: 15px;
      width: 15px;
      height: 15px;
      background: #4CAF50;
      border-radius: 50%;
      animation: heartbeat 1.5s infinite;
    }

    @keyframes heartbeat {
      0% { transform: scale(1); opacity: 1; }
      50% { transform: scale(1.2); opacity: 0.7; }
      100% { transform: scale(1); opacity: 1; }
    }

    .data-source {
      text-align: center;
      opacity: 0.8;
      margin-bottom: 20px;
      font-size: 0.85rem;
    }

    .daily-stats {
      background: rgba(255,255,255,0.1);
      padding: 20px;
      border-radius: 10px;
      margin-bottom: 20px;
    }

    .daily-stats h4 {
      margin: 0 0 15px 0;
      font-size: 1.2rem;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 15px;
    }

    .stat-item {
      text-align: center;
      padding: 10px;
      background: rgba(255,255,255,0.1);
      border-radius: 8px;
    }

    .stat-label {
      display: block;
      font-size: 0.85rem;
      opacity: 0.8;
      margin-bottom: 5px;
    }

    .stat-value {
      display: block;
      font-size: 1.3rem;
      font-weight: bold;
    }

    .sync-history {
      background: rgba(255,255,255,0.1);
      padding: 15px;
      border-radius: 8px;
      margin-bottom: 20px;
    }

    .sync-history h4 {
      margin: 0 0 12px 0;
      font-size: 1.1rem;
    }

    .sync-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 8px 0;
      border-bottom: 1px solid rgba(255,255,255,0.1);
    }

    .sync-item:last-child {
      border-bottom: none;
    }

    .sync-time {
      font-size: 0.9rem;
      opacity: 0.8;
    }

    .sync-status {
      font-size: 0.85rem;
    }

    .sync-status.success {
      color: #4CAF50;
    }

    .sync-status.error {
      color: #F44336;
    }

    .permissions-info {
      background: rgba(255,255,255,0.1);
      padding: 20px;
      border-radius: 10px;
    }

    .info-box h5 {
      margin: 0 0 15px 0;
      color: #FFD700;
    }

    .info-box ul {
      margin: 15px 0;
      padding-left: 20px;
    }

    .info-box li {
      margin-bottom: 8px;
      font-size: 0.9rem;
    }

    .info-box p {
      margin: 15px 0 0 0;
      font-size: 0.9rem;
      opacity: 0.9;
    }

    @media (max-width: 768px) {
      .health-sensor-container {
        padding: 15px;
        margin: 15px 0;
      }
      
      .sensor-header {
        flex-direction: column;
        gap: 10px;
        text-align: center;
      }
      
      .sensor-actions {
        justify-content: center;
      }
      
      .health-metrics {
        grid-template-columns: 1fr;
      }
      
      .stats-grid {
        grid-template-columns: repeat(2, 1fr);
      }
    }
  `]
})
export class HealthSensorComponent implements OnInit, OnDestroy {
  currentData: HealthData | null = null;
  todayStats: any = null;
  isMonitoring = false;
  isConnecting = false;
  isSyncing = false;
  
  syncHistory: Array<{timestamp: Date, status: string, message: string}> = [];
  
  private subscription: Subscription = new Subscription();

  constructor(
    private healthSensorService: HealthSensorService,
    private datoVitalService: DatoVitalService
  ) {}

  ngOnInit() {
    // Suscribirse a datos en tiempo real
    this.subscription.add(
      this.healthSensorService.healthData$.subscribe(data => {
        this.currentData = data;
      })
    );

    // Cargar estadísticas del día
    this.loadTodayStats();
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
    this.healthSensorService.stopMonitoring();
  }

  async toggleMonitoring() {
    if (this.isMonitoring) {
      this.healthSensorService.stopMonitoring();
      this.isMonitoring = false;
    } else {
      this.isConnecting = true;
      const success = await this.healthSensorService.startMonitoring();
      this.isMonitoring = success;
      this.isConnecting = false;
      
      if (success) {
        this.addSyncRecord('success', 'Monitoreo iniciado exitosamente');
      } else {
        this.addSyncRecord('error', 'Error al iniciar monitoreo');
      }
    }
  }

  async connectBluetooth() {
    this.isConnecting = true;
    try {
      const success = await this.healthSensorService.connectBluetoothDevice();
      if (success) {
        this.addSyncRecord('success', 'Dispositivo Bluetooth conectado');
      } else {
        this.addSyncRecord('error', 'Error conectando dispositivo');
      }
    } catch (error) {
      this.addSyncRecord('error', 'Bluetooth no disponible');
    } finally {
      this.isConnecting = false;
    }
  }

  async syncToBackend() {
    if (!this.currentData) return;
    
    this.isSyncing = true;
    try {
      // Crear datos vitales para sincronizar
      const datosVitales = [];
      
      if (this.currentData.heartRate) {
        datosVitales.push({
          usuarioId: 1, // En una app real vendría del login
          fechaRegistro: this.currentData.timestamp,
          tipoHealthKit: 'heart_rate',
          valor: this.currentData.heartRate,
          unidad: 'bpm'
        });
      }
      
      if (this.currentData.steps) {
        datosVitales.push({
          usuarioId: 1,
          fechaRegistro: this.currentData.timestamp,
          tipoHealthKit: 'step_count',
          valor: this.currentData.steps,
          unidad: 'count'
        });
      }

      // Sincronizar con el backend usando el endpoint existente
      await this.datoVitalService.syncHealthKitData(datosVitales);
      
      this.addSyncRecord('success', `${datosVitales.length} datos sincronizados`);
    } catch (error) {
      console.error('Error sincronizando:', error);
      this.addSyncRecord('error', 'Error en sincronización');
    } finally {
      this.isSyncing = false;
    }
  }

  loadTodayStats() {
    this.healthSensorService.getTodayStats().subscribe(stats => {
      this.todayStats = stats;
    });
  }

  get estimatedCalories(): number {
    if (!this.currentData?.steps) return 0;
    // Estimación simple: ~0.04 calorías por paso
    return Math.round(this.currentData.steps * 0.04);
  }

  getSourceLabel(source: string): string {
    switch (source) {
      case 'accelerometer': return 'Acelerómetro del dispositivo';
      case 'bluetooth': return 'Dispositivo Bluetooth';
      default: return source.includes('bluetooth') ? source : 'Sensores del dispositivo';
    }
  }

  private addSyncRecord(status: string, message: string) {
    this.syncHistory.unshift({
      timestamp: new Date(),
      status,
      message
    });
    
    // Mantener solo los últimos 10 registros
    if (this.syncHistory.length > 10) {
      this.syncHistory = this.syncHistory.slice(0, 10);
    }
  }
} 