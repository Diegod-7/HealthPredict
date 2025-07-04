import { Component, OnInit } from '@angular/core';
import { GoogleDriveSyncService, SyncResult } from '../../services/google-drive-sync.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-google-drive-sync',
  templateUrl: './google-drive-sync.component.html',
  styleUrls: ['./google-drive-sync.component.scss']
})
export class GoogleDriveSyncComponent implements OnInit {
  sincronizando = false;
  ultimaSincronizacion: Date | null = null;
  ultimoResultado: SyncResult | null = null;

  constructor(
    private googleDriveSyncService: GoogleDriveSyncService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.cargarUltimaSincronizacion();
  }

  sincronizarPasos(): void {
    if (this.sincronizando) return;

    this.sincronizando = true;
    this.snackBar.open('🔄 Sincronizando datos de pasos desde Google Drive...', '', {
      duration: 0 // No se cierra automáticamente
    });

    // Usar el endpoint real de sincronización con Google Drive
    this.googleDriveSyncService.syncPasosFromGoogleDrive().subscribe({
      next: (resultado: SyncResult) => {
        this.sincronizando = false;
        this.ultimoResultado = resultado;
        this.ultimaSincronizacion = new Date();
        
        this.snackBar.dismiss();
        
        if (resultado.success) {
          this.snackBar.open('✅ Datos de pasos sincronizados exitosamente', 'Cerrar', {
            duration: 5000,
            panelClass: ['success-snackbar']
          });
        } else {
          this.snackBar.open(`❌ Error: ${resultado.message}`, 'Cerrar', {
            duration: 8000,
            panelClass: ['error-snackbar']
          });
        }
      },
      error: (error) => {
        this.sincronizando = false;
        this.snackBar.dismiss();
        
        console.error('Error en sincronización:', error);
        this.snackBar.open('❌ Error de conexión durante la sincronización', 'Cerrar', {
          duration: 8000,
          panelClass: ['error-snackbar']
        });
      }
    });
  }

  private cargarUltimaSincronizacion(): void {
    this.googleDriveSyncService.getUltimaSincronizacion().subscribe({
      next: (data) => {
        if (data && data.ultimaSincronizacion) {
          this.ultimaSincronizacion = new Date(data.ultimaSincronizacion);
        }
      },
      error: (error) => {
        console.log('No se pudo cargar información de última sincronización:', error);
      }
    });
  }

  formatearTiempo(fecha: Date): string {
    const ahora = new Date();
    const diferencia = ahora.getTime() - fecha.getTime();
    const minutos = Math.floor(diferencia / (1000 * 60));
    const horas = Math.floor(minutos / 60);
    const dias = Math.floor(horas / 24);

    if (dias > 0) {
      return `hace ${dias} día${dias > 1 ? 's' : ''}`;
    } else if (horas > 0) {
      return `hace ${horas} hora${horas > 1 ? 's' : ''}`;
    } else if (minutos > 0) {
      return `hace ${minutos} minuto${minutos > 1 ? 's' : ''}`;
    } else {
      return 'hace un momento';
    }
  }
} 