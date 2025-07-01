import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UsuarioService } from '../../services/usuario.service';
import { SupervisorService, DashboardSupervisor } from '../../services/supervisor.service';
// import { AnalyticsEmpresarialService } from '../../services/analytics-empresarial.service';
import { Usuario } from '../../models/usuario.model';

@Component({
  selector: 'app-analytics-empresarial',
  templateUrl: './analytics-empresarial.component.html',
  styleUrls: ['./analytics-empresarial.component.scss']
})
export class AnalyticsEmpresarialComponent implements OnInit {

  // USUARIO ACTUAL
  usuarioActual: Usuario | null = null;
  
  // DATOS DE ANALYTICS
  datosAnalytics: any = null;
  dashboardData: DashboardSupervisor | null = null;
  
  // ESTADO DE CARGA
  isLoading: boolean = true;
  error: string = '';

  // CONFIGURACIÓN DE VISTA
  vistaActual: 'roi' | 'departamentos' | 'tendencias' | 'wellness' | 'resumen' = 'resumen';
  
  // FILTROS
  periodoSeleccionado: '7d' | '30d' | '90d' | '1y' = '30d';
  departamentoSeleccionado: string = 'todos';
  
  // DEPARTAMENTOS DISPONIBLES
  departamentos: string[] = ['Desarrollo', 'Marketing', 'RRHH', 'Administración'];

  constructor(
    private usuarioService: UsuarioService,
    private supervisorService: SupervisorService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.usuarioActual = this.usuarioService.getCurrentUser();
    
    if (!this.usuarioActual) {
      console.log('❌ Usuario no autenticado');
      this.router.navigate(['/login']);
      return;
    }

    // Verificar que sea jefe
    if (this.usuarioActual.rol !== 'Jefe' && !this.usuarioActual.esJefe) {
      console.log('❌ Acceso denegado: Usuario no es jefe');
      this.router.navigate(['/dashboard']);
      return;
    }

    console.log('📊 Cargando Analytics Empresarial para:', this.usuarioActual.nombreCompleto);
    this.cargarDatosAnalytics();
  }

  /**
   * Carga todos los datos de analytics empresarial
   */
  cargarDatosAnalytics(): void {
    this.isLoading = true;
    this.error = '';

    if (!this.usuarioActual?.id) {
      this.error = 'Error: ID de usuario no encontrado';
      this.isLoading = false;
      return;
    }

    // Cargar dashboard base del supervisor
    this.supervisorService.getDashboardSupervisor(this.usuarioActual.id).subscribe({
      next: (dashboard) => {
        this.dashboardData = dashboard;
        this.cargarAnalyticsAdicionales();
      },
      error: (error) => {
        console.error('❌ Error al cargar dashboard supervisor:', error);
        this.crearDatosAnalyticsSimulados();
      }
    });
  }

  /**
   * Carga analytics adicionales específicos para el módulo
   */
  cargarAnalyticsAdicionales(): void {
    if (!this.usuarioActual?.id) return;

    // Por ahora usamos datos simulados hasta que el backend esté listo
    setTimeout(() => {
      this.crearDatosAnalyticsSimulados();
    }, 1000);
  }

  /**
   * Procesa y enriquece los datos de analytics
   */
  procesarDatosAnalytics(): void {
    if (this.datosAnalytics && this.dashboardData) {
      // Combinar datos del dashboard con analytics específicos
      this.datosAnalytics.resumenEjecutivo = {
        ...this.datosAnalytics.resumenEjecutivo,
        totalTrabajadores: this.dashboardData.resumenGeneral.totalSubordinados,
        alertasActivas: this.dashboardData.resumenGeneral.alertasActivas,
        trabajadoresEnRiesgo: this.dashboardData.trabajadoresEnRiesgo?.length || 0
      };
    }
  }

  /**
   * Crea datos simulados cuando no hay conexión
   */
  crearDatosAnalyticsSimulados(): void {
    console.log('📊 Creando datos de analytics simulados');
    
    this.datosAnalytics = {
      resumenEjecutivo: {
        roiSalud: 156.7,
        reduccionAusentismo: 23.5,
        mejoraBienestar: 18.2,
        ahorrosMedicos: 85400,
        scoreGeneral: 84.2
      },
      
      comparativasDepartamentos: {
        desarrollo: {
          nombre: 'Desarrollo',
          trabajadores: 3,
          promedioSalud: 78.5,
          alertasPromedio: 2.1,
          ausentismoReduccion: 15.2,
          costoIntervenciones: 12500
        },
        marketing: {
          nombre: 'Marketing',
          trabajadores: 2,
          promedioSalud: 86.3,
          alertasPromedio: 1.2,
          ausentismoReduccion: 28.7,
          costoIntervenciones: 8200
        },
        rrhh: {
          nombre: 'RRHH',
          trabajadores: 2,
          promedioSalud: 82.1,
          alertasPromedio: 1.8,
          ausentismoReduccion: 22.1,
          costoIntervenciones: 9800
        }
      },
      
      tendenciasAusentismo: {
        ultimos30Dias: {
          totalDias: 12,
          reduccionPorcentual: 23.5,
          ahorrosEstimados: 18500,
          principales_causas: ['Estrés laboral', 'Problemas de sueño', 'Fatiga']
        },
        predicciones: {
          proximos30Dias: 8,
          factoresRiesgo: ['Incremento carga laboral', 'Temporada alta'],
          recomendaciones: ['Monitoreo intensivo', 'Programas antiestrés']
        }
      },
      
      efectividadWellness: {
        programasActivos: 4,
        participacion: 78.5,
        mejoraBienestar: 18.2,
        satisfaccion: 4.3,
        programas: [
          {
            nombre: 'Monitoreo Inteligente',
            participantes: 7,
            efectividad: 92.1,
            impacto: 'Alto',
            roi: 245.8
          },
          {
            nombre: 'Alertas Preventivas',
            participantes: 7,
            efectividad: 87.4,
            impacto: 'Alto',
            roi: 189.3
          },
          {
            nombre: 'Recomendaciones IA',
            participantes: 6,
            efectividad: 75.2,
            impacto: 'Medio',
            roi: 156.7
          },
          {
            nombre: 'Seguimiento Biométrico',
            participantes: 7,
            efectividad: 83.6,
            impacto: 'Alto',
            roi: 198.4
          }
        ]
      },
      
      benchmarkInterno: {
        mejorDepartamento: 'Marketing',
        peorDepartamento: 'Desarrollo',
        promedioEmpresa: 82.3,
        rangoVariacion: 8.8,
        oportunidadesMejora: [
          'Implementar programas de gestión de estrés en Desarrollo',
          'Replicar buenas prácticas de Marketing',
          'Aumentar participación en programas wellness'
        ]
      },
      
      predictivosIA: {
        alertasPreventivas: 3,
        riesgosBurnout: 2,
        recomendacionesAutomatic: 8,
        precision: 89.4,
        ahorrosProjected: 45600
      }
    };

    this.isLoading = false;
    console.log('✅ Datos de analytics simulados creados');
  }

  // MÉTODOS DE NAVEGACIÓN Y FILTROS
  cambiarVista(vista: any): void {
    if (['roi', 'departamentos', 'tendencias', 'wellness', 'resumen'].includes(vista)) {
      this.vistaActual = vista;
    }
  }

  cambiarPeriodo(periodo: '7d' | '30d' | '90d' | '1y'): void {
    this.periodoSeleccionado = periodo;
    this.cargarDatosAnalytics();
  }

  cambiarDepartamento(departamento: string): void {
    this.departamentoSeleccionado = departamento;
  }

  // MÉTODOS DE UTILIDAD
  getColorScore(score: number): string {
    if (score >= 90) return '#22c55e';
    if (score >= 80) return '#84cc16';
    if (score >= 70) return '#f59e0b';
    if (score >= 60) return '#f97316';
    return '#ef4444';
  }

  getIconoTendencia(valor: number): string {
    if (valor > 0) return '📈';
    if (valor < 0) return '📉';
    return '➡️';
  }

  formatearMoneda(valor: number): string {
    return new Intl.NumberFormat('es-ES', {
      style: 'currency',
      currency: 'EUR'
    }).format(valor);
  }

  formatearPorcentaje(valor: number): string {
    return `${valor.toFixed(1)}%`;
  }

  exportarReporte(): void {
    console.log('📄 Exportando reporte de analytics empresarial');
    // Implementar exportación
  }

  recargarDatos(): void {
    this.cargarDatosAnalytics();
  }

  // MÉTODOS PARA TEMPLATE
  getCurrentDate(): Date {
    return new Date();
  }

  // GETTERS PARA TEMPLATES
  get resumenEjecutivo(): any {
    return this.datosAnalytics?.resumenEjecutivo || {};
  }

  get comparativasDepartamentos(): any[] {
    const comp = this.datosAnalytics?.comparativasDepartamentos || {};
    return Object.values(comp);
  }

  get tendenciasAusentismo(): any {
    return this.datosAnalytics?.tendenciasAusentismo || {};
  }

  get efectividadWellness(): any {
    return this.datosAnalytics?.efectividadWellness || {};
  }

  get benchmarkInterno(): any {
    return this.datosAnalytics?.benchmarkInterno || {};
  }

  get predictivosIA(): any {
    return this.datosAnalytics?.predictivosIA || {};
  }
} 