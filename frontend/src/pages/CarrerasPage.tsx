import { useEffect, useState } from "react";
import type { Carrera } from "../types/Carrera";
import {
  getCarreras,
  deleteCarrera,
} from "../services/carreraService";
import CarreraForm from "../components/CarreraForm";

export default function CarrerasPage() {
  const [carreras, setCarreras] = useState<Carrera[]>([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState("");

  const [carreraEditar, setCarreraEditar] =
    useState<Carrera | null>(null);

  useEffect(() => {
    cargarCarreras();
  }, []);

  async function cargarCarreras() {
    try {
      setCargando(true);
      setError("");

      const data = await getCarreras();

      setCarreras(data);
    } catch (error) {
      console.error(error);
      setError("No se pudieron cargar las carreras");
    } finally {
      setCargando(false);
    }
  }

  async function handleGuardado() {
    setCarreraEditar(null);
    await cargarCarreras();
  }

  async function handleEliminar(carrera: Carrera) {
    const confirmar = window.confirm(
      `¿Seguro que querés eliminar la carrera ${carrera.nombre}?`
    );

    if (!confirmar) {
      return;
    }

    try {
      await deleteCarrera(carrera.id);

      if (carreraEditar?.id === carrera.id) {
        setCarreraEditar(null);
      }

      await cargarCarreras();
    } catch (error) {
      console.error(error);

      alert(
        "No se pudo eliminar la carrera. Puede estar siendo utilizada por alumnos o materias."
      );
    }
  }

  return (
    <div>
      <div className="page-header">
        <h1>Carreras</h1>
        <p>Gestión de carreras del instituto.</p>
      </div>

      <div className="card">
        <CarreraForm
          carreraEditar={carreraEditar}
          onGuardado={handleGuardado}
          onCancelarEdicion={() =>
            setCarreraEditar(null)
          }
        />
      </div>

      <div className="card">
        {cargando ? (
          <p>Cargando carreras...</p>
        ) : error ? (
          <p>{error}</p>
        ) : carreras.length === 0 ? (
          <p>No hay carreras registradas.</p>
        ) : (
          <div className="table-container">
            <table>
              <thead>
                <tr>
                  <th>Nombre</th>
                  <th>Descripción</th>
                  <th>Duración</th>
                  <th>Estado</th>
                  <th>Acciones</th>
                </tr>
              </thead>

              <tbody>
                {carreras.map((carrera) => (
                  <tr key={carrera.id}>
                    <td>{carrera.nombre}</td>

                    <td>
                      {carrera.descripcion ?? "-"}
                    </td>

                    <td>
                      {carrera.duracionAnios} años
                    </td>

                    <td>
                      <span
                        className={
                          carrera.activa
                            ? "status status-active"
                            : "status status-inactive"
                        }
                      >
                        {carrera.activa
                          ? "Activa"
                          : "Inactiva"}
                      </span>
                    </td>

                    <td>
                      <div className="actions">
                        <button
                          className="btn btn-secondary btn-small"
                          type="button"
                          onClick={() =>
                            setCarreraEditar(carrera)
                          }
                        >
                          Editar
                        </button>

                        <button
                          className="btn btn-danger btn-small"
                          type="button"
                          onClick={() =>
                            handleEliminar(carrera)
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