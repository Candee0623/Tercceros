import { useEffect, useState } from "react";
import type { Alumno } from "../types/Alumno";
import {
  getAlumnos,
  deleteAlumno,
} from "../services/alumnoService";
import AlumnoForm from "../components/AlumnoForm";

export default function AlumnosPage() {
  const [alumnos, setAlumnos] = useState<Alumno[]>([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState("");

  // Alumno que estamos editando.
  // Si es null, el formulario funciona como "Nuevo alumno".
  const [alumnoEditar, setAlumnoEditar] =
    useState<Alumno | null>(null);

  useEffect(() => {
    cargarAlumnos();
  }, []);

  async function cargarAlumnos() {
    try {
      setCargando(true);
      setError("");

      const data = await getAlumnos();

      setAlumnos(data);
    } catch (error) {
      console.error(error);

      setError("No se pudieron cargar los alumnos");
    } finally {
      setCargando(false);
    }
  }

  async function handleGuardado() {
    // Si estábamos editando, volvemos al modo creación
    setAlumnoEditar(null);

    // Volvemos a consultar el backend
    await cargarAlumnos();
  }

  async function handleEliminar(alumno: Alumno) {
    const confirmar = window.confirm(
      `¿Seguro que querés eliminar a ${alumno.nombre} ${alumno.apellido}?`
    );

    if (!confirmar) {
      return;
    }

    try {
      await deleteAlumno(alumno.id);

      // Si justo estábamos editando al alumno eliminado,
      // limpiamos el formulario.
      if (alumnoEditar?.id === alumno.id) {
        setAlumnoEditar(null);
      }

      await cargarAlumnos();
    } catch (error) {
      console.error(error);

      alert("No se pudo eliminar el alumno");
    }
  }

  return  (
  <div>
    <div className="page-header">
      <h1>Alumnos</h1>
      <p>Alta, edición y gestión de alumnos del instituto.</p>
    </div>

    <div className="card">
      <AlumnoForm
        alumnoEditar={alumnoEditar}
        onGuardado={handleGuardado}
        onCancelarEdicion={() => setAlumnoEditar(null)}
      />
    </div>

    <div className="card">
      {cargando ? (
        <p>Cargando alumnos...</p>
      ) : error ? (
        <p>{error}</p>
      ) : alumnos.length === 0 ? (
        <p>No hay alumnos registrados.</p>
      ) : (
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Apellido</th>
                <th>DNI</th>
                <th>Email</th>
                <th>Carrera</th>
                <th>Estado</th>
                <th>Acciones</th>
              </tr>
            </thead>

            <tbody>
              {alumnos.map((alumno) => (
                <tr key={alumno.id}>
                  <td>{alumno.nombre}</td>
                  <td>{alumno.apellido}</td>
                  <td>{alumno.dni}</td>
                  <td>{alumno.email ?? "-"}</td>
                  <td>{alumno.carreraNombre ?? "Sin carrera"}</td>

                  <td>
                    <span
                      className={
                        alumno.activo
                          ? "status status-active"
                          : "status status-inactive"
                      }
                    >
                      {alumno.activo ? "Activo" : "Inactivo"}
                    </span>
                  </td>

                  <td>
                    <div className="actions">
                      <button
                        className="btn btn-secondary btn-small"
                        type="button"
                        onClick={() => setAlumnoEditar(alumno)}
                      >
                        Editar
                      </button>

                      <button
                        className="btn btn-danger btn-small"
                        type="button"
                        onClick={() => handleEliminar(alumno)}
                      >
                        Eliminar
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  </div>
);
}