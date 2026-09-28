import { apiFetch } from "../auth/authService";

const API_URL = "/EventosCalendario";

export interface EventoCalendario {
  id: string;
  title: string;
  description?: string | null;
  startTime: string;
  endTime: string;
  isAllDay: boolean;
  location?: string | null;
  usuarioId: string;
}

export interface CrearEventoRequest {
  title: string;
  description?: string | null;
  startTime: string;
  endTime: string;
  isAllDay: boolean;
  location?: string | null;
}

async function obtenerError(response: Response, mensaje: string) {
  const texto = await response.text();
  return new Error(texto || mensaje);
}

export async function getEventos(): Promise<EventoCalendario[]> {
  const response = await apiFetch(API_URL);

  if (!response.ok) {
    throw await obtenerError(
      response,
      "Error al obtener los eventos"
    );
  }

  return response.json();
}

export async function getEvento(
  id: string
): Promise<EventoCalendario> {
  const response = await apiFetch(`${API_URL}/${id}`);

  if (!response.ok) {
    throw await obtenerError(
      response,
      "Error al obtener el evento"
    );
  }

  return response.json();
}

export async function createEvento(
  evento: CrearEventoRequest
): Promise<EventoCalendario> {
  const response = await apiFetch(API_URL, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(evento),
  });

  if (!response.ok) {
    throw await obtenerError(
      response,
      "Error al crear el evento"
    );
  }

  return response.json();
}

export async function updateEvento(
  id: string,
  evento: CrearEventoRequest
): Promise<EventoCalendario> {
  const response = await apiFetch(`${API_URL}/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(evento),
  });

  if (!response.ok) {
    throw await obtenerError(
      response,
      "Error al actualizar el evento"
    );
  }

  return response.json();
}

export async function deleteEvento(
  id: string
): Promise<void> {
  const response = await apiFetch(`${API_URL}/${id}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    throw await obtenerError(
      response,
      "Error al eliminar el evento"
    );
  }
}