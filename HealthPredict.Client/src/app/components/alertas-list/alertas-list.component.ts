import { Component, OnInit } from '@angular/core';
import { AlertaService } from '../../services/alerta.service';
import { Alerta } from '../../models/alerta.model';

@Component({
  selector: 'app-alertas-list',
  templateUrl: './alertas-list.component.html',
  styleUrls: ['./alertas-list.component.scss']
})
export class AlertasListComponent implements OnInit {
  alertas: Alerta[] = [];
  loading = false;
  error: string | null = null;
  usuarioId = 1; // Esto debería obtenerse de un servicio de autenticación

  filtros = {
    soloNoLeidas: false,
    severidad: ''
  };

  constructor(private alertaService: AlertaService) { }

  ngOnInit(): void {
    this.cargarAlertas();
  }

  cargarAlertas(): void {
    this.loading = true;
    this.error = null;

    if (this.filtros.soloNoLeidas) {
      this.alertaService.getAlertasNoLeidas(this.usuarioId).subscribe({
        next: (data) => {
          this.alertas = data;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Error al cargar las alertas: ' + err.message;
          this.loading = false;
        }
      });
    } else if (this.filtros.severidad) {
      this.alertaService.getAlertasBySeveridad(this.usuarioId, this.filtros.severidad).subscribe({
        next: (data) => {
          this.alertas = data;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Error al cargar las alertas: ' + err.message;
          this.loading = false;
        }
      });
    } else {
      this.alertaService.getAlertasByUsuario(this.usuarioId).subscribe({
        next: (data) => {
          this.alertas = data;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Error al cargar las alertas: ' + err.message;
          this.loading = false;
        }
      });
    }
  }

  marcarComoLeida(id: number): void {
    this.alertaService.marcarComoLeida(id).subscribe({
      next: (alertaActualizada) => {
        // Actualizar la alerta en la lista
        const index = this.alertas.findIndex(a => a.id === id);
        if (index !== -1) {
          this.alertas[index] = alertaActualizada;
        }
      },
      error: (err) => {
        this.error = 'Error al marcar como leída: ' + err.message;
      }
    });
  }

  resolverAlerta(id: number, notasResolucion: string): void {
    this.alertaService.resolverAlerta(id, notasResolucion).subscribe({
      next: (alertaActualizada) => {
        // Actualizar la alerta en la lista
        const index = this.alertas.findIndex(a => a.id === id);
        if (index !== -1) {
          this.alertas[index] = alertaActualizada;
        }
      },
      error: (err) => {
        this.error = 'Error al resolver la alerta: ' + err.message;
      }
    });
  }

  aplicarFiltros(): void {
    this.cargarAlertas();
  }

  limpiarFiltros(): void {
    this.filtros = {
      soloNoLeidas: false,
      severidad: ''
    };
    this.cargarAlertas();
  }

  getSeveridadClass(severidad: string): string {
    switch(severidad.toLowerCase()) {
      case 'alta': return 'badge bg-danger';
      case 'media': return 'badge bg-warning';
      case 'baja': return 'badge bg-info';
      default: return 'badge bg-secondary';
    }
  }
} 