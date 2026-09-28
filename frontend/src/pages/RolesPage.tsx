import { useEffect, useState } from "react";
import type { Pantalla, Rol } from "../types/Seguridad";
import { createRol, deleteRol, getPantallas, getRoles, updateRol } from "../services/seguridadService";

const empty={nombre:"",descripcion:"",activo:true,permisos:[] as string[]};
export default function RolesPage(){
 const [roles,setRoles]=useState<Rol[]>([]), [pantallas,setPantallas]=useState<Pantalla[]>([]), [edit,setEdit]=useState<Rol|null>(null), [form,setForm]=useState(empty), [error,setError]=useState("");
 async function load(){try{setError(""); const [r,p]=await Promise.all([getRoles(),getPantallas()]);setRoles(r);setPantallas(p)}catch(e){setError(e instanceof Error?e.message:"Error")}}
 useEffect(()=>{load()},[]);
 function start(r:Rol){setEdit(r);setForm({nombre:r.nombre,descripcion:r.descripcion??"",activo:r.activo,permisos:[...r.permisos]})}
 function toggle(k:string){setForm(f=>({...f,permisos:f.permisos.includes(k)?f.permisos.filter(x=>x!==k):[...f.permisos,k]}))}
 async function save(e:React.FormEvent){e.preventDefault();try{if(edit)await updateRol(edit.id,form);else await createRol(form);setEdit(null);setForm(empty);await load()}catch(e){setError(e instanceof Error?e.message:"Error")}}
 async function remove(r:Rol){if(!confirm(`¿Eliminar rol ${r.nombre}?`))return;try{await deleteRol(r.id);await load()}catch(e){setError(e instanceof Error?e.message:"Error")}}
 return <div><div className="page-header"><h1>Roles</h1><p>Definí qué pantallas puede utilizar cada rol.</p></div>{error&&<div className="alert-error">{error}</div>}
 <div className="card"><h3>{edit?"Editar rol":"Nuevo rol"}</h3><form onSubmit={save}><div className="form-grid"><div className="form-group"><label>Nombre</label><input value={form.nombre} onChange={e=>{const nombre=e.target.value;setForm({...form,nombre,permisos:!edit&&nombre.trim().toUpperCase()==="ALUMNO"?["mis_datos","mi_asistencia","auto_matriculacion"]:form.permisos})}} required disabled={edit?.esSistema}/></div><div className="form-group"><label>Descripción</label><input value={form.descripcion} onChange={e=>setForm({...form,descripcion:e.target.value})}/></div></div>
 <h4>Acceso a pantallas</h4><div className="permissions-grid">{pantallas.map(p=><label className="permission-item" key={p.clave}><input type="checkbox" checked={form.permisos.includes(p.clave)} onChange={()=>toggle(p.clave)} disabled={edit?.esSistema}/><span>{p.nombre}</span></label>)}</div>
 <label className="check-line"><input type="checkbox" checked={form.activo} onChange={e=>setForm({...form,activo:e.target.checked})} disabled={edit?.esSistema}/> Rol activo</label>
 <div className="form-actions"><button className="btn btn-primary">Guardar</button>{edit&&<button type="button" className="btn btn-secondary" onClick={()=>{setEdit(null);setForm(empty)}}>Cancelar</button>}</div></form></div>
 <div className="card table-container"><table><thead><tr><th>Rol</th><th>Descripción</th><th>Pantallas</th><th>Estado</th><th>Acciones</th></tr></thead><tbody>{roles.map(r=><tr key={r.id}><td>{r.nombre}{r.esSistema?" 🔒":""}</td><td>{r.descripcion||"-"}</td><td>{r.permisos.length}</td><td>{r.activo?"Activo":"Inactivo"}</td><td><button className="btn btn-secondary btn-small" onClick={()=>start(r)}>Editar</button>{!r.esSistema&&<button className="btn btn-danger btn-small security-gap" onClick={()=>remove(r)}>Eliminar</button>}</td></tr>)}</tbody></table></div></div>
}
