import { apiFetch } from "../auth/authService";
import type { Alumno } from "../types/Alumno";

const API_URL = "http://localhost:5270/api/Alumnos";

export interface CreateAlumnoRequest {
  nombre: string;
  apellido: string;
  dni: string;
  email?: string | null;
  telefono?: string | null;
  fechaNacimiento: string;
  carreraId?: string | null;
}

export async function getAlumnos(): Promise<Alumno[]> {
  const response = await apiFetch(API_URL);

  if (!response.ok) {
    throw new Error("Error al obtener los alumnos");
  }

  return await response.json();
}

export async function createAlumno(
  alumno: CreateAlumnoRequest
): Promise<Alumno> {
  const response = await apiFetch(API_URL, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(alumno),
  });

  if (!response.ok) {
    const errorText = await response.text();

    console.error("Error backend:", errorText);

    throw new Error(errorText || "Error al crear el alumno");
  }

  return await response.json();
}


export async function deleteAlumno(id: string): Promise<void> {
  const response = await apiFetch(`${API_URL}/${id}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    const errorText = await response.text();

    console.error("Error backend:", errorText);

    throw new Error("Error al eliminar el alumno");
  }
}

export interface UpdateAlumnoRequest {
  nombre: string;
  apellido: string;
  dni: string;
  email?: string | null;
  telefono?: string | null;
  fechaNacimiento: string;
  carreraId?: string | null;
  activo: boolean;
}

export async function updateAlumno(
  id: string,
  alumno: UpdateAlumnoRequest
): Promise<Alumno> {
  const response = await apiFetch(`${API_URL}/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(alumno),
  });

  if (!response.ok) {
    const errorText = await response.text();

    console.error("Error backend:", errorText);

    throw new Error("Error al actualizar el alumno");
  }

  return await response.json();
}