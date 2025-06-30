import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface DashboardSupervisor {
  supervisor: {
    id: number;
    nombre: string;
    departamento: string;
    cargo: string;
  };
  resumenGeneral: {
    totalSubordinados: number;
    alertasActivas: number;
    alertasCriticas: number;
    alertasAltas: number;
    trabajadoresEnRiesgo: number;
    promedioScoreBienestar: number;
  };
  alertasRecientes: AlertaReciente[];
  resumenSubordinados: ResumenSubordinado[];
  metricasDepartamento: MetricasDepartamento;
  tendenciasSalud: TendenciasSalud;
  trabajadoresEnRiesgo: TrabajadorEnRiesgo[];
  fechaActualizacion: Date;
}

export interface AlertaReciente {
  id: number;
  usuarioId: number;
  usuario: string;
  tipoAlerta: string;
  severidad: string;
  descripcion: string;
  fechaCreacion: Date;
  leida: boolean;
}

export interface ResumenSubordinado {
  id: number;
  nombre: string;
  cargo: string;
  departamento: string;
  email: string;
  alertasActivas: number;
  ultimosDatos: DatoVitalResumen[];
  ultimoAcceso: Date;
  scoreBienestar: number;
  nivelRiesgo: string;
}

export interface DatoVitalResumen {
  tipoDato: string;
  ultimoValor: number;
  promedio: number;
  unidad: string;
}

export interface MetricasDepartamento {
  departamento: string;
  totalUsuarios: number;
  alertasPorSeveridad: {
    severidad: string;
    cantidad: number;
  }[];
  promediosDepartamento: {
    tipoDato: string;
    promedio: number;
    cantidad: number;
    unidad: string;
  }[];
}

export interface TendenciasSalud {
  alertasDiarias: {
    fecha: Date;
    cantidad: number;
    criticas: number;
  }[];
  resumen: {
    alertasUltimos7Dias: number;
    alertasCriticasUltimos7Dias: number;
  };
}

export interface TrabajadorEnRiesgo {
  usuarioId: number;
  usuario: string;
  cargo: string;
  alertasCriticas?: number;
  alertasAltas?: number;
  ultimaAlerta: string;
  fechaUltimaAlerta: Date;
  nivelRiesgo: string;
}

@Injectable({
  providedIn: 'root'
})
export class SupervisorService {
  private apiUrl = `${environment.apiUrl}/Supervisor`;

  constructor(private http: HttpClient) { }

  /**
   * Obtiene el dashboard completo del supervisor
   */
  getDashboardSupervisor(jefeId: number): Observable<DashboardSupervisor> {
    return this.http.get<DashboardSupervisor>(`${this.apiUrl}/dashboard/${jefeId}`);
  }

  /**
   * Obtiene métricas específicas de un departamento
   */
  getMetricasDepartamento(departamento: string): Observable<MetricasDepartamento> {
    return this.http.get<MetricasDepartamento>(`${this.apiUrl}/metricas-departamento/${departamento}`);
  }

  /**
   * Obtiene trabajadores en riesgo de un supervisor
   */
  getTrabajadoresEnRiesgo(jefeId: number): Observable<{trabajadoresEnRiesgo: TrabajadorEnRiesgo[], fechaActualizacion: Date}> {
    return this.http.get<{trabajadoresEnRiesgo: TrabajadorEnRiesgo[], fechaActualizacion: Date}>(`${this.apiUrl}/trabajadores-riesgo/${jefeId}`);
  }

  /**
   * Obtiene tendencias de salud del equipo
   */
  getTendenciasSalud(jefeId: number): Observable<{tendenciasSalud: TendenciasSalud, fechaActualizacion: Date}> {
    return this.http.get<{tendenciasSalud: TendenciasSalud, fechaActualizacion: Date}>(`${this.apiUrl}/tendencias-salud/${jefeId}`);
  }

  /**
   * Obtiene el color CSS basado en el nivel de riesgo
   */
  getColorNivelRiesgo(nivelRiesgo: string): string {
    switch (nivelRiesgo?.toLowerCase()) {
      case 'crítico':
        return '#ff4757'; // Rojo
      case 'alto':
        return '#ff6b35'; // Naranja
      case 'moderado':
        return '#ffa502'; // Amarillo
      case 'bajo':
        return '#26de81'; // Verde
      default:
        return '#747d8c'; // Gris
    }
  }

  /**
   * Obtiene el icono basado en el nivel de riesgo
   */
  getIconoNivelRiesgo(nivelRiesgo: string): string {
    switch (nivelRiesgo?.toLowerCase()) {
      case 'crítico':
        return '🚨';
      case 'alto':
        return '⚠️';
      case 'moderado':
        return '⚡';
      case 'bajo':
        return '✅';
      default:
        return '❓';
    }
  }

  /**
   * Obtiene el color CSS basado en la severidad de alerta
   */
  getColorSeveridad(severidad: string): string {
    switch (severidad?.toLowerCase()) {
      case 'crítica':
        return '#ff4757'; // Rojo
      case 'alta':
        return '#ff6b35'; // Naranja
      case 'media':
        return '#ffa502'; // Amarillo
      case 'baja':
        return '#26de81'; // Verde
      default:
        return '#747d8c'; // Gris
    }
  }

  /**
   * Formatea el score de bienestar con color apropiado
   */
  getColorScoreBienestar(score: number): string {
    if (score >= 85) return '#26de81'; // Verde
    if (score >= 70) return '#ffa502'; // Amarillo
    if (score >= 50) return '#ff6b35'; // Naranja
    return '#ff4757'; // Rojo
  }
} 