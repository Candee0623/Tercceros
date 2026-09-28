export interface HorarioCursada { id?: string; diaSemana:number; horaInicio:string; horaFin:string; }
export interface Cursada { id:string; materiaId:string; materiaNombre:string; profesorId?:string|null; profesorNombre?:string|null; cicloLectivo:number; periodo:string; fechaInicio:string; fechaFin:string; activa:boolean; porcentajePromocion:number; porcentajeRegularidad:number; porcentajeLibre:number; horasSemanales:number; horasTotalesPlanificadas:number; horarios:HorarioCursada[]; }
export interface Matricula { id:string; alumnoId:string; alumnoNombre:string; alumnoDni:string; cursadaId:string; fechaMatriculacion:string; activa:boolean; }
export type EstadoAsistencia="PRESENTE"|"AUSENCIA_TOTAL"|"AUSENCIA_PARCIAL";
export interface AsistenciaAlumno { matriculaId:string; alumnoId:string; alumnoNombre:string; alumnoDni:string; estado:EstadoAsistencia; horasAusente:number; observacion?:string|null; }
export interface PlanillaAsistencia { claseId?:string|null; cursadaId:string; fecha:string; horasProgramadas:number; alumnos:AsistenciaAlumno[]; }
export interface AlumnoConsultaAsistencia { id:string; nombreCompleto:string; dni:string; }
export interface EstadoAsistenciaCursada { matriculaId:string; cursadaId:string; materia:string; periodo:string; cicloLectivo:number; profesor?:string|null; horasTotalesPlanificadas:number; horasAusentes:number; horasAsistidas:number; porcentajeAsistencia:number; porcentajePromocion:number; porcentajeRegularidad:number; porcentajeLibre:number; condicion:"PROMOCIONA"|"REGULAR"|"LIBRE"; }
export interface EstadoAsistenciaAlumno { alumnoId:string; alumnoNombre:string; dni:string; cursadas:EstadoAsistenciaCursada[]; }
