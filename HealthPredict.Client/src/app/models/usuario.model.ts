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
} 