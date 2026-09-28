import { useEffect, useState } from "react";
import type { Profesor } from "../types/Profesor";
import {
  getProfesores,
  deleteProfesor,
} from "../services/profesorService";
import ProfesorForm from "../components/ProfesorForm";

export default function ProfesoresPage() {
  const [profesores, setProfesores] = useState<Profesor[]>([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState("");

  const [profesorEditar, setProfesorEditar] =
    useState<Profesor | null>(null);

  useEffect(() => {
    cargarProfesores();
  }, []);

  async function cargarProfesores() {
    try {
      setCargando(true);
      setError("");

      const data = await getProfesores();

      setProfesores(data);
    } catch (error) {
      console.error(error);

      setError("No se pudieron cargar los profesores");
    } finally {
      setCargando(false);
    }
  }

  async function handleGuardado() {
    setProfesorEditar(null);
    await cargarProfesores();
  }

  async function handleEliminar(profesor: Profesor) {
    const confirmar = window.confirm(
      `¿Seguro que querés eliminar a ${profesor.nombre} ${profesor.apellido}?`
    );

    if (!confirmar) {
      return;
    }

    try {
      await deleteProfesor(profesor.id);

      if (profesorEditar?.id === profesor.id) {
        setProfesorEditar(null);
      }

      await cargarProfesores();
    } catch (error) {
      console.error(error);

      alert(
        "No se pudo eliminar el profesor. Puede estar asignado a alguna materia."
      );
    }
  }

  return (
    <div>
      <div className="page-header">
        <h1>Profesores</h1>
        <p>Alta, edición y gestión de profesores del instituto.</p>
      </div>

      <div className="card">
        <ProfesorForm
          profesorEditar={profesorEditar}
          onGuardado={handleGuardado}
          onCancelarEdicion={() =>
            setProfesorEditar(null)
          }
        />
      </div>

      <div className="card">
        {cargando ? (
          <p>Cargando profesores...</p>
        ) : error ? (
          <p>{error}</p>
        ) : profesores.length === 0 ? (
          <p>No hay profesores registrados.</p>
        ) : (
          <div className="table-container">
            <table>
              <thead>
                <tr>
                  <th>Nombre</th>
                  <th>Apellido</th>
                  <th>DNI</th>
                  <th>Email</th>
                  <th>Título</th>
                  <th>Estado</th>
                  <th>Acciones</th>
                </tr>
              </thead>

              <tbody>
                {profesores.map((profesor) => (
                  <tr key={profesor.id}>
                    <td>{profesor.nombre}</td>
                    <td>{profesor.apellido}</td>
                    <td>{profesor.dni}</td>
                    <td>{profesor.email ?? "-"}</td>
                    <td>{profesor.titulo ?? "-"}</td>

                    <td>
                      <span
                        className={
                          profesor.activo
                            ? "status status-active"
                            : "status status-inactive"
                        }
                      >
                        {profesor.activo
                          ? "Activo"
                          : "Inactivo"}
                      </span>
                    </td>

                    <td>
                      <div className="actions">
                        <button
                          className="btn btn-secondary btn-small"
                          type="button"
                          onClick={() =>
                            setProfesorEditar(profesor)
                          }
                        >
                          Editar
                        </button>

                        <button
                          className="btn btn-danger btn-small"
                          type="button"
                          onClick={() =>
                            handleEliminar(profesor)
                          }
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