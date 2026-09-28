import { useEffect, useState } from "react";
import { getCursadas } from "../services/academicoService";
import { generarEvaluaciones, getPlanillaCalificaciones, guardarCalificaciones } from "../services/calificacionService";
import type { Cursada } from "../types/Academico";
import type { PlanillaCalificaciones } from "../types/Calificaciones";

export default function CalificacionesPage(){
 const [cursadas,setCursadas]=useState<Cursada[]>([]),[cursadaId,setCursadaId]=useState(""),[p,setP]=useState<PlanillaCalificaciones|null>(null),[error,setError]=useState(""),[saving,setSaving]=useState(false);
 useEffect(()=>{getCursadas().then(x=>setCursadas(x.filter(c=>c.activa))).catch(e=>setError(e.message))},[]);
 async function load(id:string){setCursadaId(id);setP(null);if(!id)return;try{setError("");setP(await getPlanillaCalificaciones(id))}catch(e){setError(e instanceof Error?e.message:"Error")}}
 function setNota(mi:number,ei:number,v:string){if(!p)return;const cp=structuredClone(p);cp.alumnos[mi].notas[ei].nota=v===""?null:Number(v);setP(cp)}
 async function generar(){if(!cursadaId)return;try{setP(await generarEvaluaciones(cursadaId))}catch(e){setError(e instanceof Error?e.message:"Error")}}
 async function save(){if(!p)return;try{setSaving(true);setError("");const notas=p.alumnos.flatMap(a=>a.cuotaAlDia?a.notas.map(n=>({matriculaId:a.matriculaId,evaluacionId:n.evaluacionId,nota:n.nota??null,observacion:n.observacion??null})):[]);setP(await guardarCalificaciones(p.cursadaId,notas))}catch(e){setError(e instanceof Error?e.message:"Error")}finally{setSaving(false)}}
 const estado=(x:string)=>x==="PROMOCIONA"?"Promociona":x==="REGULAR"?"Regular":x==="NO_REGULARIZA"?"No regulariza":x==="BLOQUEADO_CUOTA"?"Bloqueado por cuota":"En curso";
 return <div><div className="page-header"><h1>Calificaciones</h1><p>Carga de evaluaciones y condición académica por cursada.</p></div>{error&&<div className="alert-error">{error}</div>}
 <div className="card"><div className="form-group"><label>Cursada</label><select value={cursadaId} onChange={e=>load(e.target.value)}><option value="">Seleccionar cursada</option>{cursadas.map(c=><option key={c.id} value={c.id}>{c.materiaNombre} · {c.cicloLectivo} · {c.periodo}</option>)}</select></div></div>
 {p&&<div className="card"><div className="grade-header"><div><h3>{p.cursadaNombre}</h3><p>{p.esPromocionable?`Promoción: ${p.notaPromocion}`:"No promocionable"} · Regularidad: {p.notaRegularizacion}</p></div><button className="btn btn-secondary" onClick={generar}>Generar/actualizar {p.cantidadEvaluaciones} evaluaciones</button></div>
 {p.evaluaciones.length===0?<p>No hay instancias generadas todavía.</p>:<div className="table-container"><table><thead><tr><th>Alumno</th><th>Cuota</th>{p.evaluaciones.map(e=><th key={e.id}>{e.nombre}</th>)}<th>Promedio</th><th>Condición</th></tr></thead><tbody>{p.alumnos.map((a,mi)=><tr key={a.matriculaId}><td>{a.alumnoNombre}<small className="table-sub">{a.alumnoDni}</small></td><td><span className={a.cuotaAlDia?"status status-active":"status status-inactive"}>{a.cuotaAlDia?"Al día":"Con deuda"}</span></td>{a.notas.map((n,ei)=><td key={n.evaluacionId}><input className="grade-input" type="number" min="0" max="10" step="0.01" disabled={!a.cuotaAlDia} value={n.nota??""} onChange={e=>setNota(mi,ei,e.target.value)} title={!a.cuotaAlDia?"Secretaría debe marcar la cuota al día":""}/></td>)}<td>{a.promedio??"-"}</td><td>{estado(a.condicion)}</td></tr>)}</tbody></table></div>}
 <div className="form-actions"><button className="btn btn-primary" disabled={saving||p.evaluaciones.length===0} onClick={save}>{saving?"Guardando...":"Guardar calificaciones"}</button></div></div>}</div>
}
