import { apiFetch } from "../auth/authService";
import type { Profesor } from "../types/Profesor";

const API_URL = "http://localhost:5270/api/Profesores";

export interface CreateProfesorRequest {
  nombre: string;
  apellido: string;
  dni: string;
  email?: string | null;
  telefono?: string | null;
  titulo?: string | null;
}

export interface UpdateProfesorRequest {
  nombre: string;
  apellido: string;
  dni: string;
  email?: string | null;
  telefono?: string | null;
  titulo?: string | null;
  activo: boolean;
}

export async function getProfesores(): Promise<Profesor[]> {
  const response = await apiFetch(API_URL);

  if (!response.ok) {
    throw new Error("Error al obtener los profesores");
  }

  return await response.json();
}

export async function createProfesor(
  profesor: CreateProfesorRequest
): Promise<Profesor> {
  const response = await apiFetch(API_URL, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(profesor),
  });

  if (!response.ok) {
    const errorText = await response.text();
    console.error("Error backend:", errorText);

    throw new Error("Error al crear el profesor");
  }

  return await response.json();
}

export async function updateProfesor(
  id: string,
  profesor: UpdateProfesorRequest
): Promise<Profesor> {
  const response = await apiFetch(`${API_URL}/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(profesor),
  });

  if (!response.ok) {
    const errorText = await response.text();
    console.error("Error backend:", errorText);

    throw new Error("Error al actualizar el profesor");
  }

  return await response.json();
}

export async function deleteProfesor(id: string): Promise<void> {
  const response = await apiFetch(`${API_URL}/${id}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    const errorText = await response.text();
    console.error("Error backend:", errorText);

    throw new Error("Error al eliminar el profesor");
  }
}