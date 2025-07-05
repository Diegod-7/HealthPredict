/**
 * Script para procesar datos de pasos y encontrar todos los registros
 * del mismo día, hora y minuto que el último registro
 * 
 * Uso: Copia los datos de la API y ejecuta procesarDatosPasos(datos)
 */

interface RegistroPasos {
  fecha: string;
  valor: number;
  unidad: string;
}

/**
 * Procesa los datos de pasos para encontrar todos los registros
 * del mismo día, hora y minuto que el último registro
 */
function procesarDatosPasosUltimoMinuto(datosPasos: RegistroPasos[]): RegistroPasos[] {
  if (!datosPasos || datosPasos.length === 0) {
    console.log('❌ No hay datos de pasos para procesar');
    return [];
  }

  console.log('🚀 INICIANDO PROCESAMIENTO DE DATOS DE PASOS');
  console.log('📊 Total de registros recibidos:', datosPasos.length);

  // Ordenar por fecha (más reciente primero)
  const datosOrdenados = datosPasos.sort((a, b) => 
    new Date(b.fecha).getTime() - new Date(a.fecha).getTime()
  );

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
    minuto: ultimoMinuto,
    fechaFormateada: `${ultimoDia}/${ultimoMes + 1}/${ultimoAno} ${ultimaHora.toString().padStart(2, '0')}:${ultimoMinuto.toString().padStart(2, '0')}`
  });

  // Filtrar todos los registros que tengan la misma fecha, hora y minuto
  const registrosMismoMinuto = datosPasos.filter(registro => {
    const fechaRegistro = new Date(registro.fecha);
    return fechaRegistro.getFullYear() === ultimoAno &&
           fechaRegistro.getMonth() === ultimoMes &&
           fechaRegistro.getDate() === ultimoDia &&
           fechaRegistro.getHours() === ultimaHora &&
           fechaRegistro.getMinutes() === ultimoMinuto;
  });

  console.log('🚶‍♂️ REGISTROS DEL ÚLTIMO MINUTO ENCONTRADOS:');
  console.log('📊 Total de registros:', registrosMismoMinuto.length);
  console.log('📅 Fecha/Hora/Minuto:', `${ultimoDia}/${ultimoMes + 1}/${ultimoAno} ${ultimaHora.toString().padStart(2, '0')}:${ultimoMinuto.toString().padStart(2, '0')}`);
  
  // Mostrar cada registro individualmente
  registrosMismoMinuto.forEach((registro, index) => {
    const fechaCompleta = new Date(registro.fecha);
    console.log(`🚶‍♂️ Registro ${index + 1}:`, {
      fecha: registro.fecha,
      valor: registro.valor,
      unidad: registro.unidad,
      fechaCompleta: fechaCompleta.toISOString(),
      horaLocal: fechaCompleta.toLocaleString('es-ES'),
      segundos: fechaCompleta.getSeconds(),
      milisegundos: fechaCompleta.getMilliseconds()
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
      totalPasos: Math.round(totalPasos * 100) / 100,
      promedioPasos: Math.round(promedio * 100) / 100,
      maximoPasos: maximo,
      minimoPasos: minimo,
      rangoValores: `${minimo} - ${maximo}`,
      distribucionTiempo: {
        primerRegistro: new Date(registrosMismoMinuto[registrosMismoMinuto.length - 1].fecha).toISOString(),
        ultimoRegistro: new Date(registrosMismoMinuto[0].fecha).toISOString()
      }
    });
  }

  console.log('✅ PROCESAMIENTO COMPLETADO');
  return registrosMismoMinuto;
}

// Datos de ejemplo que proporcionaste
const datosEjemplo: RegistroPasos[] = [
  {
    "fecha": "2025-07-04T10:58:07.208722+00:00",
    "valor": 27.1301915174154,
    "unidad": "pasos"
  },
  {
    "fecha": "2025-07-04T10:58:07.210215+00:00",
    "valor": 76.563114858452,
    "unidad": "pasos"
  },
  {
    "fecha": "2025-07-04T10:58:07.210228+00:00",
    "valor": 76.563114858452,
    "unidad": "pasos"
  },
  {
    "fecha": "2025-07-04T10:58:07.210236+00:00",
    "valor": 76.563114858452,
    "unidad": "pasos"
  },
  {
    "fecha": "2025-07-05T07:36:50.179615+00:00",
    "valor": 1,
    "unidad": "pasos"
  },
  {
    "fecha": "2025-07-05T07:36:50.179648+00:00",
    "valor": 12.8593648675682,
    "unidad": "pasos"
  },
  {
    "fecha": "2025-07-05T07:36:50.17966+00:00",
    "valor": 23.1406351324318,
    "unidad": "pasos"
  }
];

// Ejecutar el procesamiento de ejemplo
console.log('🧪 EJECUTANDO PROCESAMIENTO DE DATOS DE EJEMPLO...');
const resultados = procesarDatosPasosUltimoMinuto(datosEjemplo);

// Función para usar en la consola del navegador
(window as any).procesarDatosPasos = procesarDatosPasosUltimoMinuto;

console.log('💡 INSTRUCCIONES DE USO:');
console.log('1. Copia este archivo en la consola del navegador');
console.log('2. Copia tus datos de la API en una variable: const misDatos = [...]');
console.log('3. Ejecuta: procesarDatosPasos(misDatos)');
console.log('4. O usa la función global: window.procesarDatosPasos(misDatos)'); 