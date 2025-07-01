using HealthPredict.DAL;
using HealthPredict.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthPredict.BLL
{
    public class InteligenciaPredictiva
    {
        private readonly HealthPredictContext _context;

        public InteligenciaPredictiva(HealthPredictContext context)
        {
            _context = context;
        }

        // ANÁLISIS DE PATRONES DE COMPORTAMIENTO
        public async Task<PatronComportamiento> AnalizarPatronesComportamiento(int usuarioId)
        {
            var datos = await ObtenerDatosUltimos30Dias(usuarioId);
            
            return new PatronComportamiento
            {
                UsuarioId = usuarioId,
                PatronSueño = AnalizarPatronSueño(datos),
                PatronActividad = AnalizarPatronActividad(datos),
                PatronEstres = AnalizarPatronEstres(datos),
                PatronVital = AnalizarPatronVital(datos),
                TendenciaGeneral = CalcularTendenciaGeneral(datos),
                FactoresRiesgo = IdentificarFactoresRiesgo(datos),
                FechaAnalisis = DateTime.Now
            };
        }

        // PREDICCIÓN DE BURNOUT Y ESTRÉS
        public async Task<PrediccionBurnout> PredecirBurnout(int usuarioId)
        {
            var datos = await ObtenerDatosUltimos30Dias(usuarioId);
            var alertas = await ObtenerAlertasRecientes(usuarioId);
            
            var scoreEstres = CalcularScoreEstres(datos);
            var scoreFisico = CalcularScoreFisico(datos);
            var scoreComportamental = CalcularScoreComportamental(datos);
            
            var probabilidadBurnout = CalcularProbabilidadBurnout(scoreEstres, scoreFisico, scoreComportamental, alertas.Count);
            
            return new PrediccionBurnout
            {
                UsuarioId = usuarioId,
                ProbabilidadBurnout = probabilidadBurnout,
                NivelRiesgo = ClasificarRiesgoBurnout(probabilidadBurnout),
                FactoresPrincipales = IdentificarFactoresBurnout(datos, alertas),
                PrediccionProximos7Dias = PredecirTendenciaEstres(datos),
                PrediccionProximos30Dias = PredecirTendenciaEstresLargoPlazo(datos),
                RecomendacionesPrevencion = GenerarRecomendacionesPrevencionBurnout(probabilidadBurnout, datos),
                FechaPrediccion = DateTime.Now
            };
        }

        // DETECCIÓN TEMPRANA DE ENFERMEDADES
        public async Task<DeteccionTemprana> DetectarRiesgosTempranos(int usuarioId)
        {
            var datos = await ObtenerDatosUltimos60Dias(usuarioId);
            
            var riesgos = new List<RiesgoDetectado>();
            
            // Análisis cardiovascular
            var riesgoCardiovascular = AnalizarRiesgoCardiovascular(datos);
            if (riesgoCardiovascular.NivelRiesgo != "Bajo")
                riesgos.Add(riesgoCardiovascular);
            
            // Análisis metabólico
            var riesgoMetabolico = AnalizarRiesgoMetabolico(datos);
            if (riesgoMetabolico.NivelRiesgo != "Bajo")
                riesgos.Add(riesgoMetabolico);
            
            // Análisis de salud mental
            var riesgoMental = AnalizarRiesgoSaludMental(datos);
            if (riesgoMental.NivelRiesgo != "Bajo")
                riesgos.Add(riesgoMental);

            return new DeteccionTemprana
            {
                UsuarioId = usuarioId,
                RiesgosDetectados = riesgos,
                ScoreRiesgoGeneral = CalcularScoreRiesgoGeneral(riesgos),
                RecomendacionesMedicas = GenerarRecomendacionesMedicas(riesgos),
                SeguimientoRequerido = DeterminarSeguimientoRequerido(riesgos),
                ProximaEvaluacion = CalcularProximaEvaluacion(riesgos),
                FechaDeteccion = DateTime.Now
            };
        }

        // RECOMENDACIONES PERSONALIZADAS
        public async Task<RecomendacionesPersonalizadas> GenerarRecomendacionesPersonalizadas(int usuarioId)
        {
            var datos = await ObtenerDatosUltimos30Dias(usuarioId);
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            var patron = await AnalizarPatronesComportamiento(usuarioId);
            var prediccionBurnout = await PredecirBurnout(usuarioId);
            
            var recomendaciones = new List<Recomendacion>();
            
            // Recomendaciones de actividad física
            recomendaciones.AddRange(GenerarRecomendacionesActividad(datos, usuario));
            
            // Recomendaciones de sueño
            recomendaciones.AddRange(GenerarRecomendacionesSueño(datos, patron));
            
            // Recomendaciones de manejo de estrés
            recomendaciones.AddRange(GenerarRecomendacionesEstres(datos, prediccionBurnout));

            return new RecomendacionesPersonalizadas
            {
                UsuarioId = usuarioId,
                Recomendaciones = recomendaciones.OrderByDescending(r => r.Prioridad).ToList(),
                ScorePersonalizacion = CalcularScorePersonalizacion(recomendaciones, patron),
                FechaGeneracion = DateTime.Now,
                ValidoHasta = DateTime.Now.AddDays(7)
            };
        }

        // MÉTODOS AUXILIARES SIMPLIFICADOS
        private PatronSueño AnalizarPatronSueño(List<DatoVital> datos)
        {
            var datosSueño = datos.Where(d => d.TipoDato.ToLower().Contains("sueño")).ToList();
            if (!datosSueño.Any()) return new PatronSueño { Calidad = "Sin datos" };

            var promedioHoras = datosSueño.Average(d => d.Valor);
            var tendencia = CalcularTendencia(datosSueño);

            return new PatronSueño
            {
                PromedioHoras = Math.Round((double)promedioHoras, 1),
                Tendencia = tendencia,
                Calidad = ClasificarCalidadSueño((double)promedioHoras),
                Consistencia = EvaluarConsistenciaSueño(datosSueño)
            };
        }

        private PatronActividad AnalizarPatronActividad(List<DatoVital> datos)
        {
            var datosActividad = datos.Where(d => d.TipoDato.ToLower().Contains("pasos")).ToList();
            if (!datosActividad.Any()) return new PatronActividad { Nivel = "Sin datos" };

            var promedioPasos = datosActividad.Average(d => d.Valor);
            var tendencia = CalcularTendencia(datosActividad);

            return new PatronActividad
            {
                PromedioPasos = (int)promedioPasos,
                Tendencia = tendencia,
                Nivel = ClasificarNivelActividad((double)promedioPasos),
                DiasActivos = ContarDiasActivos(datosActividad)
            };
        }

        private PatronEstres AnalizarPatronEstres(List<DatoVital> datos)
        {
            var datosEstres = datos.Where(d => d.TipoDato.ToLower().Contains("estrés")).ToList();
            if (!datosEstres.Any()) return new PatronEstres { Nivel = "Sin datos" };

            var promedioEstres = datosEstres.Average(d => d.Valor);
            var tendencia = CalcularTendencia(datosEstres);

            return new PatronEstres
            {
                PromedioNivel = Math.Round((double)promedioEstres, 1),
                Tendencia = tendencia,
                Nivel = ClasificarNivelEstres((double)promedioEstres),
                PicosEstres = ContarPicosEstres(datosEstres)
            };
        }

        private PatronVital AnalizarPatronVital(List<DatoVital> datos)
        {
            var indicadoresPreocupantes = new List<string>();
            
            // Análisis de presión arterial
            var datosPresion = datos.Where(d => d.TipoDato.ToLower().Contains("presión")).ToList();
            if (datosPresion.Any() && datosPresion.Average(d => d.Valor) > 140)
                indicadoresPreocupantes.Add("Presión arterial elevada");
            
            // Análisis de frecuencia cardíaca
            var datosFC = datos.Where(d => d.TipoDato.ToLower().Contains("cardíaca")).ToList();
            if (datosFC.Any() && (datosFC.Average(d => d.Valor) > 100 || datosFC.Average(d => d.Valor) < 60))
                indicadoresPreocupantes.Add("Frecuencia cardíaca anormal");

            return new PatronVital
            {
                EstadoGeneral = indicadoresPreocupantes.Any() ? "Requiere atención" : "Normal",
                IndicadoresPreocupantes = indicadoresPreocupantes,
                TendenciaVital = CalcularTendenciaVital(datos)
            };
        }

        private string CalcularTendenciaGeneral(List<DatoVital> datos)
        {
            var tendencias = new List<string>();
            
            var gruposPorTipo = datos.GroupBy(d => d.TipoDato);
            foreach (var grupo in gruposPorTipo)
            {
                tendencias.Add(CalcularTendencia(grupo.ToList()));
            }
            
            var subiendo = tendencias.Count(t => t == "Subiendo");
            var bajando = tendencias.Count(t => t == "Bajando");
            
            if (subiendo > bajando) return "Mejorando";
            if (bajando > subiendo) return "Deteriorando";
            return "Estable";
        }

        private List<string> IdentificarFactoresRiesgo(List<DatoVital> datos)
        {
            var factores = new List<string>();
            
            // Factor estrés
            var datosEstres = datos.Where(d => d.TipoDato.ToLower().Contains("estrés")).ToList();
            if (datosEstres.Any() && datosEstres.Average(d => d.Valor) > 6)
                factores.Add("Estrés elevado persistente");
            
            // Factor sueño
            var datosSueño = datos.Where(d => d.TipoDato.ToLower().Contains("sueño")).ToList();
            if (datosSueño.Any() && datosSueño.Average(d => d.Valor) < 6)
                factores.Add("Sueño insuficiente");
            
            // Factor actividad
            var datosActividad = datos.Where(d => d.TipoDato.ToLower().Contains("pasos")).ToList();
            if (datosActividad.Any() && datosActividad.Average(d => d.Valor) < 5000)
                factores.Add("Sedentarismo");

            return factores;
        }

        private double CalcularScoreEstres(List<DatoVital> datos)
        {
            var datosEstres = datos.Where(d => d.TipoDato.ToLower().Contains("estrés")).ToList();
            if (!datosEstres.Any()) return 30; // Score moderado por defecto
            
            var promedioEstres = datosEstres.Average(d => d.Valor);
            return Math.Min((double)promedioEstres * 10, 100); // Convertir a escala 0-100
        }

        private double CalcularScoreFisico(List<DatoVital> datos)
        {
            var score = 0.0;
            
            // Factor presión arterial
            var datosPresion = datos.Where(d => d.TipoDato.ToLower().Contains("presión")).ToList();
            if (datosPresion.Any())
            {
                var promedioPresion = datosPresion.Average(d => d.Valor);
                if (promedioPresion > 140) score += 30;
                else if (promedioPresion > 130) score += 20;
                else if (promedioPresion > 120) score += 10;
            }
            
            // Factor actividad física
            var datosActividad = datos.Where(d => d.TipoDato.ToLower().Contains("pasos")).ToList();
            if (datosActividad.Any())
            {
                var promedioPasos = datosActividad.Average(d => d.Valor);
                if (promedioPasos < 5000) score += 25;
                else if (promedioPasos < 7000) score += 15;
                else if (promedioPasos < 10000) score += 5;
            }
            
            return Math.Min(score, 100);
        }

        private double CalcularScoreComportamental(List<DatoVital> datos)
        {
            var score = 0.0;
            
            // Factor sueño
            var datosSueño = datos.Where(d => d.TipoDato.ToLower().Contains("sueño")).ToList();
            if (datosSueño.Any())
            {
                var promedioSueño = datosSueño.Average(d => d.Valor);
                if (promedioSueño < 5) score += 30;
                else if (promedioSueño < 6) score += 20;
                else if (promedioSueño < 7) score += 10;
            }
            
            return Math.Min(score, 100);
        }

        private double CalcularProbabilidadBurnout(double scoreEstres, double scoreFisico, double scoreComportamental, int alertasRecientes)
        {
            var probabilidadBase = (scoreEstres * 0.4 + scoreFisico * 0.3 + scoreComportamental * 0.2 + Math.Min(alertasRecientes * 10, 100) * 0.1);
            return Math.Min(probabilidadBase, 95);
        }

        private string ClasificarRiesgoBurnout(double probabilidad)
        {
            if (probabilidad >= 70) return "Crítico";
            if (probabilidad >= 50) return "Alto";
            if (probabilidad >= 30) return "Moderado";
            return "Bajo";
        }

        private List<string> IdentificarFactoresBurnout(List<DatoVital> datos, List<Alerta> alertas)
        {
            var factores = new List<string>();
            
            if (alertas.Count > 3) factores.Add("Múltiples alertas de salud");
            
            var datosEstres = datos.Where(d => d.TipoDato.ToLower().Contains("estrés")).ToList();
            if (datosEstres.Any() && datosEstres.Average(d => d.Valor) > 7)
                factores.Add("Estrés crónico elevado");
            
            var datosSueño = datos.Where(d => d.TipoDato.ToLower().Contains("sueño")).ToList();
            if (datosSueño.Any() && datosSueño.Average(d => d.Valor) < 6)
                factores.Add("Privación crónica del sueño");
            
            return factores;
        }

        private string PredecirTendenciaEstres(List<DatoVital> datos)
        {
            var datosEstres = datos.Where(d => d.TipoDato.ToLower().Contains("estrés")).ToList();
            if (!datosEstres.Any()) return "Sin datos suficientes";
            
            var tendencia = CalcularTendencia(datosEstres);
            
            if (tendencia == "Subiendo") return "Probable aumento del estrés en próximos 7 días";
            if (tendencia == "Bajando") return "Probable mejora del estrés en próximos 7 días";
            return "Estrés estable previsto para próximos 7 días";
        }

        private string PredecirTendenciaEstresLargoPlazo(List<DatoVital> datos)
        {
            var datosEstres = datos.Where(d => d.TipoDato.ToLower().Contains("estrés")).ToList();
            if (!datosEstres.Any()) return "Sin datos suficientes";
            
            var promedioReciente = datosEstres.TakeLast(7).Average(d => d.Valor);
            var promedioAnterior = datosEstres.Take(datosEstres.Count - 7).Average(d => d.Valor);
            
            var diferencia = promedioReciente - promedioAnterior;
            
            if (diferencia > 1) return "Riesgo de estrés crónico en próximos 30 días";
            if (diferencia < -1) return "Probable mejora sostenida en próximos 30 días";
            return "Niveles de estrés estables previstos";
        }

        private List<string> GenerarRecomendacionesPrevencionBurnout(double probabilidad, List<DatoVital> datos)
        {
            var recomendaciones = new List<string>();
            
            if (probabilidad > 50)
            {
                recomendaciones.Add("🚨 Considere consulta con profesional de salud mental");
                recomendaciones.Add("⏰ Implementar técnicas de manejo de tiempo");
                recomendaciones.Add("🧘 Práctica diaria de mindfulness o meditación");
            }
            
            if (probabilidad > 30)
            {
                recomendaciones.Add("💤 Establecer rutina de sueño consistente");
                recomendaciones.Add("🚶 Incrementar actividad física gradualmente");
                recomendaciones.Add("👥 Buscar apoyo social y profesional");
            }
            
            return recomendaciones;
        }

        // MÉTODOS DE DETECCIÓN TEMPRANA
        private RiesgoDetectado AnalizarRiesgoCardiovascular(List<DatoVital> datos)
        {
            var datosPresion = datos.Where(d => d.TipoDato.ToLower().Contains("presión")).ToList();
            var datosFC = datos.Where(d => d.TipoDato.ToLower().Contains("cardíaca")).ToList();
            
            var riesgoPresion = EvaluarRiesgoPresion(datosPresion);
            var riesgoFC = EvaluarRiesgoFrecuenciaCardiaca(datosFC);
            
            var nivelRiesgo = Math.Max(riesgoPresion, riesgoFC);
            
            return new RiesgoDetectado
            {
                TipoRiesgo = "Cardiovascular",
                NivelRiesgo = ClasificarNivel(nivelRiesgo),
                Probabilidad = nivelRiesgo,
                Descripcion = GenerarDescripcionRiesgoCardiovascular(riesgoPresion, riesgoFC),
                FactoresContribuyentes = IdentificarFactoresCardiovasculares(datosPresion, datosFC),
                RecomendacionesInmediatas = GenerarRecomendacionesCardiovasculares(nivelRiesgo)
            };
        }

        private RiesgoDetectado AnalizarRiesgoMetabolico(List<DatoVital> datos)
        {
            var datosGlucosa = datos.Where(d => d.TipoDato.ToLower().Contains("glucosa")).ToList();
            var datosPeso = datos.Where(d => d.TipoDato.ToLower().Contains("peso")).ToList();
            
            var riesgo = 0.0;
            
            if (datosGlucosa.Any() && datosGlucosa.Average(d => d.Valor) > 140)
                riesgo += 40;
            
            if (datosPeso.Any())
            {
                var tendenciaPeso = CalcularTendencia(datosPeso);
                if (tendenciaPeso == "Subiendo") riesgo += 20;
            }
            
            return new RiesgoDetectado
            {
                TipoRiesgo = "Metabólico",
                NivelRiesgo = ClasificarNivel(riesgo),
                Probabilidad = riesgo,
                Descripcion = "Análisis de factores de riesgo metabólico",
                FactoresContribuyentes = new List<string> { "Análisis glucosa", "Tendencia peso" },
                RecomendacionesInmediatas = new List<string> { "Monitoreo nutricional", "Consulta médica" }
            };
        }

        private RiesgoDetectado AnalizarRiesgoSaludMental(List<DatoVital> datos)
        {
            var datosEstres = datos.Where(d => d.TipoDato.ToLower().Contains("estrés")).ToList();
            var datosSueño = datos.Where(d => d.TipoDato.ToLower().Contains("sueño")).ToList();
            
            var riesgo = 0.0;
            
            if (datosEstres.Any() && datosEstres.Average(d => d.Valor) > 7)
                riesgo += 50;
            
            if (datosSueño.Any() && datosSueño.Average(d => d.Valor) < 6)
                riesgo += 30;
            
            return new RiesgoDetectado
            {
                TipoRiesgo = "Salud Mental",
                NivelRiesgo = ClasificarNivel(riesgo),
                Probabilidad = riesgo,
                Descripcion = "Evaluación de factores de riesgo psicológico",
                FactoresContribuyentes = new List<string> { "Estrés crónico", "Alteraciones del sueño" },
                RecomendacionesInmediatas = new List<string> { "Apoyo psicológico", "Técnicas de relajación" }
            };
        }

        // MÉTODOS DE RECOMENDACIONES
        private List<Recomendacion> GenerarRecomendacionesActividad(List<DatoVital> datos, Usuario usuario)
        {
            var recomendaciones = new List<Recomendacion>();
            
            var datosActividad = datos.Where(d => d.TipoDato.ToLower().Contains("pasos")).ToList();
            if (datosActividad.Any())
            {
                var promedioPasos = datosActividad.Average(d => d.Valor);
                
                if (promedioPasos < 5000)
                {
                    recomendaciones.Add(new Recomendacion
                    {
                        Categoria = "Actividad Física",
                        Titulo = "Incrementar actividad diaria",
                        Descripcion = "Tu nivel de actividad está por debajo del recomendado",
                        Prioridad = 8,
                        TipoAccion = "Ejercicio",
                        PasosEspecificos = new List<string> 
                        { 
                            "Caminar 10 minutos extra diarios",
                            "Usar escaleras en lugar de ascensor",
                            "Pausas activas cada 2 horas"
                        },
                        Justificacion = $"Promedio actual: {(int)promedioPasos} pasos/día. Recomendado: 8000+"
                    });
                }
            }
            
            return recomendaciones;
        }

        private List<Recomendacion> GenerarRecomendacionesSueño(List<DatoVital> datos, PatronComportamiento patron)
        {
            var recomendaciones = new List<Recomendacion>();
            
            if (patron.PatronSueño.PromedioHoras < 7)
            {
                recomendaciones.Add(new Recomendacion
                {
                    Categoria = "Sueño",
                    Titulo = "Mejorar calidad del sueño",
                    Descripcion = "Tu tiempo de sueño está por debajo del recomendado",
                    Prioridad = 9,
                    TipoAccion = "Higiene del sueño",
                    PasosEspecificos = new List<string>
                    {
                        "Establecer horario fijo para dormir",
                        "Evitar pantallas 1 hora antes de dormir",
                        "Crear ambiente oscuro y fresco"
                    },
                    Justificacion = $"Promedio actual: {patron.PatronSueño.PromedioHoras}h. Recomendado: 7-9h"
                });
            }
            
            return recomendaciones;
        }

        private List<Recomendacion> GenerarRecomendacionesEstres(List<DatoVital> datos, PrediccionBurnout prediccion)
        {
            var recomendaciones = new List<Recomendacion>();
            
            if (prediccion.ProbabilidadBurnout > 50)
            {
                recomendaciones.Add(new Recomendacion
                {
                    Categoria = "Manejo de Estrés",
                    Titulo = "Intervención urgente para estrés",
                    Descripcion = "Alto riesgo de burnout detectado",
                    Prioridad = 10,
                    TipoAccion = "Intervención",
                    PasosEspecificos = new List<string>
                    {
                        "Consultar con profesional de salud mental",
                        "Implementar técnicas de respiración",
                        "Revisar carga laboral con supervisor"
                    },
                    Justificacion = $"Probabilidad de burnout: {prediccion.ProbabilidadBurnout:F1}%"
                });
            }
            
            return recomendaciones;
        }

        // MÉTODOS DE CÁLCULO Y UTILIDADES
        private string CalcularTendencia(List<DatoVital> datos)
        {
            if (datos.Count < 2) return "Estable";
            
            var ordenados = datos.OrderBy(d => d.FechaRegistro).ToList();
            var primeraMitad = ordenados.Take(ordenados.Count / 2).Average(d => d.Valor);
            var segundaMitad = ordenados.Skip(ordenados.Count / 2).Average(d => d.Valor);
            
            var diferencia = segundaMitad - primeraMitad;
            var porcentajeCambio = Math.Abs(diferencia) / primeraMitad * 100;
            
            if (porcentajeCambio < 5) return "Estable";
            return diferencia > 0 ? "Subiendo" : "Bajando";
        }

        private string ClasificarNivel(double valor)
        {
            if (valor >= 70) return "Crítico";
            if (valor >= 50) return "Alto";
            if (valor >= 25) return "Moderado";
            return "Bajo";
        }

        private string ClasificarCalidadSueño(double horas)
        {
            if (horas >= 7 && horas <= 9) return "Óptima";
            if (horas >= 6 && horas < 7) return "Aceptable";
            if (horas >= 5 && horas < 6) return "Insuficiente";
            return "Crítica";
        }

        private string ClasificarNivelActividad(double pasos)
        {
            if (pasos >= 10000) return "Muy activo";
            if (pasos >= 7500) return "Activo";
            if (pasos >= 5000) return "Moderadamente activo";
            return "Sedentario";
        }

        private string ClasificarNivelEstres(double nivel)
        {
            if (nivel >= 8) return "Crítico";
            if (nivel >= 6) return "Alto";
            if (nivel >= 4) return "Moderado";
            return "Bajo";
        }

        // MÉTODOS DE OBTENCIÓN DE DATOS
        private async Task<List<DatoVital>> ObtenerDatosUltimos30Dias(int usuarioId)
        {
            var fechaInicio = DateTime.Now.AddDays(-30);
            return await _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId && d.FechaRegistro >= fechaInicio)
                .OrderBy(d => d.FechaRegistro)
                .ToListAsync();
        }

        private async Task<List<DatoVital>> ObtenerDatosUltimos60Dias(int usuarioId)
        {
            var fechaInicio = DateTime.Now.AddDays(-60);
            return await _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId && d.FechaRegistro >= fechaInicio)
                .OrderBy(d => d.FechaRegistro)
                .ToListAsync();
        }

        private async Task<List<Alerta>> ObtenerAlertasRecientes(int usuarioId)
        {
            var fechaInicio = DateTime.Now.AddDays(-14);
            return await _context.Alertas
                .Where(a => a.UsuarioId == usuarioId && a.FechaCreacion >= fechaInicio)
                .ToListAsync();
        }

        // MÉTODOS AUXILIARES FALTANTES
        private string EvaluarConsistenciaSueño(List<DatoVital> datosSueño)
        {
            if (datosSueño.Count < 3) return "Datos insuficientes";
            
            var desviacion = CalcularDesviacionEstandar(datosSueño.Select(d => (double)d.Valor).ToList());
            
            if (desviacion < 1) return "Muy consistente";
            if (desviacion < 2) return "Consistente";
            return "Irregular";
        }

        private int ContarDiasActivos(List<DatoVital> datosActividad)
        {
            return datosActividad.Count(d => d.Valor >= 5000);
        }

        private int ContarPicosEstres(List<DatoVital> datosEstres)
        {
            return datosEstres.Count(d => d.Valor >= 8);
        }

        private string CalcularTendenciaVital(List<DatoVital> datos)
        {
            var indicadoresPositivos = 0;
            var indicadoresNegativos = 0;
            
            var gruposPorTipo = datos.GroupBy(d => d.TipoDato);
            foreach (var grupo in gruposPorTipo)
            {
                var tendencia = CalcularTendencia(grupo.ToList());
                if (EsTendenciaPositiva(grupo.Key, tendencia))
                    indicadoresPositivos++;
                else if (EsTendenciaNegativa(grupo.Key, tendencia))
                    indicadoresNegativos++;
            }
            
            if (indicadoresPositivos > indicadoresNegativos) return "Mejorando";
            if (indicadoresNegativos > indicadoresPositivos) return "Deteriorando";
            return "Estable";
        }

        private bool EsTendenciaPositiva(string tipoDato, string tendencia)
        {
            var tiposPositivosSubiendo = new[] { "pasos", "sueño", "actividad" };
            var tiposPositivosBajando = new[] { "estrés", "presión" };
            
            if (tiposPositivosSubiendo.Any(t => tipoDato.ToLower().Contains(t)))
                return tendencia == "Subiendo";
            
            if (tiposPositivosBajando.Any(t => tipoDato.ToLower().Contains(t)))
                return tendencia == "Bajando";
            
            return false;
        }

        private bool EsTendenciaNegativa(string tipoDato, string tendencia)
        {
            var tiposNegativosSubiendo = new[] { "estrés", "presión" };
            var tiposNegativosBajando = new[] { "pasos", "sueño", "actividad" };
            
            if (tiposNegativosSubiendo.Any(t => tipoDato.ToLower().Contains(t)))
                return tendencia == "Subiendo";
            
            if (tiposNegativosBajando.Any(t => tipoDato.ToLower().Contains(t)))
                return tendencia == "Bajando";
            
            return false;
        }

        private double EvaluarRiesgoPresion(List<DatoVital> datosPresion)
        {
            if (!datosPresion.Any()) return 0;
            
            var promedioPresion = datosPresion.Average(d => d.Valor);
            
            if (promedioPresion >= 160) return 80;
            if (promedioPresion >= 140) return 60;
            if (promedioPresion >= 130) return 40;
            if (promedioPresion >= 120) return 20;
            return 0;
        }

        private double EvaluarRiesgoFrecuenciaCardiaca(List<DatoVital> datosFC)
        {
            if (!datosFC.Any()) return 0;
            
            var promedioFC = datosFC.Average(d => d.Valor);
            
            if (promedioFC >= 120 || promedioFC <= 50) return 70;
            if (promedioFC >= 100 || promedioFC <= 60) return 40;
            if (promedioFC >= 90 || promedioFC <= 65) return 20;
            return 0;
        }

        private string GenerarDescripcionRiesgoCardiovascular(double riesgoPresion, double riesgoFC)
        {
            if (riesgoPresion > 60 && riesgoFC > 40)
                return "Riesgo cardiovascular elevado por presión arterial y frecuencia cardíaca";
            if (riesgoPresion > 40)
                return "Riesgo por presión arterial elevada";
            if (riesgoFC > 40)
                return "Riesgo por frecuencia cardíaca anormal";
            return "Indicadores cardiovasculares dentro de rangos aceptables";
        }

        private List<string> IdentificarFactoresCardiovasculares(List<DatoVital> datosPresion, List<DatoVital> datosFC)
        {
            var factores = new List<string>();
            
            if (datosPresion.Any() && datosPresion.Average(d => d.Valor) > 140)
                factores.Add("Hipertensión arterial");
            
            if (datosFC.Any())
            {
                var promedioFC = datosFC.Average(d => d.Valor);
                if (promedioFC > 100) factores.Add("Taquicardia");
                if (promedioFC < 60) factores.Add("Bradicardia");
            }
            
            return factores;
        }

        private List<string> GenerarRecomendacionesCardiovasculares(double nivelRiesgo)
        {
            var recomendaciones = new List<string>();
            
            if (nivelRiesgo > 60)
            {
                recomendaciones.Add("Consulta médica urgente");
                recomendaciones.Add("Monitoreo continuo de presión arterial");
            }
            else if (nivelRiesgo > 30)
            {
                recomendaciones.Add("Reducir consumo de sal");
                recomendaciones.Add("Incrementar ejercicio cardiovascular");
            }
            
            return recomendaciones;
        }

        private double CalcularScoreRiesgoGeneral(List<RiesgoDetectado> riesgos)
        {
            if (!riesgos.Any()) return 0;
            
            return riesgos.Average(r => r.Probabilidad);
        }

        private List<string> GenerarRecomendacionesMedicas(List<RiesgoDetectado> riesgos)
        {
            var recomendaciones = new List<string>();
            
            foreach (var riesgo in riesgos)
            {
                recomendaciones.AddRange(riesgo.RecomendacionesInmediatas);
            }
            
            return recomendaciones.Distinct().ToList();
        }

        private string DeterminarSeguimientoRequerido(List<RiesgoDetectado> riesgos)
        {
            if (riesgos.Any(r => r.NivelRiesgo == "Crítico"))
                return "Seguimiento inmediato requerido";
            if (riesgos.Any(r => r.NivelRiesgo == "Alto"))
                return "Seguimiento semanal recomendado";
            if (riesgos.Any(r => r.NivelRiesgo == "Moderado"))
                return "Seguimiento quincenal sugerido";
            return "Seguimiento mensual suficiente";
        }

        private DateTime CalcularProximaEvaluacion(List<RiesgoDetectado> riesgos)
        {
            if (riesgos.Any(r => r.NivelRiesgo == "Crítico"))
                return DateTime.Now.AddDays(1);
            if (riesgos.Any(r => r.NivelRiesgo == "Alto"))
                return DateTime.Now.AddDays(7);
            if (riesgos.Any(r => r.NivelRiesgo == "Moderado"))
                return DateTime.Now.AddDays(15);
            return DateTime.Now.AddDays(30);
        }

        private double CalcularScorePersonalizacion(List<Recomendacion> recomendaciones, PatronComportamiento patron)
        {
            // Score basado en la cantidad y relevancia de las recomendaciones
            var scoreBase = Math.Min(recomendaciones.Count * 10, 100);
            var scorePrioridad = recomendaciones.Average(r => r.Prioridad) * 10;
            
            return (scoreBase + scorePrioridad) / 2;
        }

        private double CalcularDesviacionEstandar(List<double> valores)
        {
            if (valores.Count < 2) return 0;
            
            var promedio = valores.Average();
            var sumaCuadrados = valores.Sum(v => Math.Pow(v - promedio, 2));
            var varianza = sumaCuadrados / (valores.Count - 1);
            
            return Math.Sqrt(varianza);
        }
    }

    // MODELOS DE DATOS PARA INTELIGENCIA PREDICTIVA
    public class PatronComportamiento
    {
        public int UsuarioId { get; set; }
        public PatronSueño PatronSueño { get; set; }
        public PatronActividad PatronActividad { get; set; }
        public PatronEstres PatronEstres { get; set; }
        public PatronVital PatronVital { get; set; }
        public string TendenciaGeneral { get; set; }
        public List<string> FactoresRiesgo { get; set; }
        public DateTime FechaAnalisis { get; set; }
    }

    public class PatronSueño
    {
        public double PromedioHoras { get; set; }
        public string Tendencia { get; set; }
        public string Calidad { get; set; }
        public string Consistencia { get; set; }
    }

    public class PatronActividad
    {
        public int PromedioPasos { get; set; }
        public string Tendencia { get; set; }
        public string Nivel { get; set; }
        public int DiasActivos { get; set; }
    }

    public class PatronEstres
    {
        public double PromedioNivel { get; set; }
        public string Tendencia { get; set; }
        public string Nivel { get; set; }
        public int PicosEstres { get; set; }
    }

    public class PatronVital
    {
        public string EstadoGeneral { get; set; }
        public List<string> IndicadoresPreocupantes { get; set; }
        public string TendenciaVital { get; set; }
    }

    public class PrediccionBurnout
    {
        public int UsuarioId { get; set; }
        public double ProbabilidadBurnout { get; set; }
        public string NivelRiesgo { get; set; }
        public List<string> FactoresPrincipales { get; set; }
        public string PrediccionProximos7Dias { get; set; }
        public string PrediccionProximos30Dias { get; set; }
        public List<string> RecomendacionesPrevencion { get; set; }
        public DateTime FechaPrediccion { get; set; }
    }

    public class DeteccionTemprana
    {
        public int UsuarioId { get; set; }
        public List<RiesgoDetectado> RiesgosDetectados { get; set; }
        public double ScoreRiesgoGeneral { get; set; }
        public List<string> RecomendacionesMedicas { get; set; }
        public string SeguimientoRequerido { get; set; }
        public DateTime ProximaEvaluacion { get; set; }
        public DateTime FechaDeteccion { get; set; }
    }

    public class RiesgoDetectado
    {
        public string TipoRiesgo { get; set; }
        public string NivelRiesgo { get; set; }
        public double Probabilidad { get; set; }
        public string Descripcion { get; set; }
        public List<string> FactoresContribuyentes { get; set; }
        public List<string> RecomendacionesInmediatas { get; set; }
    }

    public class RecomendacionesPersonalizadas
    {
        public int UsuarioId { get; set; }
        public List<Recomendacion> Recomendaciones { get; set; }
        public double ScorePersonalizacion { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime ValidoHasta { get; set; }
    }

    public class Recomendacion
    {
        public string Categoria { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Prioridad { get; set; }
        public string TipoAccion { get; set; }
        public List<string> PasosEspecificos { get; set; }
        public string Justificacion { get; set; }
    }

    public class AlertaPreventiva
    {
        public string TipoAlerta { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Urgencia { get; set; }
        public bool RequiereAccion { get; set; }
        public string AccionRecomendada { get; set; }
        public DateTime FechaDeteccion { get; set; }
    }
} 