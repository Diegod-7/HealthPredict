export interface DatoVital {
  id?: number;
  usuarioId: number;
  fechaRegistro?: Date;
  tipoDato: string;
  valor: number;
  unidad: string;
  dispositivoOrigen?: string;
  notas?: string;
} 