import { useEffect, useMemo, useState } from "react";
import { desmatricularme, getMisCursadas, matricularme } from "../services/portalAlumnoService";
import type { MiCursadaDisponible } from "../types/PortalAlumno";

function fecha(x:string){return new Date(x).toLocaleDateString();}

export default function AutoMatriculacionPage(){
 const [cursadas,setCursadas]=useState<MiCursadaDisponible[]>([]),[buscar,setBuscar]=useState(""),[error,setError]=useState(""),[loading,setLoading]=useState(false);
 async function load(){try{setError("");setCursadas(await getMisCursadas())}catch(e){setError(e instanceof Error?e.message:"Error")}}
 useEffect(()=>{load()},[]);
 const filtradas=useMemo(()=>cursadas.filter(c=>`${c.materia} ${c.profesor??""} ${c.periodo} ${c.cicloLectivo}`.toLowerCase().includes(buscar.toLowerCase())),[cursadas,buscar]);
 async function inscribir(c:MiCursadaDisponible){if(!confirm(`¿Matricularte en ${c.materia}?`))return;try{setLoading(true);await matricularme(c.cursadaId);await load()}catch(e){setError(e instanceof Error?e.message:"Error")}finally{setLoading(false)}}
 async function baja(c:MiCursadaDisponible){if(!c.matriculaId)return;if(!confirm(`¿Cancelar tu matrícula en ${c.materia}?`))return;try{setLoading(true);await desmatricularme(c.matriculaId);await load()}catch(e){setError(e instanceof Error?e.message:"Error")}finally{setLoading(false)}}
 return <div><div className="page-header"><h1>Inscripción a materias</h1><p>Consultá las cursadas disponibles y matriculate directamente desde tu cuenta.</p></div>{error&&<div className="alert-error">{error}</div>}
 <div className="card"><div className="form-group"><label>Buscar materia</label><input value={buscar} onChange={e=>setBuscar(e.target.value)} placeholder="Materia, profesor, período..."/></div></div>
 <div className="course-enrollment-grid">{filtradas.map(c=><div className={`card enrollment-card ${c.matriculado?"enrolled":""}`} key={c.cursadaId}><div className="enrollment-head"><div><h3>{c.materia}</h3><p>{c.cicloLectivo} · {c.periodo}</p></div>{c.matriculado&&<span className="status-pill status-promociona">MATRICULADO</span>}</div><div className="enrollment-details"><p><strong>Profesor:</strong> {c.profesor??"A confirmar"}</p><p><strong>Cursada:</strong> {fecha(c.fechaInicio)} al {fecha(c.fechaFin)}</p><p><strong>Carga:</strong> {c.horasSemanales} hs semanales · {c.horasTotalesPlanificadas} hs totales</p><p><strong>Condición:</strong> promoción {c.porcentajePromocion}% · regularidad {c.porcentajeRegularidad}%</p></div><div className="schedule-chips">{c.horarios.map((h,i)=><span key={`${h.diaSemana}-${i}`}>{h.dia} {h.horaInicio}–{h.horaFin}</span>)}</div>
 {c.matriculado?<div className="enrollment-actions"><span className="muted">Inscripto {c.fechaMatriculacion?fecha(c.fechaMatriculacion):""}</span>{c.puedeDesmatricularse?<button className="btn btn-danger btn-small" disabled={loading} onClick={()=>baja(c)}>Cancelar matrícula</button>:<span className="muted">La baja requiere administración porque ya existe asistencia.</span>}</div>:<button className="btn btn-primary" disabled={loading} onClick={()=>inscribir(c)}>Matricularme</button>}</div>)}</div>
 {filtradas.length===0&&<div className="card">No hay cursadas disponibles que coincidan con la búsqueda.</div>}</div>
}
