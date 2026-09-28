import type { EstadoAsistenciaAlumno, Matricula } from "./Academico";

export interface MiPerfilAlumno {
  alumnoId:string; nombre:string; apellido:string; dni:string; email?:string|null; telefono?:string|null;
  fechaNacimiento:string; fechaIngreso:string; carreraNombre?:string|null;
}
export interface MiHorarioCursada { diaSemana:number; dia:string; horaInicio:string; horaFin:string; }
export interface MiCursadaDisponible {
  cursadaId:string; materiaId:string; materia:string; profesor?:string|null; cicloLectivo:number; periodo:string;
  fechaInicio:string; fechaFin:string; horasSemanales:number; horasTotalesPlanificadas:number;
  porcentajePromocion:number; porcentajeRegularidad:number; matriculado:boolean; matriculaId?:string|null;
  fechaMatriculacion?:string|null; puedeDesmatricularse:boolean; horarios:MiHorarioCursada[];
}
export type { EstadoAsistenciaAlumno, Matricula };
