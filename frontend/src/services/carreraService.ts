import { apiFetch } from "../auth/authService";
import type { Carrera } from "../types/Carrera";

const API_URL = "http://localhost:5270/api/Carreras";

export interface CreateCarreraRequest {
  nombre: string;
  descripcion?: string | null;
  duracionAnios: number;
}

export interface UpdateCarreraRequest {
  nombre: string;
  descripcion?: string | null;
  duracionAnios: number;
  activa: boolean;
}

export async function getCarreras(): Promise<Carrera[]> {
  const response = await apiFetch(API_URL);

  if (!response.ok) {
    throw new Error("Error al obtener las carreras");
  }

  return await response.json();
}

export async function createCarrera(
  carrera: CreateCarreraRequest
): Promise<Carrera> {
  const response = await apiFetch(API_URL, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(carrera),
  });

  if (!response.ok) {
    const errorText = await response.text();
    console.error("Error backend:", errorText);

    throw new Error("Error al crear la carrera");
  }

  return await response.json();
}

export async function updateCarrera(
  id: string,
  carrera: UpdateCarreraRequest
): Promise<Carrera> {
  const response = await apiFetch(`${API_URL}/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(carrera),
  });

  if (!response.ok) {
    const errorText = await response.text();
    console.error("Error backend:", errorText);

    throw new Error("Error al actualizar la carrera");
  }

  return await response.json();
}

export async function deleteCarrera(id: string): Promise<void> {
  const response = await apiFetch(`${API_URL}/${id}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    const errorText = await response.text();
    console.error("Error backend:", errorText);

    throw new Error("Error al eliminar la carrera");
  }
}