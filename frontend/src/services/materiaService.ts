import { apiFetch } from "../auth/authService";
import type { Materia } from "../types/Materia";

const API_URL = "http://localhost:5270/api/Materias";

export interface CreateMateriaRequest {
  nombre: string;
  codigo: string;
  anio: number;
  carreraId: string;
  profesorId?: string | null;
  cantidadEvaluaciones: number;
  esPromocionable: boolean;
  notaPromocion?: number | null;
  notaRegularizacion: number;
}

export interface UpdateMateriaRequest {
  nombre: string;
  codigo: string;
  anio: number;
  activa: boolean;
  carreraId: string;
  profesorId?: string | null;
  cantidadEvaluaciones: number;
  esPromocionable: boolean;
  notaPromocion?: number | null;
  notaRegularizacion: number;
}

export async function getMaterias(): Promise<Materia[]> {
  const response = await apiFetch(API_URL);

  if (!response.ok) {
    throw new Error("Error al obtener las materias");
  }

  return await response.json();
}

export async function createMateria(
  materia: CreateMateriaRequest
): Promise<Materia> {
  const response = await apiFetch(API_URL, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(materia),
  });

  if (!response.ok) {
    const errorText = await response.text();
    console.error("Error backend:", errorText);

    throw new Error("Error al crear la materia");
  }

  return await response.json();
}

export async function updateMateria(
  id: string,
  materia: UpdateMateriaRequest
): Promise<Materia> {
  const response = await apiFetch(`${API_URL}/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(materia),
  });

  if (!response.ok) {
    const errorText = await response.text();
    console.error("Error backend:", errorText);

    throw new Error("Error al actualizar la materia");
  }

  return await response.json();
}

export async function deleteMateria(id: string): Promise<void> {
  const response = await apiFetch(`${API_URL}/${id}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    const errorText = await response.text();
    console.error("Error backend:", errorText);

    throw new Error("Error al eliminar la materia");
  }
}
