import { useEffect, useState } from "react";
import type { PlanEstudioCarrera } from "../types/PlanEstudio";
import { getPlanEstudios } from "../services/planEstudioService";

export default function PlanEstudiosPage() {
  const [plan, setPlan] = useState<PlanEstudioCarrera[]>([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    cargarPlan();
  }, []);

  async function cargarPlan() {
    try {
      setCargando(true);
      setError("");

      const data = await getPlanEstudios();
      setPlan(data);
    } catch (error) {
      console.error(error);
      setError("No se pudo cargar el plan de estudios");
    } finally {
      setCargando(false);
    }
  }

  return (
    <div>
      <div className="page-header">
        <h1>Plan de estudios</h1>
        <p>Todas las carreras, con sus materias agrupadas por año.</p>
      </div>

      {cargando ? (
        <div className="card">
          <p>Cargando plan de estudios...</p>
        </div>
      ) : error ? (
        <div className="card">
          <p>{error}</p>
        </div>
      ) : plan.length === 0 ? (
        <div className="card">
          <p>No hay carreras registradas.</p>
        </div>
      ) : (
        plan.map((carrera) => (
          <div className="card plan-carrera" key={carrera.carreraId}>
            <div className="plan-carrera-header">
              <div>
                <h2>{carrera.carreraNombre}</h2>
                <span className="table-sub">
                  {carrera.duracionAnios} año
                  {carrera.duracionAnios === 1 ? "" : "s"} de duración
                </span>
              </div>

              <span
                className={
                  carrera.carreraActiva
                    ? "status status-active"
                    : "status status-inactive"
                }
              >
                {carrera.carreraActiva ? "Activa" : "Inactiva"}
              </span>
            </div>

            {carrera.anios.every((a) => a.materias.length === 0) ? (
              <p className="muted">
                Todavía no hay materias cargadas para esta carrera.
              </p>
            ) : (
              <div className="plan-anios-grid">
                {carrera.anios.map((anio) => (
                  <div className="plan-anio" key={anio.anio}>
                    <h3>{anio.anio}º año</h3>

                    {anio.materias.length === 0 ? (
                      <p className="muted">Sin materias cargadas.</p>
                    ) : (
                      <ul className="plan-materias-list">
                        {anio.materias.map((materia) => (
                          <li
                            key={materia.id}
                            className={
                              materia.activa
                                ? "plan-materia-item"
                                : "plan-materia-item inactiva"
                            }
                          >
                            <div>
                              <strong>{materia.nombre}</strong>
                              <span className="table-sub">
                                {materia.codigo}
                              </span>
                            </div>
                            <span className="plan-materia-profesor">
                              {materia.profesorNombre ?? "Sin profesor asignado"}
                            </span>
                          </li>
                        ))}
                      </ul>
                    )}
                  </div>
                ))}
              </div>
            )}
          </div>
        ))
      )}
    </div>
  );
}
