import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Alerta } from '../models/alerta.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AlertaService {
  private apiUrl = `${environment.apiUrl}/alertas`;

  constructor(private http: HttpClient) { }

  getAlertasByUsuario(usuarioId: number): Observable<Alerta[]> {
    return this.http.get<Alerta[]>(`${this.apiUrl}/usuario/${usuarioId}`);
  }

  getAlerta(id: number): Observable<Alerta> {
    return this.http.get<Alerta>(`${this.apiUrl}/${id}`);
  }

  createAlerta(alerta: Alerta): Observable<Alerta> {
    return this.http.post<Alerta>(this.apiUrl, alerta);
  }

  updateAlerta(alerta: Alerta): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${alerta.id}`, alerta);
  }

  deleteAlerta(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  marcarComoLeida(id: number): Observable<Alerta> {
    return this.http.post<Alerta>(`${this.apiUrl}/${id}/marcarleida`, {});
  }

  resolverAlerta(id: number, notasResolucion: string): Observable<Alerta> {
    return this.http.post<Alerta>(`${this.apiUrl}/${id}/resolver`, { notasResolucion });
  }
}
