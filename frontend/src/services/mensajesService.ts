import { apiFetch } from "../auth/authService";

export interface MensajeBandeja {
id: string;
asunto: string;
contenido: string;
fechaEnvio: string;
remitente: string;
leido: boolean;
}

export interface MensajeEnviado {
id: string;
asunto: string;
contenido: string;
fechaEnvio: string;
tipoDestinatario: string;
destinatarioDescripcion: string;
cantidadDestinatarios: number;
}

export interface MensajeCursada {
id: string;
materiaNombre: string;
cicloLectivo: number;
periodo: string;
}

export interface CrearMensaje {
asunto: string;
contenido: string;
tipoDestinatario: number;
carreraId?: string | null;
anio?: number | null;
cursadaId?: string | null;
}

async function leerRespuesta<T>(response: Response): Promise<T> {
if (!response.ok) {
const texto = await response.text();


throw new Error(
  texto || "Ocurrió un error al comunicarse con el servidor."
);


}

if (response.status === 204) {
return undefined as T;
}

return (await response.json()) as T;
}

export async function enviarMensaje(
mensaje: CrearMensaje
): Promise<MensajeEnviado> {
const response = await apiFetch("/Mensajes", {
method: "POST",
body: JSON.stringify(mensaje),
});

return leerRespuesta<MensajeEnviado>(response);
}

export async function obtenerBandeja(): Promise<MensajeBandeja[]> {
const response = await apiFetch("/Mensajes/bandeja");

return leerRespuesta<MensajeBandeja[]>(response);
}

export async function obtenerNoLeidos(): Promise<number> {
const response = await apiFetch("/Mensajes/no-leidos");

const data = await leerRespuesta<{ cantidad: number }>(response);

return data.cantidad;
}

export async function obtenerDetalle(
id: string
): Promise<MensajeBandeja> {
const response = await apiFetch(`/Mensajes/${id}`);

return leerRespuesta<MensajeBandeja>(response);
}

export async function marcarComoLeido(id: string): Promise<void> {
const response = await apiFetch(`/Mensajes/${id}/leido`, {
method: "PUT",
});

await leerRespuesta<void>(response);
}

export async function obtenerEnviados(): Promise<MensajeEnviado[]> {
const response = await apiFetch("/Mensajes/enviados");

return leerRespuesta<MensajeEnviado[]>(response);
}

export async function obtenerCursadasProfesor(): Promise<MensajeCursada[]> {
const response = await apiFetch("/Mensajes/cursadas-profesor");

return leerRespuesta<MensajeCursada[]>(response);
}

export async function obtenerCursadasDisponibles(): Promise<MensajeCursada[]> {
const response = await apiFetch("/Mensajes/cursadas");

return leerRespuesta<MensajeCursada[]>(response);
}
