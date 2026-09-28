export interface Carrera {
  id: string;
  nombre: string;
  descripcion?: string | null;
  duracionAnios: number;
  activa: boolean;
  fechaCreacion: string;
}