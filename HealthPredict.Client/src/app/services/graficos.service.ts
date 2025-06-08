import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GraficosService {
  private apiUrl = `${environment.apiUrl}/graficos`;

  constructor(private http: HttpClient) { }

  getDatosVitalesPorTipo(usuarioId: number, tipoDato: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/DatosVitales/${usuarioId}/${tipoDato}`);
  }

  getTiposDeDatos(usuarioId: number): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/TiposDeDatos/${usuarioId}`);
  }

  getResumenDatosVitales(usuarioId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/ResumenDatosVitales/${usuarioId}`);
  }

  getComparativaMensual(usuarioId: number, tipoDato: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/ComparativaMensual/${usuarioId}/${tipoDato}`);
  }
} 