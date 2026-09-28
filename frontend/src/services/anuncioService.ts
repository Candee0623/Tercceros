import { apiFetch } from "../auth/authService";
import type {
  ActualizarAnuncioRequest,
  Anuncio,
  CrearAnuncioRequest,
} from "../types/Anuncio";

export async function getAnuncios(todos = false): Promise<Anuncio[]> {
  const res = await apiFetch(`/Anuncios?todos=${todos}`);
  if (!res.ok) throw new Error(await res.text() || "Error al cargar anuncios");
  return await res.json();
}

export async function crearAnuncio(dto: CrearAnuncioRequest): Promise<Anuncio> {
  const res = await apiFetch("/Anuncios", {
    method: "POST",
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error(await res.text() || "Error al publicar anuncio");
  return await res.json();
}

export async function actualizarAnuncio(
  id: string,
  dto: ActualizarAnuncioRequest
): Promise<Anuncio> {
  const res = await apiFetch(`/Anuncios/${id}`, {
    method: "PUT",
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error(await res.text() || "Error al modificar anuncio");
  return await res.json();
}

export async function eliminarAnuncio(id: string): Promise<void> {
  const res = await apiFetch(`/Anuncios/${id}`, {
    method: "DELETE",
  });
  if (!res.ok) throw new Error(await res.text() || "Error al eliminar anuncio");
}
