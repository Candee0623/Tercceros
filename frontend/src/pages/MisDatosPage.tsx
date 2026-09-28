import { useEffect, useState } from "react";
import { getMisDatos } from "../services/portalAlumnoService";
import type { MiPerfilAlumno } from "../types/PortalAlumno";

function fecha(x:string){return new Date(x).toLocaleDateString();}

export default function MisDatosPage(){
 const [data,setData]=useState<MiPerfilAlumno|null>(null),[error,setError]=useState("");
 useEffect(()=>{getMisDatos().then(setData).catch(e=>setError(e instanceof Error?e.message:"Error"))},[]);
 return <div><div className="page-header"><h1>Mis datos</h1><p>Tu información académica registrada en el instituto.</p></div>{error&&<div className="alert-error">{error}</div>}
 {!data&&!error&&<div className="card">Cargando...</div>}
 {data&&<div className="card student-profile-card"><div className="student-profile-head"><div className="student-avatar">🎓</div><div><h2>{data.apellido}, {data.nombre}</h2><p>DNI {data.dni}</p></div></div><div className="student-profile-grid">
 <div><span>Carrera</span><strong>{data.carreraNombre??"Sin carrera asignada"}</strong></div><div><span>Correo</span><strong>{data.email||"-"}</strong></div><div><span>Teléfono</span><strong>{data.telefono||"-"}</strong></div><div><span>Fecha de nacimiento</span><strong>{fecha(data.fechaNacimiento)}</strong></div><div><span>Fecha de ingreso</span><strong>{fecha(data.fechaIngreso)}</strong></div></div></div>}
 </div>
}
