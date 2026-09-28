export interface Tramite {
  id: string;
  codigo: string;
  tipo: string;
  motivo: string;
  estado: "Pendiente" | "Aprobado" | "Rechazado" | "Entregado" | string;
  fechaSolicitud: string;
  fechaResolucion?: string | null;
  observacionResolucion?: string | null;
  usuarioId: string;
  solicitanteNombre: string;
  alumnoDni?: string | null;
}

export interface CrearTramiteRequest {
  tipo: string;
  motivo: string;
}

export interface ResolverTramiteRequest {
  estado: string;
  observacion?: string;
}
