import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertaService } from '../../services/alerta.service';
import { UsuarioService } from '../../services/usuario.service';
import { DatoVitalService } from '../../services/dato-vital.service';
import { PasosService } from '../../services/pasos.service';
import { Alerta } from '../../models/alerta.model';
import { Usuario } from '../../models/usuario.model';
import { DatoVital } from '../../models/dato-vital.model';

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
  datosVitalesPasos: DatoVital[] = [];
  loading = false;
  error: string | null = null;
  
  // ✅ PROPIEDADES PARA VISUALIZACIÓN DEL ÚLTIMO MINUTO
  datosUltimoMinuto: any[] = [];
  fechaUltimoMinuto: Date | null = null;
  estadisticasUltimoMinuto: {
    totalPasos: number;
    promedioPasos: number;
    maximoPasos: number;
    minimoPasos: number;
  } | null = null;
  
  // ✅ VERIFICAR SI VIENE DESDE EL PANEL DEL JEFE
  jefeAnterior: Usuario | null = null;

  constructor(
    private router: Router,
    private alertaService: AlertaService,
    private usuarioService: UsuarioService,
    private datoVitalService: DatoVitalService,
    private pasosService: PasosService
  ) { }

  ngOnInit(): void {
    this.usuarioActual = this.usuarioService.getCurrentUser();
    
    // Verificar si viene desde el panel del jefe
    const jefeAnteriorData = localStorage.getItem('jefeAnterior');
    if (jefeAnteriorData) {
      try {
        this.jefeAnterior = JSON.parse(jefeAnteriorData);
        console.log('👔 Se detectó navegación desde panel de jefe:', this.jefeAnterior?.nombreCompleto);
      } catch (error) {
        console.error('❌ Error al parsear datos del jefe anterior:', error);
      }
    }
    
    if (!this.usuarioActual) {
      console.log('❌ Usuario no autenticado, redirigiendo al login');
      this.router.navigate(['/login']);
      return;
    }

    if (this.usuarioActual.rol === 'Jefe' && !this.jefeAnterior) {
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

    // 🚶‍♂️ CARGAR DATOS VITALES DE PASOS
    this.cargarDatosVitalesPasos();
  }

  // ✅ MÉTODO PARA CARGAR DATOS VITALES DE PASOS
  cargarDatosVitalesPasos(): void {
    if (!this.usuarioActual?.id) {
      console.error('❌ No se puede cargar datos de pasos: ID de usuario no encontrado');
      return;
    }

    console.log('🚶‍♂️ Iniciando carga de datos vitales de pasos para usuario ID:', this.usuarioActual.id);
    
    this.datoVitalService.getDatosVitalesByTipo(this.usuarioActual.id, 'Pasos').subscribe({
      next: (datosVitales) => {
        console.log('🚶‍♂️ DATOS VITALES DE PASOS OBTENIDOS:', datosVitales);
        console.log('📊 Cantidad total de registros de pasos:', datosVitales.length);
        
        // Mostrar detalles de cada registro de pasos
        datosVitales.forEach((dato, index) => {
          console.log(`🚶‍♂️ Registro ${index + 1}:`, {
            id: dato.id,
            valor: dato.valor,
            fecha: dato.fechaRegistro,
            tipoDato: dato.tipoDato,
            unidad: dato.unidad
          });
        });

        // Calcular estadísticas básicas
        if (datosVitales.length > 0) {
          const valores = datosVitales.map(d => d.valor);
          const totalPasos = valores.reduce((sum, val) => sum + val, 0);
          const promedioPasos = totalPasos / valores.length;
          const maxPasos = Math.max(...valores);
          const minPasos = Math.min(...valores);

          console.log('📈 ESTADÍSTICAS DE PASOS:', {
            totalRegistros: datosVitales.length,
            totalPasos: totalPasos,
            promedioPasos: Math.round(promedioPasos),
            maxPasos: maxPasos,
            minPasos: minPasos
          });
        }

        this.datosVitalesPasos = datosVitales;

        // 🎯 PROCESAR DATOS DE PASOS DEL ÚLTIMO MINUTO CON DATOS REALES DE LA API
        this.procesarDatosPasosUltimoMinuto(datosVitales as any[]);
      },
      error: (err) => {
        console.error('❌ Error al cargar datos vitales de pasos:', err);
        console.error('❌ Detalles del error:', err.message);
      }
    });
  }

  // ✅ MÉTODO PARA PROCESAR DATOS DE PASOS DEL ÚLTIMO MINUTO CON DATOS REALES
  procesarDatosPasosUltimoMinuto(datosVitales: any[]): void {
    if (!datosVitales || datosVitales.length === 0) {
      console.log('❌ No hay datos de pasos de la API para procesar');
      return;
    }

    console.log('🚀 PROCESANDO DATOS REALES DE PASOS DE LA API...');
    
    try {
      console.log('🔍 Analizando estructura de datos recibidos...');
      console.log('📊 Primer dato de ejemplo:', datosVitales[0]);
      
      // Detectar el formato de los datos
      const tieneFormatoAPI = datosVitales[0]?.fechaRegistro !== undefined;
      const tieneFormatoDirecto = datosVitales[0]?.fecha !== undefined;
      
      console.log('🎯 Formato detectado:', {
        tieneFormatoAPI,
        tieneFormatoDirecto,
        campos: Object.keys(datosVitales[0] || {})
      });
      
      let datosPasosFormateados;
      
      if (tieneFormatoDirecto) {
        // Los datos ya están en formato correcto (tienen campo 'fecha')
        console.log('✅ Datos ya están en formato correcto, validando fechas...');
        
        datosPasosFormateados = datosVitales.map((dato, index) => {
          // Validar que los datos sean válidos
          if (!dato.fecha) {
            console.warn(`⚠️ Dato sin fecha encontrado en registro ${index + 1}:`, dato);
            return null;
          }
          
          // Validar que la fecha sea válida
          const fechaTest = new Date(dato.fecha);
          if (isNaN(fechaTest.getTime())) {
            console.warn(`⚠️ Fecha inválida en registro ${index + 1}:`, dato.fecha, 'Dato completo:', dato);
            return null;
          }
          
          return {
            fecha: dato.fecha,
            valor: dato.valor,
            unidad: dato.unidad || 'pasos'
          };
        }).filter(dato => dato !== null);
        
      } else if (tieneFormatoAPI) {
        // Los datos están en formato API (tienen campo 'fechaRegistro')
        console.log('🔄 Convirtiendo datos del formato API...');
        
        datosPasosFormateados = datosVitales.map((dato, index) => {
          // Validar que los datos sean válidos
          if (!dato.fechaRegistro) {
            console.warn(`⚠️ Dato sin fechaRegistro encontrado en registro ${index + 1}:`, dato);
            return null;
          }
          
          // Validar que la fecha sea válida
          const fechaTest = new Date(dato.fechaRegistro);
          if (isNaN(fechaTest.getTime())) {
            console.warn(`⚠️ Fecha inválida en fechaRegistro del registro ${index + 1}:`, dato.fechaRegistro, 'Dato completo:', dato);
            return null;
          }
          
          return {
            fecha: dato.fechaRegistro,
            valor: dato.valor,
            unidad: dato.unidad || 'pasos'
          };
        }).filter(dato => dato !== null);
        
      } else {
        console.error('❌ Formato de datos no reconocido. Campos disponibles:', Object.keys(datosVitales[0] || {}));
        return;
      }

      console.log('📊 Datos procesados para análisis:', datosPasosFormateados.length, 'registros válidos');
      
      if (datosPasosFormateados.length === 0) {
        console.log('❌ No hay datos válidos para procesar después del filtrado');
        return;
      }
      
      // Usar el servicio de pasos para procesar los datos reales
      const resultados = this.pasosService.procesarDatosPasosUltimoMinuto(datosPasosFormateados);
      
      console.log('✅ Procesamiento completado. Registros del último minuto encontrados:', resultados.length);
      
      if (resultados.length > 0) {
        console.log('🎯 RESUMEN: Se encontraron', resultados.length, 'registros de pasos en el último minuto registrado');
        
        // ✅ GUARDAR DATOS PARA VISUALIZACIÓN
        this.datosUltimoMinuto = resultados;
        this.fechaUltimoMinuto = new Date(resultados[0].fecha);
        
        // Calcular estadísticas
        const valores = resultados.map(r => r.valor);
        const totalPasos = valores.reduce((sum, val) => sum + val, 0);
        const promedioPasos = totalPasos / valores.length;
        const maximoPasos = Math.max(...valores);
        const minimoPasos = Math.min(...valores);
        
        this.estadisticasUltimoMinuto = {
          totalPasos: Math.round(totalPasos * 100) / 100,
          promedioPasos: Math.round(promedioPasos * 100) / 100,
          maximoPasos: maximoPasos,
          minimoPasos: minimoPasos
        };
        
        console.log('📊 Datos guardados para visualización:', {
          registros: this.datosUltimoMinuto.length,
          fecha: this.fechaUltimoMinuto,
          estadisticas: this.estadisticasUltimoMinuto
        });
        
      } else {
        console.log('⚠️ No se encontraron múltiples registros en el último minuto');
        // Limpiar datos de visualización
        this.datosUltimoMinuto = [];
        this.fechaUltimoMinuto = null;
        this.estadisticasUltimoMinuto = null;
      }
    } catch (error) {
      console.error('❌ Error al procesar datos de pasos:', error);
      console.log('🔍 Datos originales que causaron el error:', datosVitales);
      
      // Mostrar información detallada sobre los datos problemáticos
      datosVitales.forEach((dato, index) => {
        try {
          new Date(dato.fechaRegistro).toISOString();
        } catch (dateError) {
          console.error(`❌ Fecha inválida en registro ${index + 1}:`, dato.fechaRegistro);
        }
      });
    }
  }

   // ✅ MÉTODO PARA PROCESAR DATOS DE PASOS DIRECTAMENTE DESDE LA API
   procesarDatosPasosDesdeAPI(): void {
     if (!this.usuarioActual?.id) {
       console.error('❌ No se puede procesar datos: ID de usuario no encontrado');
       return;
     }

     console.log('🔄 Iniciando procesamiento de datos de pasos desde la API...');
     this.pasosService.procesarDatosPasosDesdeAPI(this.usuarioActual.id);
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

  /**
   * Vuelve al panel del jefe si se navegó desde allí
   */
  volverAlPanelJefe(): void {
    if (this.jefeAnterior) {
      console.log('🔙 Volviendo al panel del jefe:', this.jefeAnterior.nombreCompleto);
      
      // Restaurar el usuario jefe
      this.usuarioService.setCurrentUser(this.jefeAnterior);
      
      // Limpiar los datos del localStorage
      localStorage.removeItem('jefeAnterior');
      
      // Navegar de vuelta al dashboard del jefe
      this.router.navigate(['/dashboard-jefe']);
    } else {
      console.log('❌ No hay jefe anterior para volver');
    }
  }

  /**
   * Verifica si hay un jefe anterior para mostrar el botón de volver
   */
  tieneJefeAnterior(): boolean {
    return this.jefeAnterior !== null;
  }

  /**
   * Obtiene los segundos de una fecha
   */
  obtenerSegundos(fecha: string): number {
    return new Date(fecha).getSeconds();
  }

  /**
   * Obtiene los milisegundos de una fecha
   */
  obtenerMilisegundos(fecha: string): number {
    return new Date(fecha).getMilliseconds();
  }
} 