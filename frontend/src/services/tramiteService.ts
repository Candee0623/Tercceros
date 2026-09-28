import { apiFetch } from "../auth/authService";
import type {
  CrearTramiteRequest,
  ResolverTramiteRequest,
  Tramite,
} from "../types/Tramite";

export async function getMisTramites(): Promise<Tramite[]> {
  const res = await apiFetch("/Tramites/mis-tramites");
  if (!res.ok) throw new Error(await res.text() || "Error al cargar mis trámites");
  return await res.json();
}

export async function getTodosTramites(): Promise<Tramite[]> {
  const res = await apiFetch("/Tramites");
  if (!res.ok) throw new Error(await res.text() || "Error al cargar trámites");
  return await res.json();
}

export async function crearTramite(dto: CrearTramiteRequest): Promise<Tramite> {
  const res = await apiFetch("/Tramites", {
    method: "POST",
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error(await res.text() || "Error al iniciar solicitud de trámite");
  return await res.json();
}

export async function resolverTramite(
  id: string,
  dto: ResolverTramiteRequest
): Promise<Tramite> {
  const res = await apiFetch(`/Tramites/${id}/resolver`, {
    method: "PATCH",
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error(await res.text() || "Error al resolver trámite");
  return await res.json();
}

export async function cancelarTramite(id: string): Promise<void> {
  const res = await apiFetch(`/Tramites/${id}`, {
    method: "DELETE",
  });
  if (!res.ok) throw new Error(await res.text() || "Error al cancelar trámite");
}
