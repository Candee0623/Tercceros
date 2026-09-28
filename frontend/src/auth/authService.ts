export interface Session {
  token: string;
  usuarioId: string;
  nombreUsuario: string;
  nombreCompleto: string;
  rol: string;
  alumnoId?: string | null;
  permisos: string[];
}

const KEY = "tercceros_session";
export const API_BASE = "http://localhost:5270/api";

export function getSession(): Session | null {
  const raw = localStorage.getItem(KEY);
  if (!raw) return null;
  try { return JSON.parse(raw) as Session; } catch { return null; }
}

export function saveSession(session: Session) { localStorage.setItem(KEY, JSON.stringify(session)); }
export function clearSession() { localStorage.removeItem(KEY); }
export function hasPermission(screen: string) { return getSession()?.permisos.includes(screen) ?? false; }

export async function login(nombreUsuario: string, password: string): Promise<Session> {
  const response = await fetch(`${API_BASE}/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ nombreUsuario, password }),
  });
  if (!response.ok) throw new Error((await response.text()) || "Usuario o contraseña incorrectos.");
  const session = await response.json() as Session;
  saveSession(session);
  return session;
}

export async function apiFetch(path: string, init: RequestInit = {}) {
  const session = getSession();
  const headers = new Headers(init.headers);
  if (!headers.has("Content-Type") && init.body) headers.set("Content-Type", "application/json");
  if (session?.token) headers.set("Authorization", `Bearer ${session.token}`);
  return fetch(path.startsWith("http") ? path : `${API_BASE}${path}`, { ...init, headers });
}
