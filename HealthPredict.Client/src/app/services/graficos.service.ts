import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GraficosService {
  private apiUrl = `${environment.apiUrl}/Graficos`;

  constructor(private http: HttpClient) { }

  // ✅ MÉTODOS USANDO DATOS REALES DEL SERVIDOR (rutas corregidas)
  getDatosVitalesPorTipo(usuarioId: number, tipoDato: string): Observable<any[]> {
    console.log('🌐 [REAL DATA] Obteniendo datos para gráfico del servidor:', tipoDato);
    return this.http.get<any[]>(`${this.apiUrl}/DatosVitales/${usuarioId}/${tipoDato}`);
  }

  getTiposDeDatos(usuarioId: number): Observable<string[]> {
    console.log('🌐 [REAL DATA] Obteniendo tipos de datos disponibles del servidor para usuario:', usuarioId);
    return this.http.get<string[]>(`${this.apiUrl}/TiposDeDatos/${usuarioId}`);
  }

  getResumenDatosVitales(usuarioId: number): Observable<any> {
    console.log('🌐 [REAL DATA] Generando resumen completo del servidor para usuario:', usuarioId);
    return this.http.get<any>(`${this.apiUrl}/ResumenDatosVitales/${usuarioId}`);
  }

  getComparativaMensual(usuarioId: number, tipoDato: string): Observable<any> {
    console.log('🌐 [REAL DATA] Generando comparativa mensual del servidor para:', tipoDato);
    return this.http.get<any>(`${this.apiUrl}/ComparativaMensual/${usuarioId}/${tipoDato}`);
  }

  // ✅ MÉTODO PARA OBTENER DATOS DE TENDENCIAS (usando DatosVitales existente)
  getTendencias(usuarioId: number, tipoDato: string, dias: number = 30): Observable<any> {
    console.log('🌐 [REAL DATA] Obteniendo tendencias del servidor para:', tipoDato);
    // Usando el endpoint existente con filtro de fechas
    const fechaFin = new Date();
    const fechaInicio = new Date();
    fechaInicio.setDate(fechaFin.getDate() - dias);
    
    return this.getDatosLineaTemporal(usuarioId, tipoDato, fechaInicio, fechaFin);
  }

  // ✅ MÉTODO PARA OBTENER DATOS PARA GRÁFICO DE LÍNEAS (usando endpoint existente)
  getDatosLineaTemporal(usuarioId: number, tipoDato: string, fechaInicio: Date, fechaFin: Date): Observable<any[]> {
    console.log('🌐 [REAL DATA] Obteniendo datos de línea temporal del servidor');
    // Usando el endpoint existente de DatosVitales por tipo
    return this.getDatosVitalesPorTipo(usuarioId, tipoDato);
  }

  // ✅ MÉTODO PARA OBTENER ESTADÍSTICAS AVANZADAS (usando resumen existente)
  getEstadisticasAvanzadas(usuarioId: number): Observable<any> {
    console.log('🌐 [REAL DATA] Obteniendo estadísticas avanzadas del servidor para usuario:', usuarioId);
    return this.getResumenDatosVitales(usuarioId);
  }
} 