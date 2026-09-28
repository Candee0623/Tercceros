import { apiFetch } from "../auth/authService";
import type { PlanillaCalificaciones } from "../types/Calificaciones";
const API="http://localhost:5270/api/Calificaciones";
async function parse(r:Response){if(!r.ok)throw new Error((await r.text())||"Error en calificaciones");return r.json()}
export async function getPlanillaCalificaciones(cursadaId:string):Promise<PlanillaCalificaciones>{return parse(await apiFetch(`${API}/planilla?cursadaId=${cursadaId}`))}
export async function generarEvaluaciones(cursadaId:string):Promise<PlanillaCalificaciones>{return parse(await apiFetch(`${API}/generar/${cursadaId}`,{method:"POST"}))}
export async function guardarCalificaciones(cursadaId:string, notas:{matriculaId:string;evaluacionId:string;nota:number|null;observacion?:string|null}[]):Promise<PlanillaCalificaciones>{return parse(await apiFetch(API,{method:"POST",headers:{"Content-Type":"application/json"},body:JSON.stringify({cursadaId,notas})}))}
