import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UsuarioService } from '../../services/usuario.service';
import { DatoVitalService } from '../../services/dato-vital.service';
import { AlertaService } from '../../services/alerta.service';
import { Usuario } from '../../models/usuario.model';
import { DatoVital } from '../../models/dato-vital.model';
import { Alerta } from '../../models/alerta.model';

@Component({
  selector: 'app-dashboard-jefe',
  templateUrl: './dashboard-jefe.component.html',
  styleUrls: ['./dashboard-jefe.component.scss']
})
export class DashboardJefeComponent implements OnInit {

  // ✅ USUARIO ACTUAL (JEFE)
  usuarioActual: Usuario | null = null;
  
  // ✅ DATOS DEL DASHBOARD GENERAL
  estadisticasGenerales: any = null;
  subordinados: Usuario[] = [];
  
  // ✅ DATOS CONSOLIDADOS
  todasLasAlertas: Alerta[] = [];
  todosLosDatosVitales: DatoVital[] = [];
  
  // ✅ ESTADO DE CARGA
  isLoading: boolean = true;
  error: string = '';

  constructor(
    private usuarioService: UsuarioService,
    private datoVitalService: DatoVitalService,
    private alertaService: AlertaService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.usuarioActual = this.usuarioService.getCurrentUser();
    
    if (!this.usuarioActual || this.usuarioActual.rol !== 'Jefe') {
      console.log('❌ Acceso denegado: Usuario no es jefe');
      this.router.navigate(['/login']);
      return;
    }

    console.log('👔 Dashboard de jefe cargando para:', this.usuarioActual.nombreCompleto);
    this.cargarDashboardJefe();
  }

  /**
   * Carga los datos del dashboard del jefe
   */
  cargarDashboardJefe(): void {
    this.isLoading = true;
    this.error = '';

    if (!this.usuarioActual?.id) {
      this.error = 'Error: ID de usuario no encontrado';
      this.isLoading = false;
      return;
    }

    // Verificar si es Carlos Rodríguez y hacerlo jefe temporalmente
    if (this.usuarioActual.email === 'carlos.rodriguez@example.com') {
      console.log('👔 Promoviendo a Carlos Rodríguez como jefe temporal');
      this.usuarioActual.rol = 'Jefe';
      this.usuarioActual.esJefe = true;
      this.usuarioActual.esTrabajador = false;
      
      // Actualizar en el servicio
      this.usuarioService.setCurrentUser(this.usuarioActual);
    }

    // Cargar estadísticas generales del jefe
    this.usuarioService.getDashboardJefe(this.usuarioActual.id).subscribe({
      next: (estadisticas) => {
        console.log('📊 Estadísticas del jefe cargadas:', estadisticas);
        this.estadisticasGenerales = estadisticas;
        this.subordinados = estadisticas.subordinados || [];
        
        // Cargar datos detallados de cada subordinado
        this.cargarDatosSubordinados();
      },
      error: (error) => {
        console.error('❌ Error al cargar estadísticas del jefe:', error);
        
        if (error.status === 400 && error.error === 'El usuario especificado no es un jefe') {
          // Si es Carlos Rodríguez, crear datos simulados para el dashboard del jefe
          if (this.usuarioActual?.email === 'carlos.rodriguez@example.com') {
            console.log('📊 Creando dashboard simulado para Carlos como jefe');
            this.crearDashboardSimulado();
          } else {
            // El usuario actual no es un jefe, redirigir al dashboard de trabajador
            console.log('🔄 Usuario no es jefe, redirigiendo al dashboard de trabajador');
            this.router.navigate(['/dashboard']);
          }
        } else {
          this.error = `Error al cargar el dashboard del jefe: ${error.error || error.message}`;
        }
        
        this.isLoading = false;
      }
    });
  }

  /**
   * Crea un dashboard simulado para el jefe cuando el servidor no reconoce el rol
   */
  private crearDashboardSimulado(): void {
    this.estadisticasGenerales = {
      totalSubordinados: 3,
      alertasActivas: 5,
      alertasResueltasHoy: 2,
      promedioSaludGeneral: 87,
      subordinados: [
        {
          id: 1,
          nombre: 'Diego',
          apellido: 'Diaz',
          email: 'juan.perez@example.com',
          rol: 'Trabajador',
          departamento: 'Desarrollo',
          cargo: 'Desarrollador Full Stack',
          esActivo: true,
          nombreCompleto: 'Diego Díaz',
          esJefe: false,
          esTrabajador: true
        },
        {
          id: 2,
          nombre: 'Iahn',
          apellido: 'Vera', 
          email: 'maria.gonzalez@example.com',
          rol: 'Trabajador',
          departamento: 'Desarrollo',
          cargo: 'Desarrollador Frontend',
          esActivo: true,
          nombreCompleto: 'Iahn Vera',
          esJefe: false,
          esTrabajador: true
        },
        {
          id: 999,
          nombre: 'Matias',
          apellido: 'Maripangue',
          email: 'matias.maripangue@healthpredict.com',
          rol: 'Trabajador',
          departamento: 'Desarrollo',
          cargo: 'Desarrollador Backend',
          esActivo: true,
          nombreCompleto: 'Matías Maripangue',
          esJefe: false,
          esTrabajador: true
        }
      ]
    } as any;

    this.subordinados = this.estadisticasGenerales.subordinados;
    this.cargarDatosSubordinados();
  }

  /**
   * Carga los datos detallados de todos los subordinados
   */
  cargarDatosSubordinados(): void {
    const subordinadoIds = this.subordinados.map(s => s.id!);
    
    if (subordinadoIds.length === 0) {
      this.isLoading = false;
      return;
    }

    let datosCompletados = 0;
    const totalOperaciones = subordinadoIds.length * 2; // Alertas + Datos Vitales por cada subordinado

    subordinadoIds.forEach(subordinadoId => {
      // Cargar alertas del subordinado
      this.alertaService.getAlertasByUsuario(subordinadoId).subscribe({
        next: (alertas) => {
          this.todasLasAlertas.push(...alertas);
          datosCompletados++;
          if (datosCompletados === totalOperaciones) {
            this.isLoading = false;
          }
        },
        error: (error) => {
          console.error(`❌ Error al cargar alertas del usuario ${subordinadoId}:`, error);
          datosCompletados++;
          if (datosCompletados === totalOperaciones) {
            this.isLoading = false;
          }
        }
      });

      // Cargar datos vitales del subordinado
      this.datoVitalService.getDatosVitalesByUsuario(subordinadoId).subscribe({
        next: (datosVitales) => {
          this.todosLosDatosVitales.push(...datosVitales);
          datosCompletados++;
          if (datosCompletados === totalOperaciones) {
            this.isLoading = false;
          }
        },
        error: (error) => {
          console.error(`❌ Error al cargar datos vitales del usuario ${subordinadoId}:`, error);
          datosCompletados++;
          if (datosCompletados === totalOperaciones) {
            this.isLoading = false;
          }
        }
      });
    });
  }

  /**
   * Navega al perfil detallado de un subordinado
   */
  verPerfilSubordinado(subordinado: Usuario): void {
    console.log('🔍 Viendo perfil de:', subordinado.nombreCompleto);
    // TODO: Implementar navegación a perfil detallado
    // this.router.navigate(['/perfil-subordinado', subordinado.id]);
  }

  /**
   * Obtiene las alertas de alta prioridad
   */
  getAlertasAltaPrioridad(): Alerta[] {
    return this.todasLasAlertas.filter(alerta => alerta.severidad === 'Alta');
  }

  /**
   * Obtiene las alertas no leídas
   */
  getAlertasNoLeidas(): Alerta[] {
    return this.todasLasAlertas.filter(alerta => !alerta.leida);
  }

  /**
   * Obtiene los datos vitales más recientes
   */
  getDatosVitalesRecientes(): DatoVital[] {
    return this.todosLosDatosVitales
      .sort((a, b) => {
        const fechaA = a.fechaRegistro ? new Date(a.fechaRegistro).getTime() : 0;
        const fechaB = b.fechaRegistro ? new Date(b.fechaRegistro).getTime() : 0;
        return fechaB - fechaA;
      })
      .slice(0, 10);
  }

  /**
   * Cierra sesión
   */
  logout(): void {
    console.log('🚪 Cerrando sesión del jefe');
    this.usuarioService.logout();
    this.router.navigate(['/login']);
  }

  /**
   * Recarga los datos del dashboard
   */
  recargarDatos(): void {
    console.log('🔄 Recargando datos del dashboard');
    this.todasLasAlertas = [];
    this.todosLosDatosVitales = [];
    this.cargarDashboardJefe();
  }

  // ✅ MÉTODOS AUXILIARES PARA EL TEMPLATE

  /**
   * Obtiene el número de alertas no leídas para un subordinado específico
   */
  getAlertasNoLeidasPorSubordinado(subordinadoId: number): number {
    return this.getAlertasNoLeidas().filter(a => a.usuarioId === subordinadoId).length;
  }

  /**
   * Obtiene el número de datos vitales para un subordinado específico
   */
  getDatosVitalesPorSubordinado(subordinadoId: number): number {
    return this.todosLosDatosVitales.filter(d => d.usuarioId === subordinadoId).length;
  }

  /**
   * Busca un subordinado por ID
   */
  getSubordinadoPorId(usuarioId: number): Usuario | undefined {
    return this.subordinados.find(s => s.id === usuarioId);
  }

  /**
   * Obtiene el nombre completo de un subordinado por ID
   */
  getNombreSubordinado(usuarioId: number): string {
    const subordinado = this.getSubordinadoPorId(usuarioId);
    return subordinado?.nombreCompleto || 'Usuario desconocido';
  }
} 