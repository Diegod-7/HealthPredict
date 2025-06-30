-- ======================================
-- DATOS DE PRUEBA PARA PANEL SUPERVISOR
-- HealthPredict PostgreSQL Database
-- ======================================

-- USUARIOS JEFES Y SUPERVISORES
INSERT INTO "USUARIOS" ("ID", "NOMBRE", "APELLIDO", "EMAIL", "PASSWORD", "FECHA_NACIMIENTO", "GENERO", "ALTURA", "PESO", "FECHA_REGISTRO", "ULTIMO_ACCESO", "ES_PROFESIONAL_MEDICO", "ESPECIALIDAD", "NUMERO_LICENCIA", "ROL", "DEPARTAMENTO", "CARGO", "JEFE_ID", "ES_ACTIVO") VALUES
(10, 'Carlos', 'Rodríguez', 'carlos.rodriguez@healthpredict.com', 'password123', '1985-03-15', 'Masculino', 1.78, 75.5, '2024-01-15', '2024-12-20 08:30:00', false, null, null, 'Jefe', 'Desarrollo', 'Jefe de Desarrollo', null, true),
(11, 'Ana', 'Martínez', 'ana.martinez@healthpredict.com', 'password123', '1982-08-22', 'Femenino', 1.65, 62.0, '2024-01-15', '2024-12-20 09:15:00', false, null, null, 'Jefe', 'Recursos Humanos', 'Jefe de RRHH', null, true),
(12, 'Miguel', 'Torres', 'miguel.torres@healthpredict.com', 'password123', '1980-12-05', 'Masculino', 1.82, 80.0, '2024-01-15', '2024-12-20 07:45:00', false, null, null, 'Jefe', 'Operaciones', 'Jefe de Operaciones', null, true);

-- TRABAJADORES DEL DEPARTAMENTO DE DESARROLLO
INSERT INTO "USUARIOS" ("ID", "NOMBRE", "APELLIDO", "EMAIL", "PASSWORD", "FECHA_NACIMIENTO", "GENERO", "ALTURA", "PESO", "FECHA_REGISTRO", "ULTIMO_ACCESO", "ES_PROFESIONAL_MEDICO", "ESPECIALIDAD", "NUMERO_LICENCIA", "ROL", "DEPARTAMENTO", "CARGO", "JEFE_ID", "ES_ACTIVO") VALUES
(20, 'Diego', 'Díaz', 'diego.diaz@healthpredict.com', 'password123', '1995-06-10', 'Masculino', 1.75, 70.0, '2024-02-01', '2024-12-20 08:00:00', false, null, null, 'Trabajador', 'Desarrollo', 'Desarrollador Frontend', 10, true),
(21, 'Iahn', 'Vera', 'iahn.vera@healthpredict.com', 'password123', '1993-09-18', 'Masculino', 1.80, 73.5, '2024-02-01', '2024-12-20 09:30:00', false, null, null, 'Trabajador', 'Desarrollo', 'Desarrollador Backend', 10, true),
(22, 'Matías', 'Maripangue', 'matias.maripangue@healthpredict.com', 'password123', '1994-11-25', 'Masculino', 1.77, 68.0, '2024-02-15', '2024-12-20 08:45:00', false, null, null, 'Trabajador', 'Desarrollo', 'Desarrollador Full Stack', 10, true),
(23, 'Sofía', 'Luna', 'sofia.luna@healthpredict.com', 'password123', '1996-04-12', 'Femenino', 1.68, 58.0, '2024-03-01', '2024-12-20 07:30:00', false, null, null, 'Trabajador', 'Desarrollo', 'DevOps Engineer', 10, true);

-- TRABAJADORES DE RECURSOS HUMANOS
INSERT INTO "USUARIOS" ("ID", "NOMBRE", "APELLIDO", "EMAIL", "PASSWORD", "FECHA_NACIMIENTO", "GENERO", "ALTURA", "PESO", "FECHA_REGISTRO", "ULTIMO_ACCESO", "ES_PROFESIONAL_MEDICO", "ESPECIALIDAD", "NUMERO_LICENCIA", "ROL", "DEPARTAMENTO", "CARGO", "JEFE_ID", "ES_ACTIVO") VALUES
(30, 'Laura', 'García', 'laura.garcia@healthpredict.com', 'password123', '1990-07-08', 'Femenino', 1.62, 55.0, '2024-02-10', '2024-12-20 08:15:00', false, null, null, 'Trabajador', 'Recursos Humanos', 'Especialista en Reclutamiento', 11, true),
(31, 'Roberto', 'Vásquez', 'roberto.vasquez@healthpredict.com', 'password123', '1988-02-28', 'Masculino', 1.74, 72.0, '2024-02-15', '2024-12-20 09:00:00', false, null, null, 'Trabajador', 'Recursos Humanos', 'Analista de Compensaciones', 11, true);

-- TRABAJADORES DE OPERACIONES
INSERT INTO "USUARIOS" ("ID", "NOMBRE", "APELLIDO", "EMAIL", "PASSWORD", "FECHA_NACIMIENTO", "GENERO", "ALTURA", "PESO", "FECHA_REGISTRO", "ULTIMO_ACCESO", "ES_PROFESIONAL_MEDICO", "ESPECIALIDAD", "NUMERO_LICENCIA", "ROL", "DEPARTAMENTO", "CARGO", "JEFE_ID", "ES_ACTIVO") VALUES
(40, 'Patricia', 'Mendoza', 'patricia.mendoza@healthpredict.com', 'password123', '1987-05-16', 'Femenino', 1.70, 65.0, '2024-02-20', '2024-12-20 07:50:00', false, null, null, 'Trabajador', 'Operaciones', 'Coordinador de Producción', 12, true),
(41, 'Fernando', 'Castillo', 'fernando.castillo@healthpredict.com', 'password123', '1991-10-03', 'Masculino', 1.79, 76.0, '2024-03-05', '2024-12-20 08:20:00', false, null, null, 'Trabajador', 'Operaciones', 'Analista de Calidad', 12, true),
(42, 'Carmen', 'Herrera', 'carmen.herrera@healthpredict.com', 'password123', '1989-12-14', 'Femenino', 1.66, 60.0, '2024-03-10', '2024-12-20 09:45:00', false, null, null, 'Trabajador', 'Operaciones', 'Supervisor de Logística', 12, true);

-- DATOS VITALES VARIADOS
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "TIPO_DATO", "VALOR", "UNIDAD", "DISPOSITIVO_ORIGEN", "NOTAS") VALUES
-- Diego Díaz (Estrés alto)
(20, '2024-12-19 08:00:00', 'Frecuencia Cardíaca', 95, 'bpm', 'Apple Watch', 'Medición matutina'),
(20, '2024-12-19 14:30:00', 'Frecuencia Cardíaca', 105, 'bpm', 'Apple Watch', 'Durante reunión técnica'),
(20, '2024-12-19 08:15:00', 'Presión Arterial Sistólica', 135, 'mmHg', 'Monitor Omron', 'Medición matutina'),
(20, '2024-12-19 08:15:00', 'Presión Arterial Diastólica', 90, 'mmHg', 'Monitor Omron', 'Medición matutina'),
(20, '2024-12-19 07:00:00', 'Horas de Sueño', 5.5, 'horas', 'Apple Watch', 'Pocas horas por deadline'),
(20, '2024-12-19 09:00:00', 'Nivel de Estrés', 8, 'escala 1-10', 'Autoevaluación', 'Alto por presión laboral'),
-- Iahn Vera (Valores normales)
(21, '2024-12-19 07:30:00', 'Frecuencia Cardíaca', 72, 'bpm', 'Fitbit', 'Frecuencia normal'),
(21, '2024-12-19 08:00:00', 'Presión Arterial Sistólica', 118, 'mmHg', 'Monitor Manual', 'Presión normal'),
(21, '2024-12-19 08:00:00', 'Presión Arterial Diastólica', 78, 'mmHg', 'Monitor Manual', 'Presión normal'),
(21, '2024-12-19 07:00:00', 'Horas de Sueño', 8.0, 'horas', 'Fitbit', 'Sueño reparador'),
(21, '2024-12-19 10:00:00', 'Nivel de Estrés', 3, 'escala 1-10', 'Autoevaluación', 'Bajo nivel de estrés'),
-- Matías Maripangue (Problemas de sueño)
(22, '2024-12-19 09:00:00', 'Frecuencia Cardíaca', 88, 'bpm', 'Samsung Galaxy Watch', 'Inicio del día'),
(22, '2024-12-19 08:30:00', 'Presión Arterial Sistólica', 125, 'mmHg', 'Monitor Digital', 'Ligeramente elevada'),
(22, '2024-12-19 06:30:00', 'Horas de Sueño', 4.5, 'horas', 'Samsung Galaxy Watch', 'Insomnio severo'),
(22, '2024-12-19 11:00:00', 'Nivel de Estrés', 7, 'escala 1-10', 'Autoevaluación', 'Preocupado por no dormir'),
-- Laura García (Estrés laboral)
(30, '2024-12-19 08:15:00', 'Frecuencia Cardíaca', 98, 'bpm', 'Apple Watch', 'Estrés por entrevistas'),
(30, '2024-12-19 08:30:00', 'Presión Arterial Sistólica', 140, 'mmHg', 'Monitor Digital', 'Elevada por estrés'),
(30, '2024-12-19 07:00:00', 'Horas de Sueño', 6.0, 'horas', 'Apple Watch', 'Preocupación laboral'),
(30, '2024-12-19 10:00:00', 'Nivel de Estrés', 8, 'escala 1-10', 'Autoevaluación', 'Período intenso trabajo');

-- ALERTAS DE RIESGO
INSERT INTO "ALERTAS" ("USUARIO_ID", "FECHA_CREACION", "TIPO_ALERTA", "DESCRIPCION", "SEVERIDAD", "LEIDA", "RESUELTA") VALUES
-- Diego Díaz - Alertas críticas
(20, '2024-12-19 08:30:00', 'Presión Arterial Alta', 'Presión arterial sistólica de 135 mmHg detectada. Combinado con pocas horas de sueño.', 'Crítica', false, false),
(20, '2024-12-19 07:15:00', 'Privación de Sueño', 'Solo 5.5 horas de sueño detectadas. Patrón recurrente.', 'Alta', false, false),
(20, '2024-12-19 09:15:00', 'Nivel de Estrés Elevado', 'Nivel de estrés reportado en 8/10.', 'Alta', true, false),
-- Matías Maripangue - Insomnio
(22, '2024-12-19 07:00:00', 'Insomnio Severo', 'Solo 4.5 horas de sueño registradas.', 'Crítica', false, false),
(22, '2024-12-19 11:30:00', 'Fatiga Crónica', 'Frecuencia cardíaca elevada en reposo.', 'Alta', true, false),
-- Laura García - Estrés laboral
(30, '2024-12-19 08:45:00', 'Hipertensión por Estrés', 'Presión arterial 140/92 mmHg junto con nivel de estrés 8/10.', 'Crítica', false, false),
(30, '2024-12-19 10:15:00', 'Sobrecarga Laboral', 'Indicadores múltiples de estrés laboral elevado.', 'Media', true, false);

-- Actualizar secuencias
SELECT setval('"USUARIOS_ID_seq"', 50); 