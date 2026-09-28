export interface EvaluacionCalificacion { id:string; numero:number; nombre:string; fecha?:string|null }
export interface NotaEvaluacion { evaluacionId:string; nota?:number|null; observacion?:string|null }
export interface AlumnoCalificacion { matriculaId:string; alumnoId:string; alumnoNombre:string; alumnoDni:string; cuotaAlDia:boolean; promedio?:number|null; condicion:string; notas:NotaEvaluacion[] }
export interface PlanillaCalificaciones { cursadaId:string; cursadaNombre:string; esPromocionable:boolean; notaPromocion?:number|null; notaRegularizacion:number; cantidadEvaluaciones:number; evaluaciones:EvaluacionCalificacion[]; alumnos:AlumnoCalificacion[] }
export interface EstadoCuota { alumnoId:string; alumnoNombre:string; alumnoDni:string; alDia:boolean; registrado:boolean; ultimoPeriodoPagado?:string|null; fechaUltimoPago?:string|null; importeUltimoPago?:number|null; observacion?:string|null; fechaActualizacion?:string|null }
