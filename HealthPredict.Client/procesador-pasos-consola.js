/**
 * PROCESADOR DE DATOS DE PASOS - ÚLTIMO MINUTO
 * 
 * Copia y pega este código en la consola del navegador
 * Luego usa: procesarDatosPasos(tusDatos)
 */

function procesarDatosPasos(datosPasos) {
  if (!datosPasos || datosPasos.length === 0) {
    console.log('❌ No hay datos de pasos para procesar');
    return [];
  }

  console.log('🚀 INICIANDO PROCESAMIENTO DE DATOS DE PASOS');
  console.log('📊 Total de registros recibidos:', datosPasos.length);

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
    minuto: ultimoMinuto,
    fechaFormateada: `${ultimoDia}/${ultimoMes + 1}/${ultimoAno} ${ultimaHora.toString().padStart(2, '0')}:${ultimoMinuto.toString().padStart(2, '0')}`
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

// Hacer la función disponible globalmente
window.procesarDatosPasos = procesarDatosPasos;

// Datos de ejemplo para probar
const datosEjemplo = [
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

console.log('🧪 EJECUTANDO PROCESAMIENTO DE DATOS DE EJEMPLO...');
const resultados = procesarDatosPasos(datosEjemplo);

// Función adicional para procesar datos del formato de la API
function procesarDatosDesdeAPI(datosAPI) {
  console.log('🔄 Convirtiendo datos del formato de la API...');
  
  if (!datosAPI || datosAPI.length === 0) {
    console.log('❌ No hay datos de la API para procesar');
    return [];
  }
  
  const datosFormateados = datosAPI.map((dato, index) => {
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
  }).filter(dato => dato !== null); // Filtrar datos nulos
  
  console.log('📊 Datos convertidos:', datosFormateados.length, 'registros válidos');
  
  if (datosFormateados.length === 0) {
    console.log('❌ No hay datos válidos para procesar después de la conversión');
    return [];
  }
  
  return procesarDatosPasos(datosFormateados);
}

// Hacer ambas funciones disponibles globalmente
window.procesarDatosPasos = procesarDatosPasos;
window.procesarDatosDesdeAPI = procesarDatosDesdeAPI;

console.log('💡 INSTRUCCIONES DE USO:');
console.log('');
console.log('📡 PARA DATOS DE LA API (formato DatoVital):');
console.log('1. Copia tus datos de la API: const misDatosAPI = [...]');
console.log('2. Ejecuta: procesarDatosDesdeAPI(misDatosAPI)');
console.log('');
console.log('📊 PARA DATOS EN FORMATO DIRECTO:');
console.log('1. Copia tus datos: const misDatos = [...]');
console.log('2. Ejecuta: procesarDatosPasos(misDatos)');
console.log('');
console.log('🌐 Las funciones están disponibles globalmente:');
console.log('- window.procesarDatosPasos(datos)');
console.log('- window.procesarDatosDesdeAPI(datosAPI)'); 