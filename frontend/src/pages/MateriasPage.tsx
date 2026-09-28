import { useEffect, useState } from "react";
import type { Materia } from "../types/Materia";
import {
  deleteMateria,
  getMaterias,
} from "../services/materiaService";
import MateriaForm from "../components/MateriaForm";

export default function MateriasPage() {
  const [materias, setMaterias] = useState<Materia[]>([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState("");

  const [materiaEditar, setMateriaEditar] =
    useState<Materia | null>(null);

  useEffect(() => {
    cargarMaterias();
  }, []);

  async function cargarMaterias() {
    try {
      setCargando(true);
      setError("");

      const data = await getMaterias();
      setMaterias(data);
    } catch (error) {
      console.error(error);
      setError("No se pudieron cargar las materias");
    } finally {
      setCargando(false);
    }
  }

  async function handleGuardado() {
    setMateriaEditar(null);
    await cargarMaterias();
  }

  async function handleEliminar(materia: Materia) {
    const confirmar = window.confirm(
      `¿Seguro que querés eliminar la materia ${materia.nombre}?`
    );

    if (!confirmar) {
      return;
    }

    try {
      await deleteMateria(materia.id);

      if (materiaEditar?.id === materia.id) {
        setMateriaEditar(null);
      }

      await cargarMaterias();
    } catch (error) {
      console.error(error);
      alert("No se pudo eliminar la materia");
    }
  }

  return (
    <div>
      <div className="page-header">
        <h1>Materias</h1>
        <p>Alta, edición y gestión de materias del instituto.</p>
      </div>

      <div className="card">
        <MateriaForm
          materiaEditar={materiaEditar}
          onGuardado={handleGuardado}
          onCancelarEdicion={() => setMateriaEditar(null)}
        />
      </div>

      <div className="card">
        {cargando ? (
          <p>Cargando materias...</p>
        ) : error ? (
          <p>{error}</p>
        ) : materias.length === 0 ? (
          <p>No hay materias registradas.</p>
        ) : (
          <div className="table-container">
            <table>
              <thead>
                <tr>
                  <th>Código</th>
                  <th>Materia</th>
                  <th>Año</th>
                  <th>Carrera</th>
                  <th>Profesor</th>
                  <th>Evaluaciones</th>
                  <th>Condición</th>
                  <th>Estado</th>
                  <th>Acciones</th>
                </tr>
              </thead>

              <tbody>
                {materias.map((materia) => (
                  <tr key={materia.id}>
                    <td>{materia.codigo}</td>
                    <td>{materia.nombre}</td>
                    <td>{materia.anio}°</td>
                    <td>{materia.carreraNombre}</td>
                    <td>{materia.profesorNombre ?? "Sin asignar"}</td>
                    <td>{materia.cantidadEvaluaciones}</td>
                    <td>{materia.esPromocionable ? `Prom. ${materia.notaPromocion} / Reg. ${materia.notaRegularizacion}` : `Reg. ${materia.notaRegularizacion}`}</td>

                    <td>
                      <span
                        className={
                          materia.activa
                            ? "status status-active"
                            : "status status-inactive"
                        }
                      >
                        {materia.activa ? "Activa" : "Inactiva"}
                      </span>
                    </td>

                    <td>
                      <div className="actions">
                        <button
                          className="btn btn-secondary btn-small"
                          type="button"
                          onClick={() => setMateriaEditar(materia)}
                        >
                          Editar
                        </button>

                        <button
                          className="btn btn-danger btn-small"
                          type="button"
                          onClick={() => handleEliminar(materia)}
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
