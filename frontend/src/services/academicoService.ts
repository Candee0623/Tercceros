import { apiFetch } from "../auth/authService";
import type { AlumnoConsultaAsistencia, AsistenciaAlumno, Cursada, EstadoAsistenciaAlumno, HorarioCursada, Matricula, PlanillaAsistencia } from "../types/Academico";
const BASE="http://localhost:5270/api";
async function json<T>(r:Response):Promise<T>{if(!r.ok)throw new Error((await r.text())||"Error en la operación");return r.json();}
export interface SaveCursadaRequest{materiaId:string;profesorId?:string|null;cicloLectivo:number;periodo:string;fechaInicio:string;fechaFin:string;activa:boolean;porcentajePromocion:number;porcentajeRegularidad:number;horarios:HorarioCursada[]}
export const getCursadas=()=>apiFetch(`${BASE}/Cursadas`).then(json<Cursada[]>);
export const createCursada=(x:SaveCursadaRequest)=>apiFetch(`${BASE}/Cursadas`,{method:"POST",headers:{"Content-Type":"application/json"},body:JSON.stringify(x)}).then(json<Cursada>);
export const updateCursada=(id:string,x:SaveCursadaRequest)=>apiFetch(`${BASE}/Cursadas/${id}`,{method:"PUT",headers:{"Content-Type":"application/json"},body:JSON.stringify(x)}).then(json<Cursada>);
export const getMatriculas=(cursadaId:string)=>apiFetch(`${BASE}/Matriculaciones/cursada/${cursadaId}`).then(json<Matricula[]>);
export const matricular=(cursadaId:string,alumnoId:string)=>apiFetch(`${BASE}/Matriculaciones/cursada/${cursadaId}`,{method:"POST",headers:{"Content-Type":"application/json"},body:JSON.stringify({alumnoId})}).then(json<Matricula>);
export async function desmatricular(id:string){const r=await apiFetch(`${BASE}/Matriculaciones/${id}`,{method:"DELETE"});if(!r.ok)throw new Error((await r.text())||"No se pudo desmatricular");}
export const getPlanilla=(cursadaId:string,fecha:string)=>apiFetch(`${BASE}/Asistencias/planilla?cursadaId=${encodeURIComponent(cursadaId)}&fecha=${encodeURIComponent(fecha)}`).then(json<PlanillaAsistencia>);
export const guardarAsistencia=(x:{cursadaId:string;fecha:string;horasProgramadas:number;alumnos:AsistenciaAlumno[]})=>apiFetch(`${BASE}/Asistencias`,{method:"POST",headers:{"Content-Type":"application/json"},body:JSON.stringify(x)}).then(json<PlanillaAsistencia>);
export const getAlumnosConsultaAsistencia=()=>apiFetch(`${BASE}/EstadoAsistencia/alumnos`).then(json<AlumnoConsultaAsistencia[]>);
export const getEstadoAsistenciaAlumno=(alumnoId:string)=>apiFetch(`${BASE}/EstadoAsistencia/alumno/${alumnoId}`).then(json<EstadoAsistenciaAlumno>);
