import { useEffect, useState } from "react";
import { getMiAsistencia } from "../services/portalAlumnoService";
import type { EstadoAsistenciaAlumno } from "../types/Academico";

export default function MiAsistenciaPage(){
 const [estado,setEstado]=useState<EstadoAsistenciaAlumno|null>(null),[error,setError]=useState("");
 useEffect(()=>{getMiAsistencia().then(setEstado).catch(e=>setError(e instanceof Error?e.message:"Error"))},[]);
 return <div><div className="page-header"><h1>Mi asistencia</h1><p>Seguimiento de tu asistencia sobre las horas totales planificadas de cada cursada.</p></div>{error&&<div className="alert-error">{error}</div>}
 {!estado&&!error&&<div className="card">Cargando...</div>}
 {estado&&estado.cursadas.length===0&&<div className="card">Todavía no tenés materias matriculadas.</div>}
 {estado&&estado.cursadas.length>0&&<div className="attendance-cards">{estado.cursadas.map(c=>{
   const faltan=Math.max(0,c.porcentajePromocion-c.porcentajeAsistencia);
   const margen=Math.max(0,c.porcentajeAsistencia-c.porcentajeRegularidad);
   return <div className="card attendance-status-card" key={c.matriculaId}><div className="attendance-card-head"><div><h3>{c.materia}</h3><p>{c.cicloLectivo} · {c.periodo}{c.profesor?` · ${c.profesor}`:""}</p></div><span className={`status-pill status-${c.condicion.toLowerCase()}`}>{c.condicion}</span></div><div className="attendance-percent">{c.porcentajeAsistencia.toFixed(2)}%</div><div className="attendance-progress"><div style={{width:`${Math.max(0,Math.min(100,c.porcentajeAsistencia))}%`}}/></div><div className="attendance-metrics"><div><strong>{c.horasAsistidas}</strong><span>Hs. asistidas</span></div><div><strong>{c.horasAusentes}</strong><span>Hs. ausentes</span></div><div><strong>{c.horasTotalesPlanificadas}</strong><span>Hs. totales</span></div></div>
   <div className="student-attendance-message">{c.condicion==="PROMOCIONA"?<>Estás dentro del porcentaje de <strong>promoción</strong>.</>:c.condicion==="REGULAR"?<>Te faltan <strong>{faltan.toFixed(2)} puntos</strong> para promocionar. Tenés un margen de <strong>{margen.toFixed(2)} puntos</strong> antes de quedar libre.</>:<>Tu asistencia está por debajo del mínimo de regularidad. Consultá con el instituto.</>}</div>
   <div className="attendance-thresholds"><span>Promoción ≥ <strong>{c.porcentajePromocion}%</strong></span><span>Regularidad ≥ <strong>{c.porcentajeRegularidad}%</strong></span><span>Libre &lt; <strong>{c.porcentajeLibre}%</strong></span></div></div>})}</div>}
 </div>
}
