import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ReportesService {
  private apiUrl = `${environment.apiUrl}/Reportes`;

  constructor(private http: HttpClient) { }

  /**
   * Genera un reporte de datos vitales para un usuario específico
   * @param usuarioId - ID del usuario
   * @param tipoDato - Tipo de dato (opcional)
   * @returns Blob con el PDF generado
   */
  generarReporteDatosVitales(usuarioId: number, tipoDato?: string): Observable<Blob> {
    let url = `${this.apiUrl}/DatosVitales/${usuarioId}`;
    
    if (tipoDato) {
      url += `?tipoDato=${encodeURIComponent(tipoDato)}`;
    }
    
    return this.http.get(url, {
      responseType: 'blob'
    }).pipe(
      catchError((error: HttpErrorResponse) => {
        // Intentar leer el mensaje de error si es una respuesta del servidor
        if (error.status === 503) {
          // Error específico de falta de librería PDF
          return throwError(() => new Error('El servicio de generación de PDF no está disponible en el servidor. El administrador debe instalar la librería wkhtmltopdf.'));
        } else if (error.error instanceof Blob) {
          // Si el error es un Blob, intentar leerlo como texto
          return new Observable<any>(observer => {
            const reader = new FileReader();
            reader.onload = (e: any) => {
              try {
                const errorJson = JSON.parse(e.target.result);
                observer.error(new Error(errorJson.message || 'Error al generar el reporte'));
              } catch (e) {
                observer.error(new Error('Error al generar el reporte'));
              }
              observer.complete();
            };
            reader.onerror = () => {
              observer.error(new Error('Error al generar el reporte'));
              observer.complete();
            };
            reader.readAsText(error.error);
          });
        }
        
        return throwError(() => new Error('Error al generar el reporte. Por favor intente nuevamente.'));
      })
    );
  }
}
