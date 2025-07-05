import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface PasosHoy {
  fecha: string;
  totalPasos: number;
  registros: number;
  ultimaActualizacion: string;
  datosGrafico: PasosPorHora[];
}

export interface PasosPorHora {
  hora: number;
  horaTexto: string;
  pasos: number;
  registros: number;
}

export interface PasosSemana {
  fechaInicio: string;
  fechaFin: string;
  totalPasos: number;
  promedioDiario: number;
  diasConDatos: number;
  datosGrafico: PasosPorDia[];
}

export interface PasosPorDia {
  fecha: string;
  fechaTexto: string;
  diaSemana: string;
  pasos: number;
  registros: number;
  esHoy: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class PasosService {
  private apiUrl = environment.apiUrl || 'https://healthpredict-l1hu.onrender.com/api';

  constructor(private http: HttpClient) { }

  /**
   * Obtener resumen de pasos del día actual
   */
  getPasosHoy(usuarioId: number): Observable<PasosHoy> {
    return this.http.get<PasosHoy>(`${this.apiUrl}/DatosVitales/pasos-hoy/${usuarioId}`);
  }

  /**
   * Obtener resumen de pasos de los últimos 7 días
   */
  getPasosSemana(usuarioId: number): Observable<PasosSemana> {
    return this.http.get<PasosSemana>(`${this.apiUrl}/DatosVitales/pasos-semana/${usuarioId}`);
  }

  /**
   * Procesar datos de pasos para encontrar y mostrar todos los registros
   * del mismo día, hora y minuto que el último registro
   */
  procesarDatosPasosUltimoMinuto(datosPasos: any[]): any[] {
    if (!datosPasos || datosPasos.length === 0) {
      console.log('❌ No hay datos de pasos para procesar');
      return [];
    }

    console.log('🔍 Validando fechas en los datos...');
    
    // Filtrar datos con fechas válidas
    const datosConFechasValidas = datosPasos.filter((dato, index) => {
      const fecha = new Date(dato.fecha);
      const esValida = !isNaN(fecha.getTime());
      
      if (!esValida) {
        console.warn(`⚠️ Fecha inválida encontrada en registro ${index + 1}:`, dato.fecha);
      }
      
      return esValida;
    });

    if (datosConFechasValidas.length === 0) {
      console.log('❌ No hay datos con fechas válidas para procesar');
      return [];
    }

    console.log(`✅ Datos válidos: ${datosConFechasValidas.length} de ${datosPasos.length} registros`);

    // Ordenar por fecha (más reciente primero)
    const datosOrdenados = datosConFechasValidas.sort((a, b) => {
      const fechaA = new Date(a.fecha);
      const fechaB = new Date(b.fecha);
      return fechaB.getTime() - fechaA.getTime();
    });

    // Obtener la fecha más reciente
    const ultimaFecha = new Date(datosOrdenados[0].fecha);
    
    // Extraer año, mes, día, hora y minuto de la fecha más reciente
    const ultimoAno = ultimaFecha.getFullYear();
    const ultimoMes = ultimaFecha.getMonth();
    const ultimoDia = ultimaFecha.getDate();
    const ultimaHora = ultimaFecha.getHours();
    const ultimoMinuto = ultimaFecha.getMinutes();

    console.log('🕐 Última fecha encontrada:', ultimaFecha.toISOString());
    console.log('📅 Buscando todos los registros del mismo día, hora y minuto:', {
      año: ultimoAno,
      mes: ultimoMes + 1, // +1 porque getMonth() devuelve 0-11
      día: ultimoDia,
      hora: ultimaHora,
      minuto: ultimoMinuto
    });

    // Filtrar todos los registros que tengan la misma fecha, hora y minuto
    const registrosMismoMinuto = datosConFechasValidas.filter(registro => {
      const fechaRegistro = new Date(registro.fecha);
      // Validar que la fecha sea válida antes de comparar
      if (isNaN(fechaRegistro.getTime())) {
        console.warn('⚠️ Fecha inválida encontrada durante filtrado:', registro.fecha);
        return false;
      }
      
      return fechaRegistro.getFullYear() === ultimoAno &&
             fechaRegistro.getMonth() === ultimoMes &&
             fechaRegistro.getDate() === ultimoDia &&
             fechaRegistro.getHours() === ultimaHora &&
             fechaRegistro.getMinutes() === ultimoMinuto;
    });

    console.log('🚶‍♂️ REGISTROS DEL ÚLTIMO MINUTO ENCONTRADOS:');
    console.log('📊 Total de registros:', registrosMismoMinuto.length);
    console.log('📅 Fecha/Hora/Minuto:', `${ultimoDia}/${ultimoMes + 1}/${ultimoAno} ${ultimaHora}:${ultimoMinuto}`);
    
    // Mostrar cada registro individualmente
    registrosMismoMinuto.forEach((registro, index) => {
      console.log(`🚶‍♂️ Registro ${index + 1}:`, {
        fecha: registro.fecha,
        valor: registro.valor,
        unidad: registro.unidad,
        fechaCompleta: new Date(registro.fecha).toISOString()
      });
    });

    // Estadísticas del último minuto
    if (registrosMismoMinuto.length > 0) {
      const valores = registrosMismoMinuto.map(r => r.valor);
      const totalPasos = valores.reduce((sum, val) => sum + val, 0);
      const promedio = totalPasos / valores.length;
      const maximo = Math.max(...valores);
      const minimo = Math.min(...valores);

      console.log('📈 ESTADÍSTICAS DEL ÚLTIMO MINUTO:', {
        totalRegistros: registrosMismoMinuto.length,
        totalPasos: totalPasos,
        promedioPasos: Math.round(promedio * 100) / 100,
        maximoPasos: maximo,
        minimoPasos: minimo
      });
    }

    // Retornar los datos para uso posterior si es necesario
    return registrosMismoMinuto;
  }

  /**
   * Método para procesar directamente datos de pasos desde la API
   * Útil cuando recibes datos en el formato de la API
   */
  procesarDatosPasosDesdeAPI(usuarioId: number): void {
    console.log('🔄 Obteniendo datos de pasos desde la API para procesamiento...');
    
    // Usar el endpoint de gráficos para obtener datos de pasos
    this.http.get<any[]>(`${environment.apiUrl}/Graficos/DatosVitales/${usuarioId}/Pasos`).subscribe({
      next: (datosVitales) => {
        console.log('📡 Datos de pasos obtenidos de la API:', datosVitales.length, 'registros');
        
        if (datosVitales.length === 0) {
          console.log('⚠️ No se encontraron datos de pasos en la API');
          return;
        }

        // Convertir al formato esperado
        const datosPasosFormateados = datosVitales.map(dato => ({
          fecha: dato.fechaRegistro,
          valor: dato.valor,
          unidad: dato.unidad || 'pasos'
        }));

        // Procesar los datos
        this.procesarDatosPasosUltimoMinuto(datosPasosFormateados);
      },
      error: (error) => {
        console.error('❌ Error al obtener datos de pasos de la API:', error);
      }
    });
  }
} 