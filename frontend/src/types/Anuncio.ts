export interface Anuncio {
  id: string;
  titulo: string;
  contenido: string;
  categoria: string;
  prioridad: "Baja" | "Normal" | "Alta" | "Urgente" | string;
  fechaPublicacion: string;
  fechaVencimiento?: string | null;
  activo: boolean;
  usuarioId: string;
  autorNombre: string;
}

export interface CrearAnuncioRequest {
  titulo: string;
  contenido: string;
  categoria: string;
  prioridad: string;
  fechaVencimiento?: string | null;
}

export interface ActualizarAnuncioRequest {
  titulo: string;
  contenido: string;
  categoria: string;
  prioridad: string;
  fechaVencimiento?: string | null;
  activo: boolean;
}
