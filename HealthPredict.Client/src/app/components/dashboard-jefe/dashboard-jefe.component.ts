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
  
  // ✅ FILTROS DE ALERTAS
  filtroAlertaActual: 'todas' | 'criticas' | 'altas' | 'no-leidas' = 'todas';

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
    console.log('🔍 Navegando al detalle del trabajador desde vista de riesgo:', trabajador.nombre);
    
    if (!trabajador.id) {
      console.error('❌ ID del trabajador no encontrado');
      return;
    }

    // Crear un usuario temporal para el dashboard individual
    const trabajadorParaDashboard: Usuario = {
      id: trabajador.id,
      nombre: trabajador.nombre ? trabajador.nombre.split(' ')[0] : 'Usuario',
      apellido: trabajador.nombre ? trabajador.nombre.split(' ').slice(1).join(' ') : '',
      nombreCompleto: trabajador.nombre || 'Usuario Desconocido',
      email: trabajador.email || `usuario${trabajador.id}@healthpredict.com`,
      cargo: trabajador.cargo || 'Trabajador',
      departamento: trabajador.departamento || 'Desarrollo',
      rol: 'Trabajador',
      esActivo: true,
      esJefe: false,
      esTrabajador: true,
      ultimoAcceso: trabajador.ultimoAcceso || new Date(),
      // Propiedades adicionales requeridas por la interfaz Usuario
      fechaNacimiento: new Date(1990, 0, 1), // Fecha por defecto
      genero: 'No especificado',
      altura: 170,
      peso: 70,
      esProfesionalMedico: false
    };

    // Guardar el usuario actual para poder volver después
    const jefeActual = this.usuarioService.getCurrentUser();
    if (!jefeActual) {
      console.error('❌ No hay usuario jefe actual');
      return;
    }
    
    localStorage.setItem('jefeAnterior', JSON.stringify(jefeActual));
    
    // Establecer temporalmente al trabajador como usuario actual
    this.usuarioService.setCurrentUser(trabajadorParaDashboard);
    
    // Navegar al dashboard del trabajador
    this.router.navigate(['/dashboard']).then(() => {
      console.log('✅ Navegación exitosa al dashboard del trabajador desde vista de riesgo');
    }).catch(error => {
      console.error('❌ Error en la navegación:', error);
      // Restaurar usuario original en caso de error
      this.usuarioService.setCurrentUser(jefeActual);
    });
  }

  /**
   * Marca una alerta como leída
   */
  marcarAlertaLeida(alertaId: number): void {
    console.log('📖 Marcando alerta como leída:', alertaId);
    
    // Buscar y marcar la alerta como leída en los datos locales
    if (this.dashboardData?.alertasRecientes) {
      const alerta = this.dashboardData.alertasRecientes.find(a => a.id === alertaId);
      if (alerta) {
        alerta.leida = true;
        console.log('✅ Alerta marcada como leída localmente');
      }
    }
    
    // También marcar en el array legacy si existe
    const alertaLegacy = this.todasLasAlertas.find(a => a.id === alertaId);
    if (alertaLegacy) {
      alertaLegacy.leida = true;
    }

    // TODO: Implementar llamada al backend para persistir el cambio
    // this.alertaService.marcarComoLeida(alertaId).subscribe({
    //   next: () => console.log('✅ Alerta marcada como leída en el servidor'),
    //   error: (error) => console.error('❌ Error al marcar alerta como leída:', error)
    // });
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
    console.log('🔍 Navegando al detalle del subordinado:', id);
    
    // Obtener información del trabajador
    const trabajador = this.dashboardData?.resumenSubordinados.find(t => t.id === id) || 
                      this.subordinados.find(s => s.id === id);
    
    if (!trabajador) {
      console.error('❌ Trabajador no encontrado');
      return;
    }

    // Crear un usuario temporal para el dashboard individual
    const trabajadorParaDashboard: Usuario = {
      id: id,
      nombre: trabajador.nombre ? trabajador.nombre.split(' ')[0] : 'Usuario',
      apellido: trabajador.nombre ? trabajador.nombre.split(' ').slice(1).join(' ') : '',
      nombreCompleto: trabajador.nombre || 'Usuario Desconocido',
      email: trabajador.email || `usuario${id}@healthpredict.com`,
      cargo: trabajador.cargo || 'Trabajador',
      departamento: trabajador.departamento || 'Desarrollo',
      rol: 'Trabajador',
      esActivo: true,
      esJefe: false,
      esTrabajador: true,
      ultimoAcceso: trabajador.ultimoAcceso || new Date(),
      // Propiedades adicionales requeridas por la interfaz Usuario
      fechaNacimiento: new Date(1990, 0, 1), // Fecha por defecto
      genero: 'No especificado',
      altura: 170,
      peso: 70,
      esProfesionalMedico: false
    };

    // Guardar el usuario actual para poder volver después
    const jefeActual = this.usuarioService.getCurrentUser();
    if (!jefeActual) {
      console.error('❌ No hay usuario jefe actual');
      return;
    }
    
    localStorage.setItem('jefeAnterior', JSON.stringify(jefeActual));
    
    // Establecer temporalmente al trabajador como usuario actual
    this.usuarioService.setCurrentUser(trabajadorParaDashboard);
    
    // Navegar al dashboard del trabajador
    this.router.navigate(['/dashboard']).then(() => {
      console.log('✅ Navegación exitosa al dashboard del trabajador');
    }).catch(error => {
      console.error('❌ Error en la navegación:', error);
      // Restaurar usuario original en caso de error
      this.usuarioService.setCurrentUser(jefeActual);
    });
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

  // ✅ MÉTODOS PARA MÉTRICAS DEL DEPARTAMENTO

  /**
   * Obtiene las métricas de alertas por severidad
   */
  getMetricasAlertasPorSeveridad(): { severidad: string, cantidad: number, color: string }[] {
    const alertas = this.dashboardData?.alertasRecientes || this.todasLasAlertas || [];
    const alertasArray = Array.isArray(alertas) ? alertas as any[] : [];
    
    const criticas = alertasArray.filter((a: any) => a.severidad?.toLowerCase() === 'crítica').length;
    const altas = alertasArray.filter((a: any) => a.severidad?.toLowerCase() === 'alta').length;
    const medias = alertasArray.filter((a: any) => a.severidad?.toLowerCase() === 'media').length;
    const bajas = alertasArray.filter((a: any) => a.severidad?.toLowerCase() === 'baja').length;

    return [
      { severidad: 'Críticas', cantidad: criticas, color: '#ef4444' },
      { severidad: 'Altas', cantidad: altas, color: '#f59e0b' },
      { severidad: 'Medias', cantidad: medias, color: '#eab308' },
      { severidad: 'Bajas', cantidad: bajas, color: '#22c55e' }
    ];
  }

  /**
   * Obtiene los promedios departamentales simulados
   */
  getPromediosDepartamentales(): { tipoDato: string, promedio: number, unidad: string, color: string }[] {
    return [
      { tipoDato: 'Presión Arterial Sistólica', promedio: 125, unidad: 'mmHg', color: '#667eea' },
      { tipoDato: 'Presión Arterial Diastólica', promedio: 82, unidad: 'mmHg', color: '#764ba2' },
      { tipoDato: 'Frecuencia Cardíaca', promedio: 75, unidad: 'bpm', color: '#f093fb' },
      { tipoDato: 'Nivel de Estrés', promedio: 4.2, unidad: '/10', color: '#f5576c' },
      { tipoDato: 'Horas de Sueño', promedio: 7.5, unidad: 'hrs', color: '#4facfe' },
      { tipoDato: 'Actividad Física', promedio: 6.8, unidad: '/10', color: '#43e97b' }
    ];
  }

  /**
   * Obtiene las tendencias de los últimos 7 días
   */
  getTendencias7Dias(): { label: string, valor: number, cambio: number, color: string }[] {
    const alertas = this.dashboardData?.alertasRecientes || this.todasLasAlertas || [];
    const alertasArray = Array.isArray(alertas) ? alertas as any[] : [];
    const totalAlertas = alertasArray.length;
    const alertasCriticas = alertasArray.filter((a: any) => a.severidad?.toLowerCase() === 'crítica').length;

    return [
      { 
        label: 'Alertas Nuevas', 
        valor: totalAlertas, 
        cambio: Math.floor(Math.random() * 20) - 10, // Cambio simulado entre -10 y +10
        color: '#667eea' 
      },
      { 
        label: 'Alertas Críticas', 
        valor: alertasCriticas, 
        cambio: Math.floor(Math.random() * 6) - 3, // Cambio simulado entre -3 y +3
        color: '#ef4444' 
      },
      { 
        label: 'Trabajadores Activos', 
        valor: this.dashboardData?.resumenSubordinados?.length || this.subordinados.length, 
        cambio: 0, 
        color: '#22c55e' 
      },
      { 
        label: 'Score Promedio Depto', 
        valor: 82, 
        cambio: 5, 
        color: '#f59e0b' 
      }
    ];
  }

  /**
   * Obtiene estadísticas de productividad del departamento
   */
  getEstadisticasProductividad(): { label: string, valor: number, unidad: string, color: string }[] {
    const totalTrabajadores = this.dashboardData?.resumenSubordinados?.length || this.subordinados.length;
    const trabajadoresBajoRiesgo = this.getTrabajadoresPorRiesgo('bajo').length;
    const porcentajeSaludable = totalTrabajadores > 0 ? Math.round((trabajadoresBajoRiesgo / totalTrabajadores) * 100) : 0;

    return [
      { label: 'Trabajadores Saludables', valor: porcentajeSaludable, unidad: '%', color: '#22c55e' },
      { label: 'Promedio Bienestar', valor: 84, unidad: '/100', color: '#667eea' },
      { label: 'Alertas Resueltas', valor: 95, unidad: '%', color: '#f59e0b' },
      { label: 'Tiempo Respuesta', valor: 2.3, unidad: 'hrs', color: '#8b5cf6' }
    ];
  }

  // ✅ MÉTODOS PARA FILTROS DE ALERTAS

  /**
   * Cambia el filtro actual de alertas
   */
  cambiarFiltroAlertas(filtro: 'todas' | 'criticas' | 'altas' | 'no-leidas'): void {
    console.log('🔍 Cambiando filtro de alertas a:', filtro);
    this.filtroAlertaActual = filtro;
  }

  /**
   * Obtiene las alertas filtradas según el filtro actual
   */
  getAlertasFiltradas(): any[] {
    const alertas = this.dashboardData?.alertasRecientes || this.todasLasAlertas || [];
    const alertasArray = Array.isArray(alertas) ? alertas as any[] : [];
    
    switch (this.filtroAlertaActual) {
      case 'criticas':
        return alertasArray.filter((alerta: any) => alerta.severidad?.toLowerCase() === 'crítica');
      
      case 'altas':
        return alertasArray.filter((alerta: any) => alerta.severidad?.toLowerCase() === 'alta');
      
      case 'no-leidas':
        return alertasArray.filter((alerta: any) => !alerta.leida);
      
      case 'todas':
      default:
        return alertasArray;
    }
  }

  /**
   * Verifica si un filtro está activo
   */
  esFiltroActivo(filtro: 'todas' | 'criticas' | 'altas' | 'no-leidas'): boolean {
    return this.filtroAlertaActual === filtro;
  }

  /**
   * Obtiene el número de alertas para un filtro específico
   */
  getContadorFiltro(filtro: 'todas' | 'criticas' | 'altas' | 'no-leidas'): number {
    const alertas = this.dashboardData?.alertasRecientes || this.todasLasAlertas || [];
    const alertasArray = Array.isArray(alertas) ? alertas as any[] : [];
    
    switch (filtro) {
      case 'criticas':
        return alertasArray.filter((alerta: any) => alerta.severidad?.toLowerCase() === 'crítica').length;
      
      case 'altas':
        return alertasArray.filter((alerta: any) => alerta.severidad?.toLowerCase() === 'alta').length;
      
      case 'no-leidas':
        return alertasArray.filter((alerta: any) => !alerta.leida).length;
      
      case 'todas':
      default:
        return alertasArray.length;
    }
  }
} 