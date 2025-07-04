import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DatoVital } from '../models/dato-vital.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DatoVitalService {
  private apiUrl = `${environment.apiUrl}/DatosVitales`;

  constructor(private http: HttpClient) { }

  // ✅ MÉTODOS USANDO DATOS REALES DEL SERVIDOR (rutas corregidas)
  getDatosVitalesByUsuario(usuarioId: number): Observable<DatoVital[]> {
    console.log('🌐 [REAL DATA] Cargando datos vitales del servidor para usuario ID:', usuarioId);
    return this.http.get<DatoVital[]>(`${this.apiUrl}/Usuario/${usuarioId}`);
  }

  getDatoVital(id: number): Observable<DatoVital> {
    console.log('🌐 [REAL DATA] Obteniendo dato vital ID:', id, 'del servidor');
    return this.http.get<DatoVital>(`${this.apiUrl}/${id}`);
  }

  createDatoVital(datoVital: DatoVital): Observable<DatoVital> {
    console.log('🌐 [REAL DATA] Creando dato vital en servidor:', datoVital);
    return this.http.post<DatoVital>(this.apiUrl, datoVital);
  }

  updateDatoVital(datoVital: DatoVital): Observable<void> {
    console.log('🌐 [REAL DATA] Actualizando dato vital en servidor:', datoVital);
    return this.http.put<void>(`${this.apiUrl}/${datoVital.id}`, datoVital);
  }

  deleteDatoVital(id: number): Observable<void> {
    console.log('🌐 [REAL DATA] Eliminando dato vital ID:', id, 'del servidor');
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getDatosVitalesByUsuarioAndDateRange(usuarioId: number, fechaInicio: Date, fechaFin: Date): Observable<DatoVital[]> {
    console.log('🌐 [REAL DATA] Obteniendo datos por rango de fechas del servidor');
    
    const params = new HttpParams()
      .set('fechaInicio', fechaInicio.toISOString())
      .set('fechaFin', fechaFin.toISOString());

    return this.http.get<DatoVital[]>(`${this.apiUrl}/Usuario/${usuarioId}`, { params });
  }

  getDatosVitalesByTipo(usuarioId: number, tipoDato: string): Observable<DatoVital[]> {
    console.log('🌐 [REAL DATA] Obteniendo datos por tipo del servidor:', tipoDato);
    // Usando el endpoint de gráficos que ya existe para obtener datos por tipo
    return this.http.get<DatoVital[]>(`${environment.apiUrl}/Graficos/DatosVitales/${usuarioId}/${tipoDato}`);
  }

  getEstadisticas(usuarioId: number, tipoDato: string, fechaInicio: Date, fechaFin: Date): Observable<any> {
    console.log('🌐 [REAL DATA] Calculando estadísticas en servidor para:', tipoDato);
    
    const params = new HttpParams()
      .set('tipoDato', tipoDato)
      .set('fechaInicio', fechaInicio.toISOString())
      .set('fechaFin', fechaFin.toISOString());

    // Usando el endpoint de estadísticas que existe en el controlador
    return this.http.get<any>(`${this.apiUrl}/estadisticas`, { params });
  }

  // ✅ MÉTODO PARA OBTENER TIPOS DE DATOS DISPONIBLES (usando el endpoint de gráficos)
  getTiposDeDatos(usuarioId: number): Observable<string[]> {
    console.log('🌐 [REAL DATA] Obteniendo tipos de datos del servidor para usuario:', usuarioId);
    return this.http.get<string[]>(`${environment.apiUrl}/Graficos/TiposDeDatos/${usuarioId}`);
  }

  // ✅ MÉTODO PARA OBTENER RESUMEN DE DATOS VITALES (usando el endpoint de gráficos)
  getResumenDatosVitales(usuarioId: number): Observable<any> {
    console.log('🌐 [REAL DATA] Generando resumen desde servidor para usuario:', usuarioId);
    return this.http.get<any>(`${environment.apiUrl}/Graficos/ResumenDatosVitales/${usuarioId}`);
  }

  // ✅ MÉTODO PARA SINCRONIZAR DATOS DE HEALTHKIT/SENSORES
  syncHealthKitData(healthKitData: any[]): Observable<any> {
    console.log('🌐 [HEALTH SYNC] Sincronizando datos de sensores al servidor:', healthKitData);
    return this.http.post<any>(`${this.apiUrl}/Sync/HealthKit`, healthKitData);
  }

  // ✅ MÉTODO PARA OBTENER ÚLTIMA FECHA DE SINCRONIZACIÓN
  getLastSyncDate(usuarioId: number): Observable<Date | null> {
    console.log('🌐 [HEALTH SYNC] Obteniendo última fecha de sincronización para usuario:', usuarioId);
    return this.http.get<Date | null>(`${this.apiUrl}/LastSync/${usuarioId}`);
  }
}
