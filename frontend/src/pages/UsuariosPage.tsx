import { useEffect, useMemo, useState } from "react";
import type {
Usuario,
UsuarioAlumnoOption,
UsuarioProfesorOption,
} from "../services/usuarioService";
import {
actualizarUsuario,
crearUsuario,
getAlumnosUsuario,
getProfesoresUsuario,
getRolesUsuario,
getUsuarios,
} from "../services/usuarioService";

const empty = {
nombreUsuario: "",
nombre: "",
apellido: "",
password: "",
rolId: "",
alumnoId: "",
profesorId: "",
activo: true,
};

export default function UsuariosPage() {
const [usuarios, setUsuarios] = useState<Usuario[]>([]);
const [roles, setRoles] = useState<{ id: string; nombre: string }[]>([]);
const [alumnos, setAlumnos] = useState<UsuarioAlumnoOption[]>([]);
const [profesores, setProfesores] = useState<UsuarioProfesorOption[]>([]);
const [edit, setEdit] = useState<Usuario | null>(null);
const [form, setForm] = useState(empty);
const [error, setError] = useState("");

const rolSeleccionado = useMemo(
() => roles.find((r) => r.id === form.rolId),
[roles, form.rolId]
);

const nombreRol = rolSeleccionado?.nombre?.toUpperCase() ?? "";

const esAlumno = nombreRol === "ALUMNO";
const esProfesor = nombreRol === "PROFESOR";

async function load(
incluirAlumnoId?: string | null,
incluirProfesorId?: string | null
) {
try {
setError("");


  const [u, r, a, p] = await Promise.all([
    getUsuarios(),
    getRolesUsuario(),
    getAlumnosUsuario(incluirAlumnoId),
    getProfesoresUsuario(incluirProfesorId),
  ]);

  setUsuarios(u);
  setRoles(r);
  setAlumnos(a);
  setProfesores(p);
} catch (e) {
  setError(e instanceof Error ? e.message : "Error");
}


}

useEffect(() => {
load();
}, []);

async function start(u: Usuario) {
setEdit(u);


setForm({
  nombreUsuario: u.nombreUsuario,
  nombre: u.nombre,
  apellido: u.apellido,
  password: "",
  rolId: u.rolId,
  alumnoId: u.alumnoId ?? "",
  profesorId: u.profesorId ?? "",
  activo: u.activo,
});

try {
  setError("");

  const [alumnosDisponibles, profesoresDisponibles] = await Promise.all([
    getAlumnosUsuario(u.alumnoId),
    getProfesoresUsuario(u.profesorId),
  ]);

  setAlumnos(alumnosDisponibles);
  setProfesores(profesoresDisponibles);
} catch (e) {
  setError(e instanceof Error ? e.message : "Error");
}


}

function cambiarRol(rolId: string) {
setForm({
...form,
rolId,
alumnoId: "",
profesorId: "",
});
}

async function save(e: React.FormEvent) {
e.preventDefault();


try {
  setError("");

  if (esAlumno && !form.alumnoId) {
    setError(
      "Para un usuario con rol ALUMNO debés seleccionar el alumno asociado."
    );
    return;
  }

  if (esProfesor && !form.profesorId) {
    setError(
      "Para un usuario con rol PROFESOR debés seleccionar el profesor asociado."
    );
    return;
  }

  const payload = {
    nombreUsuario: form.nombreUsuario,
    nombre: form.nombre,
    apellido: form.apellido,
    password: form.password,
    rolId: form.rolId,
    alumnoId: esAlumno ? form.alumnoId || null : null,
    profesorId: esProfesor ? form.profesorId || null : null,
    activo: form.activo,
  };

  if (edit) {
    await actualizarUsuario(edit.id, {
      ...payload,
      password: form.password || undefined,
    });
  } else {
    await crearUsuario(payload);
  }

  setEdit(null);
  setForm(empty);

  await load();
} catch (e) {
  setError(e instanceof Error ? e.message : "Error");
}


}

function cancel() {
setEdit(null);
setForm(empty);
load();
}

return ( <div> <div className="page-header"> <h1>Usuarios</h1>


    <p>
      Alta de usuarios, asignación de rol y vinculación con la ficha
      académica.
    </p>
  </div>

  {error && <div className="alert-error">{error}</div>}

  <div className="card">
    <h3>{edit ? "Editar usuario" : "Nuevo usuario"}</h3>

    <form onSubmit={save}>
      <div className="form-grid">
        <div className="form-group">
          <label>Usuario</label>

          <input
            value={form.nombreUsuario}
            onChange={(e) =>
              setForm({
                ...form,
                nombreUsuario: e.target.value,
              })
            }
            required
          />
        </div>

        <div className="form-group">
          <label>
            {edit
              ? "Nueva contraseña (opcional)"
              : "Contraseña"}
          </label>

          <input
            type="password"
            value={form.password}
            onChange={(e) =>
              setForm({
                ...form,
                password: e.target.value,
              })
            }
            required={!edit}
          />
        </div>

        <div className="form-group">
          <label>Nombre</label>

          <input
            value={form.nombre}
            onChange={(e) =>
              setForm({
                ...form,
                nombre: e.target.value,
              })
            }
          />
        </div>

        <div className="form-group">
          <label>Apellido</label>

          <input
            value={form.apellido}
            onChange={(e) =>
              setForm({
                ...form,
                apellido: e.target.value,
              })
            }
          />
        </div>

        <div className="form-group">
          <label>Rol</label>

          <select
            value={form.rolId}
            onChange={(e) => cambiarRol(e.target.value)}
            required
          >
            <option value="">Seleccionar...</option>

            {roles.map((rol) => (
              <option key={rol.id} value={rol.id}>
                {rol.nombre}
              </option>
            ))}
          </select>
        </div>

        {esAlumno && (
          <div className="form-group">
            <label>Alumno asociado</label>

            <select
              value={form.alumnoId}
              onChange={(e) =>
                setForm({
                  ...form,
                  alumnoId: e.target.value,
                })
              }
              required
            >
              <option value="">
                Seleccionar alumno...
              </option>

              {alumnos.map((alumno) => (
                <option key={alumno.id} value={alumno.id}>
                  {alumno.nombreCompleto} · DNI {alumno.dni}
                </option>
              ))}
            </select>

            <small>
              Este vínculo determina qué datos, asistencia y
              matrículas podrá consultar este usuario.
            </small>
          </div>
        )}

        {esProfesor && (
          <div className="form-group">
            <label>Profesor asociado</label>

            <select
              value={form.profesorId}
              onChange={(e) =>
                setForm({
                  ...form,
                  profesorId: e.target.value,
                })
              }
              required
            >
              <option value="">
                Seleccionar profesor...
              </option>

              {profesores.map((profesor) => (
                <option
                  key={profesor.id}
                  value={profesor.id}
                >
                  {profesor.nombreCompleto} · DNI{" "}
                  {profesor.dni}
                </option>
              ))}
            </select>

            <small>
              Este vínculo determina qué cursadas podrá
              utilizar el usuario como profesor.
            </small>
          </div>
        )}
      </div>

      {esAlumno && (
        <div className="info-box">
          <strong>Cuenta de alumno:</strong> el usuario
          queda vinculado al alumno seleccionado.
        </div>
      )}

      {esProfesor && (
        <div className="info-box">
          <strong>Cuenta de profesor:</strong> el usuario
          queda vinculado al profesor seleccionado y podrá
          trabajar con sus propias cursadas.
        </div>
      )}

      <label className="check-line">
        <input
          type="checkbox"
          checked={form.activo}
          onChange={(e) =>
            setForm({
              ...form,
              activo: e.target.checked,
            })
          }
        />

        Usuario activo
      </label>

      <div className="form-actions">
        <button
          type="submit"
          className="btn btn-primary"
        >
          Guardar
        </button>

        {edit && (
          <button
            type="button"
            className="btn btn-secondary"
            onClick={cancel}
          >
            Cancelar
          </button>
        )}
      </div>
    </form>
  </div>

  <div className="card table-container">
    <table>
      <thead>
        <tr>
          <th>Usuario</th>
          <th>Nombre</th>
          <th>Rol</th>
          <th>Alumno asociado</th>
          <th>Profesor asociado</th>
          <th>Estado</th>
          <th>Acciones</th>
        </tr>
      </thead>

      <tbody>
        {usuarios.map((u) => (
          <tr key={u.id}>
            <td>{u.nombreUsuario}</td>

            <td>
              {`${u.nombre} ${u.apellido}`.trim() || "-"}
            </td>

            <td>{u.rolNombre}</td>

            <td>{u.alumnoNombre ?? "-"}</td>

            <td>{u.profesorNombre ?? "-"}</td>

            <td>
              {u.activo ? "Activo" : "Inactivo"}
            </td>

            <td>
              <button
                type="button"
                className="btn btn-secondary btn-small"
                onClick={() => start(u)}
              >
                Editar
              </button>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  </div>
</div>


);
}
