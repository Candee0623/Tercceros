import { apiFetch } from "../auth/authService";
import type { EstadoAsistenciaAlumno, Matricula } from "../types/Academico";
import type { MiCursadaDisponible, MiPerfilAlumno } from "../types/PortalAlumno";
async function json<T>(r:Response):Promise<T>{if(!r.ok)throw new Error((await r.text())||"Error en la operación");return r.json();}
export const getMisDatos=()=>apiFetch("/PortalAlumno/mis-datos").then(json<MiPerfilAlumno>);
export const getMiAsistencia=()=>apiFetch("/PortalAlumno/mi-asistencia").then(json<EstadoAsistenciaAlumno>);
export const getMisCursadas=()=>apiFetch("/PortalAlumno/cursadas").then(json<MiCursadaDisponible[]>);
export const matricularme=(cursadaId:string)=>apiFetch(`/PortalAlumno/matriculas/${cursadaId}`,{method:"POST"}).then(json<Matricula>);
export async function desmatricularme(matriculaId:string){const r=await apiFetch(`/PortalAlumno/matriculas/${matriculaId}`,{method:"DELETE"});if(!r.ok)throw new Error((await r.text())||"No se pudo cancelar la matrícula");}
