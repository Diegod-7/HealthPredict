export interface Usuario {
  id?: number;
  nombre: string;
  apellido: string;
  email: string;
  password?: string;
  fechaNacimiento: Date;
  genero: string;
  altura: number;
  peso: number;
  fechaRegistro?: Date;
  ultimoAcceso?: Date;
  esProfesionalMedico: boolean;
  especialidad?: string;
  numeroLicencia?: string;

  // ✅ PROPIEDADES DEL SISTEMA DE ROLES
  rol: string; // "Jefe" o "Trabajador"
  departamento?: string;
  cargo?: string;
  jefeId?: number;
  esActivo: boolean;

  // Relaciones
  jefe?: Usuario;
  subordinados?: Usuario[];

  // ✅ PROPIEDADES CALCULADAS (opcional, se pueden manejar en el frontend)
  nombreCompleto?: string;
  esJefe?: boolean;
  esTrabajador?: boolean;
} 