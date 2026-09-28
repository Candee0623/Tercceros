export interface Materia {
  id: string;
  nombre: string;
  codigo: string;
  anio: number;
  activa: boolean;
  carreraId: string;
  carreraNombre: string;
  profesorId?: string | null;
  profesorNombre?: string | null;
  cantidadEvaluaciones: number;
  esPromocionable: boolean;
  notaPromocion?: number | null;
  notaRegularizacion: number;
}
