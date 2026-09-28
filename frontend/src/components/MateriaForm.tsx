import { useEffect, useState } from "react";
import type { Materia } from "../types/Materia";
import type { Carrera } from "../types/Carrera";
import type { Profesor } from "../types/Profesor";
import { createMateria, updateMateria, type CreateMateriaRequest, type UpdateMateriaRequest } from "../services/materiaService";
import { getCarreras } from "../services/carreraService";
import { getProfesores } from "../services/profesorService";

interface Props { materiaEditar?: Materia | null; onGuardado: () => void; onCancelarEdicion: () => void; }

export default function MateriaForm({ materiaEditar, onGuardado, onCancelarEdicion }: Props) {
  const [nombre,setNombre]=useState(""),[codigo,setCodigo]=useState(""),[anio,setAnio]=useState(1),[carreraId,setCarreraId]=useState(""),[profesorId,setProfesorId]=useState(""),[activa,setActiva]=useState(true);
  const [cantidadEvaluaciones,setCantidadEvaluaciones]=useState(2),[esPromocionable,setEsPromocionable]=useState(true),[notaPromocion,setNotaPromocion]=useState(7),[notaRegularizacion,setNotaRegularizacion]=useState(4);
  const [carreras,setCarreras]=useState<Carrera[]>([]),[profesores,setProfesores]=useState<Profesor[]>([]),[cargandoOpciones,setCargandoOpciones]=useState(true);

  useEffect(()=>{(async()=>{try{setCargandoOpciones(true);const [c,p]=await Promise.all([getCarreras(),getProfesores()]);setCarreras(c.filter(x=>x.activa));setProfesores(p.filter(x=>x.activo));}finally{setCargandoOpciones(false)}})()},[]);
  useEffect(()=>{if(materiaEditar){setNombre(materiaEditar.nombre);setCodigo(materiaEditar.codigo);setAnio(materiaEditar.anio);setCarreraId(materiaEditar.carreraId);setProfesorId(materiaEditar.profesorId??"");setActiva(materiaEditar.activa);setCantidadEvaluaciones(materiaEditar.cantidadEvaluaciones??2);setEsPromocionable(materiaEditar.esPromocionable??true);setNotaPromocion(materiaEditar.notaPromocion??7);setNotaRegularizacion(materiaEditar.notaRegularizacion??4);}else limpiar()},[materiaEditar]);
  function limpiar(){setNombre("");setCodigo("");setAnio(1);setCarreraId("");setProfesorId("");setActiva(true);setCantidadEvaluaciones(2);setEsPromocionable(true);setNotaPromocion(7);setNotaRegularizacion(4)}
  async function submit(e:React.FormEvent){e.preventDefault();if(!carreraId)return alert("Tenés que seleccionar una carrera");if(cantidadEvaluaciones<1)return alert("Debe existir al menos una evaluación");if(notaRegularizacion<0||notaRegularizacion>10)return alert("La nota de regularización debe estar entre 0 y 10");if(esPromocionable&&(notaPromocion<notaRegularizacion||notaPromocion>10))return alert("La nota de promoción debe ser mayor o igual a la de regularización");
    const base={nombre,codigo,anio,carreraId,profesorId:profesorId||null,cantidadEvaluaciones,esPromocionable,notaPromocion:esPromocionable?notaPromocion:null,notaRegularizacion};
    try{if(materiaEditar)await updateMateria(materiaEditar.id,{...base,activa} as UpdateMateriaRequest);else await createMateria(base as CreateMateriaRequest);limpiar();onGuardado();}catch(err){console.error(err);alert(materiaEditar?"No se pudo actualizar la materia":"No se pudo crear la materia")}}

  return <form onSubmit={submit}><h2>{materiaEditar?"Editar materia":"Nueva materia"}</h2>
    <div className="form-grid">
      <div className="form-group"><label>Nombre</label><input value={nombre} onChange={e=>setNombre(e.target.value)} required/></div>
      <div className="form-group"><label>Código</label><input value={codigo} onChange={e=>setCodigo(e.target.value)} required/></div>
      <div className="form-group"><label>Año</label><input type="number" min="1" value={anio} onChange={e=>setAnio(Number(e.target.value))} required/></div>
      <div className="form-group"><label>Carrera</label><select value={carreraId} onChange={e=>setCarreraId(e.target.value)} required disabled={cargandoOpciones}><option value="">Seleccionar carrera</option>{carreras.map(c=><option key={c.id} value={c.id}>{c.nombre}</option>)}</select></div>
      <div className="form-group"><label>Profesor</label><select value={profesorId} onChange={e=>setProfesorId(e.target.value)} disabled={cargandoOpciones}><option value="">Sin profesor asignado</option>{profesores.map(p=><option key={p.id} value={p.id}>{p.apellido}, {p.nombre}</option>)}</select></div>
      <div className="form-group"><label>Cantidad de evaluaciones</label><input type="number" min="1" max="20" value={cantidadEvaluaciones} onChange={e=>setCantidadEvaluaciones(Number(e.target.value))}/></div>
      <div className="form-group"><label>Nota para regularizar</label><input type="number" min="0" max="10" step="0.01" value={notaRegularizacion} onChange={e=>setNotaRegularizacion(Number(e.target.value))}/></div>
      <div className="form-group"><label>Nota para promocionar</label><input type="number" min="0" max="10" step="0.01" value={notaPromocion} onChange={e=>setNotaPromocion(Number(e.target.value))} disabled={!esPromocionable}/></div>
    </div>
    <label className="check-line"><input type="checkbox" checked={esPromocionable} onChange={e=>setEsPromocionable(e.target.checked)}/> Materia promocionable</label>
    {materiaEditar&&<label className="check-line"><input type="checkbox" checked={activa} onChange={e=>setActiva(e.target.checked)}/> Activa</label>}
    <div className="form-actions"><button className="btn btn-primary">{materiaEditar?"Actualizar materia":"Guardar materia"}</button>{materiaEditar&&<button className="btn btn-secondary" type="button" onClick={()=>{limpiar();onCancelarEdicion()}}>Cancelar</button>}</div>
  </form>
}
