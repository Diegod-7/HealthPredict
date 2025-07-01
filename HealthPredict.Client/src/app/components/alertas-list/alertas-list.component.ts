import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertaService } from '../../services/alerta.service';
import { UsuarioService } from '../../services/usuario.service';
import { Alerta } from '../../models/alerta.model';
import { Usuario } from '../../models/usuario.model';

@Component({
  selector: 'app-alertas-list',
  templateUrl: './alertas-list.component.html',
  styleUrls: ['./alertas-list.component.scss']
})
export class AlertasListComponent implements OnInit {
  alertas: Alerta[] = [];
  loading = false;
  error: string | null = null;
  usuarioActual: Usuario | null = null;

  filtros = {
    soloNoLeidas: false,
    severidad: ''
  };

  constructor(
    private alertaService: AlertaService,
    private usuarioService: UsuarioService,
    private router: Router
  ) { }

  ngOnInit(): void {
    // Obtener usuario actual autenticado
    this.usuarioActual = this.usuarioService.getCurrentUser();
    
    if (!this.usuarioActual) {
      console.log('❌ Usuario no autenticado, redirigiendo al login');
      this.router.navigate(['/login']);
      return;
    }

    console.log('📋 Cargando alertas para usuario:', this.usuarioActual.nombreCompleto);
    this.cargarAlertas();
  }

  cargarAlertas(): void {
    if (!this.usuarioActual?.id) {
      this.error = 'Error: Usuario no autenticado';
      return;
    }

    this.loading = true;
    this.error = null;
    const usuarioId = this.usuarioActual.id;

    if (this.filtros.soloNoLeidas) {
      this.alertaService.getAlertasNoLeidas(usuarioId).subscribe({
        next: (data) => {
          console.log('📋 Alertas no leídas cargadas:', data);
          this.alertas = data;
          this.loading = false;
        },
        error: (err) => {
          console.error('❌ Error al cargar alertas no leídas:', err);
          this.error = 'Error al cargar las alertas: ' + err.message;
          this.loading = false;
          // Crear datos simulados en caso de error
          this.crearAlertasSimuladas();
        }
      });
    } else if (this.filtros.severidad) {
      this.alertaService.getAlertasBySeveridad(usuarioId, this.filtros.severidad).subscribe({
        next: (data) => {
          console.log('📋 Alertas por severidad cargadas:', data);
          this.alertas = data;
          this.loading = false;
        },
        error: (err) => {
          console.error('❌ Error al cargar alertas por severidad:', err);
          this.error = 'Error al cargar las alertas: ' + err.message;
          this.loading = false;
          // Crear datos simulados en caso de error
          this.crearAlertasSimuladas();
        }
      });
    } else {
      this.alertaService.getAlertasByUsuario(usuarioId).subscribe({
        next: (data) => {
          console.log('📋 Todas las alertas cargadas:', data);
          this.alertas = data;
          this.loading = false;
        },
        error: (err) => {
          console.error('❌ Error al cargar todas las alertas:', err);
          this.error = 'Error al cargar las alertas: ' + err.message;
          this.loading = false;
          // Crear datos simulados en caso de error
          this.crearAlertasSimuladas();
        }
      });
    }
  }

  /**
   * Crea alertas simuladas cuando hay problemas de conexión
   */
  private crearAlertasSimuladas(): void {
    if (!this.usuarioActual?.id) return;

    console.log('🔄 Creando alertas simuladas para el usuario');
    this.alertas = [
             {
         id: 1,
         usuarioId: this.usuarioActual.id,
         tipoAlerta: 'Presión Elevándose',
         descripcion: 'Presión 145/97 mmHg - Monitorear tendencia',
         severidad: 'Media',
         leida: false,
         resuelta: false,
         fechaCreacion: new Date(2025, 0, 20, 6, 5),
         fechaResolucion: undefined,
         notasResolucion: undefined
       },
       {
         id: 2,
         usuarioId: this.usuarioActual.id,
         tipoAlerta: 'Estrés Alto',
         descripcion: 'Nivel de estrés 7/10 - Considerar intervención',
         severidad: 'Media',
         leida: false,
         resuelta: false,
         fechaCreacion: new Date(2025, 0, 20, 6, 5),
         fechaResolucion: undefined,
         notasResolucion: undefined
       },
       {
         id: 3,
         usuarioId: this.usuarioActual.id,
         tipoAlerta: 'Frecuencia Cardíaca Irregular',
         descripcion: 'Ritmo cardíaco detectado fuera del rango normal durante ejercicio',
         severidad: 'Alta',
         leida: true,
         resuelta: false,
         fechaCreacion: new Date(2025, 0, 19, 14, 30),
         fechaResolucion: undefined,
         notasResolucion: undefined
       },
      {
        id: 4,
        usuarioId: this.usuarioActual.id,
        tipoAlerta: 'Bajo Nivel de Actividad',
        descripcion: 'Menos de 5000 pasos durante los últimos 3 días',
        severidad: 'Baja',
        leida: true,
        resuelta: true,
        fechaCreacion: new Date(2025, 0, 18, 10, 15),
        fechaResolucion: new Date(2025, 0, 19, 9, 0),
        notasResolucion: 'Usuario retomó rutina de ejercicio'
      }
    ];
    
    this.loading = false;
    console.log('✅ Alertas simuladas creadas:', this.alertas);
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