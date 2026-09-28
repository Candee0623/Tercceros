import { apiFetch } from "../auth/authService";

export interface Usuario {
  id: string;
  nombreUsuario: string;
  nombre: string;
  apellido: string;
  activo: boolean;
  rolId: string;
  rolNombre: string;

  alumnoId?: string | null;
  alumnoNombre?: string | null;

  profesorId?: string | null;
  profesorNombre?: string | null;
}

export interface UsuarioRolOption {
  id: string;
  nombre: string;
}

export interface UsuarioAlumnoOption {
  id: string;
  nombreCompleto: string;
  dni: string;
}

export interface UsuarioProfesorOption {
  id: string;
  nombreCompleto: string;
  dni: string;
}

export interface CreateUsuario {
  nombreUsuario: string;
  nombre: string;
  apellido: string;
  password: string;
  rolId: string;
  alumnoId?: string | null;
  profesorId?: string | null;
  activo: boolean;
}

export interface UpdateUsuario {
  nombreUsuario: string;
  nombre: string;
  apellido: string;
  password?: string;
  rolId: string;
  alumnoId?: string | null;
  profesorId?: string | null;
  activo: boolean;
}

async function leerRespuesta<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const texto = await response.text();

    throw new Error(
      texto || `Error ${response.status} al comunicarse con el servidor.`
    );
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return await response.json() as T;
}

export async function getUsuarios(): Promise<Usuario[]> {
  const response = await apiFetch("/Usuarios");
  return leerRespuesta<Usuario[]>(response);
}

export async function getRolesUsuario(): Promise<UsuarioRolOption[]> {
  const response = await apiFetch("/Usuarios/roles");
  return leerRespuesta<UsuarioRolOption[]>(response);
}

export async function getAlumnosUsuario(
  includeAlumnoId?: string | null
): Promise<UsuarioAlumnoOption[]> {
  const query = includeAlumnoId
    ? `?includeAlumnoId=${encodeURIComponent(includeAlumnoId)}`
    : "";

  const response = await apiFetch(`/Usuarios/alumnos${query}`);

  return leerRespuesta<UsuarioAlumnoOption[]>(response);
}

export async function getProfesoresUsuario(
  includeProfesorId?: string | null
): Promise<UsuarioProfesorOption[]> {
  const query = includeProfesorId
    ? `?includeProfesorId=${encodeURIComponent(includeProfesorId)}`
    : "";

  const response = await apiFetch(`/Usuarios/profesores${query}`);

  return leerRespuesta<UsuarioProfesorOption[]>(response);
}

export async function crearUsuario(
  data: CreateUsuario
): Promise<Usuario> {
  const response = await apiFetch("/Usuarios", {
    method: "POST",
    body: JSON.stringify(data),
  });

  return leerRespuesta<Usuario>(response);
}

export async function actualizarUsuario(
  id: string,
  data: UpdateUsuario
): Promise<Usuario> {
  const response = await apiFetch(`/Usuarios/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

  return leerRespuesta<Usuario>(response);
}
