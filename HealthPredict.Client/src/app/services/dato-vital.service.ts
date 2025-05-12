import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DatoVital } from '../models/dato-vital.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DatoVitalService {
  private apiUrl = `${environment.apiUrl}/datosvitales`;

  constructor(private http: HttpClient) { }

  getDatosVitalesByUsuario(usuarioId: number): Observable<DatoVital[]> {
    return this.http.get<DatoVital[]>(`${this.apiUrl}/usuario/${usuarioId}`);
  }

  getDatoVital(id: number): Observable<DatoVital> {
    return this.http.get<DatoVital>(`${this.apiUrl}/${id}`);
  }

  createDatoVital(datoVital: DatoVital): Observable<DatoVital> {
    return this.http.post<DatoVital>(this.apiUrl, datoVital);
  }

  updateDatoVital(datoVital: DatoVital): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${datoVital.id}`, datoVital);
  }

  deleteDatoVital(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getEstadisticas(usuarioId: number, tipoDato: string, fechaInicio: Date, fechaFin: Date): Observable<any> {
    const params = new HttpParams()
      .set('usuarioId', usuarioId.toString())
      .set('tipoDato', tipoDato)
      .set('fechaInicio', fechaInicio.toISOString())
      .set('fechaFin', fechaFin.toISOString());
    
    return this.http.get<any>(`${this.apiUrl}/estadisticas`, { params });
  }
}
