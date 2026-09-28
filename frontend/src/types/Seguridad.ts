export interface Rol { id:string; nombre:string; descripcion?:string|null; activo:boolean; esSistema:boolean; permisos:string[]; }
export interface Pantalla { clave:string; nombre:string; }
export interface Usuario { id:string; nombreUsuario:string; nombre:string; apellido:string; activo:boolean; rolId:string; rolNombre:string; alumnoId?:string|null; alumnoNombre?:string|null; }
export interface UsuarioAlumnoOption { id:string; nombreCompleto:string; dni:string; }
