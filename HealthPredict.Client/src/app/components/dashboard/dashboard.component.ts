import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertaService } from '../../services/alerta.service';
import { UsuarioService } from '../../services/usuario.service';
import { Alerta } from '../../models/alerta.model';
import { Usuario } from '../../models/usuario.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  // ✅ USUARIO ACTUAL (TRABAJADOR)
  usuarioActual: Usuario | null = null;
  
  // ✅ DATOS DEL DASHBOARD
  alertas: Alerta[] = [];
  alertasNoLeidas: Alerta[] = [];
  alertasAlta: Alerta[] = [];
  loading = false;
  error: string | null = null;

  constructor(
    private router: Router,
    private alertaService: AlertaService,
    private usuarioService: UsuarioService
  ) { }

  ngOnInit(): void {
    this.usuarioActual = this.usuarioService.getCurrentUser();
    
    if (!this.usuarioActual) {
      console.log('❌ Usuario no autenticado, redirigiendo al login');
      this.router.navigate(['/login']);
      return;
    }

    if (this.usuarioActual.rol === 'Jefe') {
      console.log('🔄 Usuario es jefe, redirigiendo a dashboard de jefe');
      this.router.navigate(['/dashboard-jefe']);
      return;
    }

    console.log('👨‍💻 Dashboard de trabajador cargando para:', this.usuarioActual.nombreCompleto);
    this.cargarDatos();
  }

  cargarDatos(): void {
    if (!this.usuarioActual?.id) {
      this.error = 'Error: ID de usuario no encontrado';
      return;
    }

    this.loading = true;
    this.error = null;
    
    // Cargar todas las alertas del usuario actual
    this.alertaService.getAlertasByUsuario(this.usuarioActual.id).subscribe({
      next: (data) => {
        console.log('📊 Alertas cargadas para trabajador:', data);
        this.alertas = data;
        this.alertasNoLeidas = data.filter(a => !a.leida);
        this.alertasAlta = data.filter(a => a.severidad.toLowerCase() === 'alta');
        this.loading = false;
      },
      error: (err) => {
        console.error('❌ Error al cargar alertas:', err);
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

  // ✅ NUEVOS MÉTODOS PARA EL SISTEMA DE PERFILAMIENTO

  /**
   * Cierra sesión del usuario
   */
  logout(): void {
    console.log('🚪 Cerrando sesión del trabajador');
    this.usuarioService.logout();
    this.router.navigate(['/login']);
  }

  /**
   * Recarga los datos del dashboard
   */
  recargarDatos(): void {
    console.log('🔄 Recargando datos del dashboard');
    this.cargarDatos();
  }
} 