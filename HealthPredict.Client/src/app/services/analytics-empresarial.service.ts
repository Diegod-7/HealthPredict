import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AnalyticsEmpresarialService {
  private apiUrl = `${environment.apiUrl}/Analytics`;

  constructor(private http: HttpClient) { }

  /**
   * Obtiene analytics completo para el módulo empresarial
   */
  getAnalyticsCompleto(jefeId: number, periodo: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/completo/${jefeId}?periodo=${periodo}`);
  }

  /**
   * Obtiene ROI específico en salud y productividad
   */
  getROISalud(jefeId: number, periodo: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/roi-salud/${jefeId}?periodo=${periodo}`);
  }

  /**
   * Obtiene comparativas detalladas por departamentos
   */
  getComparativasDepartamentos(jefeId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/comparativas-departamentos/${jefeId}`);
  }

  /**
   * Obtiene tendencias de ausentismo
   */
  getTendenciasAusentismo(jefeId: number, periodo: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/tendencias-ausentismo/${jefeId}?periodo=${periodo}`);
  }

  /**
   * Obtiene efectividad de programas wellness
   */
  getEfectividadWellness(jefeId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/efectividad-wellness/${jefeId}`);
  }

  /**
   * Obtiene benchmarking interno y sectorial
   */
  getBenchmarking(jefeId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/benchmarking/${jefeId}`);
  }

  /**
   * Exporta reporte de analytics empresarial
   */
  exportarReporte(jefeId: number, tipo: string, periodo: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/exportar/${jefeId}?tipo=${tipo}&periodo=${periodo}`, {
      responseType: 'blob'
    });
  }
} 