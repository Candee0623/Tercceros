import { useEffect, useState } from "react";
import type { Carrera } from "../types/Carrera";
import {
  createCarrera,
  updateCarrera,
  type CreateCarreraRequest,
  type UpdateCarreraRequest,
} from "../services/carreraService";

interface Props {
  carreraEditar?: Carrera | null;
  onGuardado: () => void;
  onCancelarEdicion: () => void;
}

export default function CarreraForm({
  carreraEditar,
  onGuardado,
  onCancelarEdicion,
}: Props) {
  const [nombre, setNombre] = useState("");
  const [descripcion, setDescripcion] = useState("");
  const [duracionAnios, setDuracionAnios] = useState(3);
  const [activa, setActiva] = useState(true);

  useEffect(() => {
    if (carreraEditar) {
      setNombre(carreraEditar.nombre);
      setDescripcion(carreraEditar.descripcion ?? "");
      setDuracionAnios(carreraEditar.duracionAnios);
      setActiva(carreraEditar.activa);
    } else {
      limpiarFormulario();
    }
  }, [carreraEditar]);

  function limpiarFormulario() {
    setNombre("");
    setDescripcion("");
    setDuracionAnios(3);
    setActiva(true);
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();

    try {
      if (carreraEditar) {
        const carreraActualizada: UpdateCarreraRequest = {
          nombre,
          descripcion: descripcion || null,
          duracionAnios,
          activa,
        };

        await updateCarrera(
          carreraEditar.id,
          carreraActualizada
        );
      } else {
        const nuevaCarrera: CreateCarreraRequest = {
          nombre,
          descripcion: descripcion || null,
          duracionAnios,
        };

        await createCarrera(nuevaCarrera);
      }

      limpiarFormulario();
      onGuardado();
    } catch (error) {
      console.error(error);

      alert(
        carreraEditar
          ? "No se pudo actualizar la carrera"
          : "No se pudo crear la carrera"
      );
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      <h2>
        {carreraEditar ? "Editar carrera" : "Nueva carrera"}
      </h2>

      <div className="form-grid">
        <div className="form-group">
          <label>Nombre</label>
          <input
            value={nombre}
            onChange={(e) => setNombre(e.target.value)}
            required
          />
        </div>

        <div className="form-group">
          <label>Duración en años</label>
          <input
            type="number"
            min="1"
            value={duracionAnios}
            onChange={(e) =>
              setDuracionAnios(Number(e.target.value))
            }
            required
          />
        </div>

        <div className="form-group">
          <label>Descripción</label>
          <input
            value={descripcion}
            onChange={(e) =>
              setDescripcion(e.target.value)
            }
          />
        </div>
      </div>

      {carreraEditar && (
        <div className="form-group">
          <label>
            <input
              type="checkbox"
              checked={activa}
              onChange={(e) =>
                setActiva(e.target.checked)
              }
            />
            Activa
          </label>
        </div>
      )}

      <div className="form-actions">
        <button className="btn btn-primary" type="submit">
          {carreraEditar
            ? "Actualizar carrera"
            : "Guardar carrera"}
        </button>

        {carreraEditar && (
          <button
            className="btn btn-secondary"
            type="button"
            onClick={() => {
              limpiarFormulario();
              onCancelarEdicion();
            }}
          >
            Cancelar
          </button>
        )}
      </div>
    </form>
  );
}