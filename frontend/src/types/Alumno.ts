export interface Alumno {
  id: string;
  nombre: string;
  apellido: string;
  dni: string;
  email?: string | null;
  telefono?: string | null;
  fechaNacimiento: string;
  fechaIngreso: string;
  activo: boolean;
  carreraId?: string | null;
  carreraNombre?: string | null;
}