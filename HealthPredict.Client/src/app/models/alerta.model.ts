export interface Alerta {
  id?: number;
  usuarioId: number;
  fechaCreacion?: Date;
  tipoAlerta: string;
  descripcion: string;
  severidad: string;
  leida?: boolean;
  fechaLectura?: Date;
  resuelta?: boolean;
  fechaResolucion?: Date;
  notasResolucion?: string;
}