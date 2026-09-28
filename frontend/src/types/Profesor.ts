export interface Profesor {
  id: string;
  nombre: string;
  apellido: string;
  dni: string;
  email?: string | null;
  telefono?: string | null;
  titulo?: string | null;
  activo: boolean;
  fechaCreacion: string;
}