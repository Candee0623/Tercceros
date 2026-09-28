import { apiFetch } from "../auth/authService";
import type { PlanEstudioCarrera } from "../types/PlanEstudio";

const API_URL = "http://localhost:5270/api/PlanEstudios";

export async function getPlanEstudios(): Promise<PlanEstudioCarrera[]> {
  const response = await apiFetch(API_URL);

  if (!response.ok) {
    throw new Error("Error al obtener el plan de estudios");
  }

  return await response.json();
}
