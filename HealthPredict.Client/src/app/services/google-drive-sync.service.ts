import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, from } from 'rxjs';
import { environment } from '../../environments/environment';

export interface SyncResult {
  success: boolean;
  message: string;
  error?: string;
  response?: any;
  file_info?: {
    name: string;
    modified: string;
    size: string;
  };
}

@Injectable({
  providedIn: 'root'
})
export class GoogleDriveSyncService {
  private apiUrl = environment.apiUrl || 'https://healthpredict-l1hu.onrender.com/api';

  constructor(private http: HttpClient) { }

  /**
   * Sincroniza datos de pasos desde Google Drive
   * Nota: En un entorno real, esto requeriría un endpoint en el backend
   * que ejecute el script de Python
   */
  syncPasosFromGoogleDrive(): Observable<SyncResult> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    // Endpoint que ejecutará el script de sincronización
    return this.http.post<SyncResult>(`${this.apiUrl}/HealthAutoExport/sync-google-drive`, {}, { headers });
  }

  /**
   * Método alternativo para desarrollo/pruebas
   * Simula la sincronización con datos de ejemplo
   */
  syncPasosSimulado(): Observable<SyncResult> {
    const datosEjemplo = [
      {
        date: new Date().toISOString().split('T')[0],
        steps: Math.floor(Math.random() * 5000) + 5000,
        distance: Math.round((Math.random() * 3 + 3) * 100) / 100,
        calories: Math.floor(Math.random() * 200) + 200
      }
    ];

    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    return this.http.post<SyncResult>(`${this.apiUrl}/HealthAutoExport/pasos`, datosEjemplo, { headers });
  }

  /**
   * Obtener estado de la última sincronización
   */
  getUltimaSincronizacion(): Observable<any> {
    return this.http.get(`${this.apiUrl}/HealthAutoExport/ultima-sincronizacion`);
  }
} 