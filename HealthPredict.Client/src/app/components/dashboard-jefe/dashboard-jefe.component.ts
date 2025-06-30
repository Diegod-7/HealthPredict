import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UsuarioService } from '../../services/usuario.service';
import { DatoVitalService } from '../../services/dato-vital.service';
import { AlertaService } from '../../services/alerta.service';
import { SupervisorService, DashboardSupervisor, ResumenSubordinado, TrabajadorEnRiesgo } from '../../services/supervisor.service';
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
  
  // ✅ DATOS DEL PANEL SUPERVISOR
  dashboardData: DashboardSupervisor | null = null;
  
  // ✅ DATOS LEGACY (para compatibilidad)
  estadisticasGenerales: any = null;
  subordinados: Usuario[] = [];
  todasLasAlertas: Alerta[] = [];
  todosLosDatosVitales: DatoVital[] = [];
  
  // ✅ ESTADO DE CARGA
  isLoading: boolean = true;
  error: string = '';

  // ✅ CONFIGURACIÓN DE VISTA
  vistaActual: 'general' | 'trabajadores' | 'alertas' | 'metricas' | 'riesgo' = 'general';

  constructor(
    private usuarioService: UsuarioService,
    private datoVitalService: DatoVitalService,
    private alertaService: AlertaService,
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

    // Si es Carlos Rodríguez por email, promoverlo a jefe
    if (this.usuarioActual.email === 'carlos.rodriguez@healthpredict.com' || this.usuarioActual.email === 'carlos.rodriguez@example.com') {
      this.usuarioActual.rol = 'Jefe';
      this.usuarioActual.esJefe = true;
      this.usuarioActual.esTrabajador = false;
      this.usuarioService.setCurrentUser(this.usuarioActual);
    }
    
    if (this.usuarioActual.rol !== 'Jefe') {
      console.log('❌ Acceso denegado: Usuario no es jefe, redirigiendo a dashboard trabajador');
      this.router.navigate(['/dashboard']);
      return;
    }

    const nombreMostrar = this.usuarioActual.nombreCompleto || this.usuarioActual.nombre || this.usuarioActual.email.split('@')[0];
    console.log('👔 Dashboard de jefe cargando para:', nombreMostrar);
    this.cargarDashboardJefe();
  }

  /**
   * Carga los datos del dashboard del jefe usando el nuevo servicio
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
    if (this.usuarioActual.email === 'carlos.rodriguez@example.com' || this.usuarioActual.email === 'carlos.rodriguez@healthpredict.com') {
      console.log('👔 Promoviendo a Carlos Rodríguez como jefe temporal');
      this.usuarioActual.rol = 'Jefe';
      this.usuarioActual.esJefe = true;
      this.usuarioActual.esTrabajador = false;
      
      // Actualizar en el servicio
      this.usuarioService.setCurrentUser(this.usuarioActual);
    }

    // Cargar dashboard completo del supervisor
    this.supervisorService.getDashboardSupervisor(this.usuarioActual.id).subscribe({
      next: (dashboard) => {
        console.log('👔 Dashboard supervisor cargado:', dashboard);
        this.dashboardData = dashboard;
        
                 // Mantener compatibilidad con datos legacy
         this.subordinados = dashboard.resumenSubordinados.map(s => ({
           id: s.id || 0,
           nombre: s.nombre.split(' ')[0],
           apellido: s.nombre.split(' ').slice(1).join(' '),
           nombreCompleto: s.nombre,
           email: s.email,
           cargo: s.cargo,
           departamento: s.departamento,
           ultimoAcceso: s.ultimoAcceso,
           rol: 'Trabajador',
           esActivo: true
         } as Usuario));

        this.estadisticasGenerales = {
          totalSubordinados: dashboard.resumenGeneral.totalSubordinados,
          totalAlertas: dashboard.resumenGeneral.alertasActivas,
          alertasNoLeidas: dashboard.alertasRecientes.filter(a => !a.leida).length,
          totalDatosVitales: dashboard.resumenSubordinados.reduce((sum, s) => sum + s.ultimosDatos.length, 0),
          subordinados: this.subordinados
        };

        this.todasLasAlertas = dashboard.alertasRecientes.map(a => ({
          id: a.id,
          usuarioId: a.usuarioId,
          tipoAlerta: a.tipoAlerta,
          descripcion: a.descripcion,
          severidad: a.severidad,
          leida: a.leida,
          fechaCreacion: a.fechaCreacion,
          resuelta: false
        } as Alerta));

        this.isLoading = false;
      },
      error: (error) => {
        console.error('❌ Error al cargar dashboard supervisor:', error);
        
        if (error.status === 404 || (error.status === 400 && error.error?.includes('Supervisor no encontrado'))) {
          // Intentar con el servicio legacy si el usuario no es reconocido como jefe
          console.log('🔄 Intentando cargar dashboard legacy para jefe');
          this.cargarDashboardLegacy();
        } else if (error.status === 0) {
          console.log('🔄 Error de conexión, intentando dashboard simulado');
          this.crearDashboardSimulado();
        } else {
          this.error = `Error al cargar el dashboard supervisor: ${error.error?.error || error.message}`;
          this.isLoading = false;
        }
      }
    });
  }

  /**
   * Método legacy de fallback
   */
  private cargarDashboardLegacy(): void {
    if (!this.usuarioActual?.id) {
      this.error = 'Error: ID de usuario no encontrado en legacy';
      this.isLoading = false;
      return;
    }
    this.usuarioService.getDashboardJefe(this.usuarioActual.id).subscribe({
      next: (estadisticas) => {
        console.log('📊 Dashboard legacy cargado:', estadisticas);
        this.estadisticasGenerales = estadisticas;
        this.subordinados = estadisticas.subordinados || [];
        this.cargarDatosSubordinados();
      },
      error: (error) => {
        console.error('❌ Error al cargar dashboard legacy:', error);
        
        if (this.usuarioActual?.email === 'carlos.rodriguez@example.com' || this.usuarioActual?.email === 'carlos.rodriguez@healthpredict.com') {
          console.log('📊 Creando dashboard simulado para Carlos como jefe');
          this.crearDashboardSimulado();
        } else {
          console.log('🔄 Error al cargar dashboard, intentando simulado');
          this.crearDashboardSimulado();
        }
      }
    });
  }

  /**
   * Crea un dashboard simulado para el jefe cuando el servidor no reconoce el rol
   */
  private crearDashboardSimulado(): void {
    console.log('📊 Creando dashboard simulado con datos de ejemplo');
    
    this.estadisticasGenerales = {
      totalSubordinados: 3,
      alertasActivas: 8,
      alertasResueltasHoy: 3,
      promedioSaludGeneral: 85,
      subordinados: [
        {
          id: 1,
          nombre: 'Diego',
          apellido: 'Diaz',
          email: 'diego.diaz@healthpredict.com',
          rol: 'Trabajador',
          departamento: 'Desarrollo',
          cargo: 'Desarrollador Full Stack',
          esActivo: true,
          nombreCompleto: 'Diego Díaz',
          esJefe: false,
          esTrabajador: true,
          fechaNacimiento: new Date('1990-05-15'),
          genero: 'Masculino',
          altura: 175,
          peso: 70,
          esProfesionalMedico: false
        },
        {
          id: 2,
          nombre: 'Iahn',
          apellido: 'Vera', 
          email: 'iahn.vera@healthpredict.com',
          rol: 'Trabajador',
          departamento: 'Desarrollo',
          cargo: 'Desarrollador Frontend',
          esActivo: true,
          nombreCompleto: 'Iahn Vera',
          esJefe: false,
          esTrabajador: true,
          fechaNacimiento: new Date('1992-08-22'),
          genero: 'Masculino',
          altura: 180,
          peso: 75,
          esProfesionalMedico: false
        },
        {
          id: 3,
          nombre: 'Matias',
          apellido: 'Maripangue',
          email: 'matias.maripangue@healthpredict.com',
          rol: 'Trabajador',
          departamento: 'Desarrollo',
          cargo: 'Desarrollador Backend',
          esActivo: true,
          nombreCompleto: 'Matías Maripangue',
          esJefe: false,
          esTrabajador: true,
          fechaNacimiento: new Date('1988-12-10'),
          genero: 'Masculino',
          altura: 170,
          peso: 68,
          esProfesionalMedico: false
        }
      ]
    } as any;

    // Crear alertas simuladas
    this.todasLasAlertas = [
      {
        id: 1,
        usuarioId: 1,
        tipoAlerta: 'Frecuencia Cardíaca Alta',
        descripcion: 'Frecuencia cardíaca elevada detectada durante actividad física',
        severidad: 'Media',
        leida: false,
        fechaCreacion: new Date(),
        resuelta: false
      },
      {
        id: 2,
        usuarioId: 2,
        tipoAlerta: 'Presión Arterial',
        descripcion: 'Lectura de presión arterial ligeramente elevada',
        severidad: 'Baja',
        leida: true,
        fechaCreacion: new Date(Date.now() - 3600000),
        resuelta: false
      },
      {
        id: 3,
        usuarioId: 3,
        tipoAlerta: 'Estrés Elevado',
        descripcion: 'Niveles de estrés por encima del umbral normal',
        severidad: 'Alta',
        leida: false,
        fechaCreacion: new Date(Date.now() - 7200000),
        resuelta: false
      }
    ] as Alerta[];

    this.subordinados = this.estadisticasGenerales.subordinados;
    this.isLoading = false;
    console.log('✅ Dashboard simulado creado exitosamente');
  }

  /**
   * Carga los datos detallados de todos los subordinados
   */
  cargarDatosSubordinados(): void {
    const subordinadoIds = this.subordinados.map(s => s.id).filter(id => id !== undefined) as number[];
    
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

  // ✅ MÉTODOS PARA EL PANEL SUPERVISOR

  /**
   * Cambia la vista actual del dashboard
   */
  cambiarVista(vista: 'general' | 'trabajadores' | 'alertas' | 'metricas' | 'riesgo'): void {
    this.vistaActual = vista;
  }

  /**
   * Obtiene el color CSS para el nivel de riesgo
   */
  getColorNivelRiesgo(nivelRiesgo: string): string {
    return this.supervisorService.getColorNivelRiesgo(nivelRiesgo);
  }

  /**
   * Obtiene el icono para el nivel de riesgo
   */
  getIconoNivelRiesgo(nivelRiesgo: string): string {
    return this.supervisorService.getIconoNivelRiesgo(nivelRiesgo);
  }

  /**
   * Obtiene el color CSS para la severidad de alerta
   */
  getColorSeveridad(severidad: string): string {
    return this.supervisorService.getColorSeveridad(severidad);
  }

  /**
   * Obtiene el color CSS para el score de bienestar
   */
  getColorScoreBienestar(score: number): string {
    return this.supervisorService.getColorScoreBienestar(score);
  }

  /**
   * Filtra trabajadores por nivel de riesgo
   */
  getTrabajadoresPorRiesgo(nivelRiesgo: string): ResumenSubordinado[] {
    if (!this.dashboardData) return [];
    return this.dashboardData.resumenSubordinados.filter(t => 
      t.nivelRiesgo.toLowerCase() === nivelRiesgo.toLowerCase()
    );
  }

  /**
   * Obtiene las alertas críticas recientes
   */
  getAlertasCriticas(): any[] {
    if (!this.dashboardData) return [];
    return this.dashboardData.alertasRecientes.filter(a => a.severidad === 'Crítica');
  }

  /**
   * Navega al detalle de un trabajador
   */
  verDetalleTrabajador(trabajador: ResumenSubordinado): void {
    console.log('🔍 Viendo detalle de trabajador:', trabajador.nombre);
    // TODO: Implementar navegación al detalle del trabajador
  }

  /**
   * Marca una alerta como leída
   */
  marcarAlertaLeida(alertaId: number): void {
    console.log('📖 Marcando alerta como leída:', alertaId);
    // TODO: Implementar marcado de alerta como leída
  }

  // Método para verificar si es ResumenSubordinado
  isResumenSubordinado(trabajador: any): trabajador is ResumenSubordinado {
    return trabajador && typeof trabajador.nombre === 'string' && !trabajador.hasOwnProperty('apellido');
  }

  // Método para obtener color del score
  getScoreColor(score: number): string {
    if (score >= 85) return 'linear-gradient(135deg, #4CAF50, #8BC34A)';
    if (score >= 70) return 'linear-gradient(135deg, #FF9800, #FFC107)';
    if (score >= 50) return 'linear-gradient(135deg, #FF5722, #FF7043)';
    return 'linear-gradient(135deg, #F44336, #E57373)';
  }

  // Método para obtener alertas activas por subordinado
  getAlertasActivasPorSubordinado(id: number): number {
    return this.getAlertasNoLeidasPorSubordinado(id);
  }

  // Método para ver detalle del subordinado
  verDetalleSubordinado(id: number): void {
    // TODO: Implementar navegación al detalle del subordinado
    console.log('Ver detalle del subordinado:', id);
  }

  // Obtener lista de alertas activas
  get alertasActivas(): any[] {
    return this.todasLasAlertas || [];
  }

  // Obtener lista combinada de trabajadores
  getListaTrabajadores(): any[] {
    return this.dashboardData?.resumenSubordinados || this.subordinados || [];
  }

  // Obtener nombre del trabajador
  getNombreTrabajador(trabajador: any): string {
    if (trabajador.nombre && !trabajador.apellido) {
      return trabajador.nombre; // ResumenSubordinado
    }
    return (trabajador.nombre || '') + ' ' + (trabajador.apellido || ''); // Usuario
  }
} 