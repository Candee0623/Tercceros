export interface PlanEstudioMateria {
  id: string;
  nombre: string;
  codigo: string;
  activa: boolean;
  profesorNombre?: string | null;
}

export interface PlanEstudioAnio {
  anio: number;
  materias: PlanEstudioMateria[];
}

export interface PlanEstudioCarrera {
  carreraId: string;
  carreraNombre: string;
  duracionAnios: number;
  carreraActiva: boolean;
  anios: PlanEstudioAnio[];
}
