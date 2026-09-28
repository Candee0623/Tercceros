import { apiFetch } from "../auth/authService";
import type { Pantalla, Rol, Usuario, UsuarioAlumnoOption } from "../types/Seguridad";

async function unwrap<T>(r: Response): Promise<T> { if (!r.ok) throw new Error((await r.text()) || "Error en la operación"); return r.json(); }
export const getRoles = () => apiFetch("/Roles").then(unwrap<Rol[]>);
export const getPantallas = () => apiFetch("/Roles/pantallas").then(unwrap<Pantalla[]>);
export const createRol = (data:{nombre:string;descripcion?:string;activo:boolean;permisos:string[]}) => apiFetch("/Roles",{method:"POST",body:JSON.stringify(data)}).then(unwrap<Rol>);
export const updateRol = (id:string,data:{nombre:string;descripcion?:string;activo:boolean;permisos:string[]}) => apiFetch(`/Roles/${id}`,{method:"PUT",body:JSON.stringify(data)}).then(unwrap<Rol>);
export async function deleteRol(id:string){ const r=await apiFetch(`/Roles/${id}`,{method:"DELETE"}); if(!r.ok) throw new Error((await r.text())||"No se pudo eliminar"); }
export const getUsuarios = () => apiFetch("/Usuarios").then(unwrap<Usuario[]>);
export const getRolesForUsuarios = () => apiFetch("/Usuarios/roles").then(unwrap<{id:string;nombre:string}[]>);
export const getAlumnosForUsuarios = (incluir?:string|null) => apiFetch(`/Usuarios/alumnos${incluir?`?incluir=${encodeURIComponent(incluir)}`:""}`).then(unwrap<UsuarioAlumnoOption[]>);
export const createUsuario = (data:{nombreUsuario:string;nombre:string;apellido:string;password:string;rolId:string;alumnoId?:string|null;activo:boolean}) => apiFetch("/Usuarios",{method:"POST",body:JSON.stringify(data)}).then(unwrap<Usuario>);
export const updateUsuario = (id:string,data:{nombreUsuario:string;nombre:string;apellido:string;password?:string;rolId:string;alumnoId?:string|null;activo:boolean}) => apiFetch(`/Usuarios/${id}`,{method:"PUT",body:JSON.stringify(data)}).then(unwrap<Usuario>);
