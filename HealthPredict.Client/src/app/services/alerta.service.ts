import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Alerta } from '../models/alerta.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AlertaService {
  private apiUrl = `${environment.apiUrl}/Alertas`;

  constructor(private http: HttpClient) { }

  // ✅ MÉTODOS USANDO DATOS REALES DEL SERVIDOR (rutas existentes)
  getAlertasByUsuario(usuarioId: number): Observable<Alerta[]> {
    console.log('🌐 [REAL DATA] Cargando alertas del servidor para usuario ID:', usuarioId);
    return this.http.get<Alerta[]>(`${this.apiUrl}/Usuario/${usuarioId}`);
  }

  // TODO: Este endpoint necesita ser implementado en el backend
  getAlertasNoLeidas(usuarioId: number): Observable<Alerta[]> {
    console.log('🌐 [REAL DATA] Obteniendo alertas no leídas del servidor para usuario:', usuarioId);
    // Temporalmente usando todas las alertas y filtrando en el cliente
    return this.getAlertasByUsuario(usuarioId);
  }

  // TODO: Este endpoint necesita ser implementado en el backend
  getAlertasBySeveridad(usuarioId: number, severidad: string): Observable<Alerta[]> {
    console.log('🌐 [REAL DATA] Filtrando alertas por severidad del servidor:', severidad);
    // Temporalmente usando todas las alertas y filtrando en el cliente
    return this.getAlertasByUsuario(usuarioId);
  }

  getAlerta(id: number): Observable<Alerta> {
    console.log('🌐 [REAL DATA] Obteniendo alerta ID:', id, 'del servidor');
    return this.http.get<Alerta>(`${this.apiUrl}/${id}`);
  }

  createAlerta(alerta: Alerta): Observable<Alerta> {
    console.log('🌐 [REAL DATA] Creando alerta en servidor:', alerta);
    return this.http.post<Alerta>(this.apiUrl, alerta);
  }

  updateAlerta(alerta: Alerta): Observable<void> {
    console.log('🌐 [REAL DATA] Actualizando alerta en servidor:', alerta);
    return this.http.put<void>(`${this.apiUrl}/${alerta.id}`, alerta);
  }

  deleteAlerta(id: number): Observable<void> {
    console.log('🌐 [REAL DATA] Eliminando alerta ID:', id, 'del servidor');
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  marcarComoLeida(id: number): Observable<Alerta> {
    console.log('🌐 [REAL DATA] Marcando alerta como leída en servidor, ID:', id);
    return this.http.post<Alerta>(`${this.apiUrl}/${id}/MarcarLeida`, {});
  }

  resolverAlerta(id: number, notasResolucion: string): Observable<Alerta> {
    console.log('🌐 [REAL DATA] Resolviendo alerta en servidor, ID:', id);
    const body = { NotasResolucion: notasResolucion };
    return this.http.post<Alerta>(`${this.apiUrl}/${id}/Resolver`, body);
  }

  // TODO: Este endpoint necesita ser implementado en el backend
  getEstadisticasAlertas(usuarioId: number): Observable<any> {
    console.log('🌐 [REAL DATA] Obteniendo estadísticas de alertas del servidor para usuario:', usuarioId);
    // Temporalmente retornando un observable vacío
    // return this.http.get<any>(`${this.apiUrl}/Usuario/${usuarioId}/Estadisticas`);
    
    // Por ahora, vamos a calcular estadísticas básicas del lado del cliente
    return new Observable(observer => {
      this.getAlertasByUsuario(usuarioId).subscribe(alertas => {
        const estadisticas = {
          total: alertas.length,
          noLeidas: alertas.filter(a => !a.leida).length,
          resueltas: alertas.filter(a => a.resuelta).length,
          porSeveridad: {
            Alta: alertas.filter(a => a.severidad === 'Alta').length,
            Media: alertas.filter(a => a.severidad === 'Media').length,
            Baja: alertas.filter(a => a.severidad === 'Baja').length
          }
        };
        observer.next(estadisticas);
        observer.complete();
      });
    });
  }
}
