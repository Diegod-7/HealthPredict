import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface PasosHoy {
  fecha: string;
  totalPasos: number;
  registros: number;
  ultimaActualizacion: string;
  datosGrafico: PasosPorHora[];
}

export interface PasosPorHora {
  hora: number;
  horaTexto: string;
  pasos: number;
  registros: number;
}

export interface PasosSemana {
  fechaInicio: string;
  fechaFin: string;
  totalPasos: number;
  promedioDiario: number;
  diasConDatos: number;
  datosGrafico: PasosPorDia[];
}

export interface PasosPorDia {
  fecha: string;
  fechaTexto: string;
  diaSemana: string;
  pasos: number;
  registros: number;
  esHoy: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class PasosService {
  private apiUrl = environment.apiUrl || 'https://healthpredict-l1hu.onrender.com/api';

  constructor(private http: HttpClient) { }

  /**
   * Obtener resumen de pasos del día actual
   */
  getPasosHoy(usuarioId: number): Observable<PasosHoy> {
    return this.http.get<PasosHoy>(`${this.apiUrl}/DatosVitales/pasos-hoy/${usuarioId}`);
  }

  /**
   * Obtener resumen de pasos de los últimos 7 días
   */
  getPasosSemana(usuarioId: number): Observable<PasosSemana> {
    return this.http.get<PasosSemana>(`${this.apiUrl}/DatosVitales/pasos-semana/${usuarioId}`);
  }
} 