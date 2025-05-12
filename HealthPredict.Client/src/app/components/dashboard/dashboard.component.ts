import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertaService } from '../../services/alerta.service';
import { Alerta } from '../../models/alerta.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  alertas: Alerta[] = [];
  alertasNoLeidas: Alerta[] = [];
  alertasAlta: Alerta[] = [];
  loading = false;
  error: string | null = null;
  usuarioId = 1; // Esto debería obtenerse de un servicio de autenticación

  constructor(
    private router: Router,
    private alertaService: AlertaService
  ) { }

  ngOnInit(): void {
    this.cargarDatos();
  }

  cargarDatos(): void {
    this.loading = true;
    this.error = null;
    
    // Cargar todas las alertas
    this.alertaService.getAlertasByUsuario(this.usuarioId).subscribe({
      next: (data) => {
        this.alertas = data;
        this.alertasNoLeidas = data.filter(a => !a.leida);
        this.alertasAlta = data.filter(a => a.severidad.toLowerCase() === 'alta');
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Error al cargar las alertas: ' + err.message;
        this.loading = false;
      }
    });
  }

  verAlerta(id: number): void {
    this.router.navigate(['/alertas', id]);
  }

  verListaAlertas(): void {
    this.router.navigate(['/alertas']);
  }

  obtenerResumenAlertas(): { total: number, noLeidas: number, alta: number, media: number, baja: number } {
    const media = this.alertas.filter(a => a.severidad.toLowerCase() === 'media').length;
    const baja = this.alertas.filter(a => a.severidad.toLowerCase() === 'baja').length;
    
    return {
      total: this.alertas.length,
      noLeidas: this.alertasNoLeidas.length,
      alta: this.alertasAlta.length,
      media: media,
      baja: baja
    };
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