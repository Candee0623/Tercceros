import { useEffect, useState } from "react";
import type { Profesor } from "../types/Profesor";
import {
  createProfesor,
  updateProfesor,
  type CreateProfesorRequest,
  type UpdateProfesorRequest,
} from "../services/profesorService";

interface Props {
  profesorEditar?: Profesor | null;
  onGuardado: () => void;
  onCancelarEdicion: () => void;
}

export default function ProfesorForm({
  profesorEditar,
  onGuardado,
  onCancelarEdicion,
}: Props) {
  const [nombre, setNombre] = useState("");
  const [apellido, setApellido] = useState("");
  const [dni, setDni] = useState("");
  const [email, setEmail] = useState("");
  const [telefono, setTelefono] = useState("");
  const [titulo, setTitulo] = useState("");
  const [activo, setActivo] = useState(true);

  useEffect(() => {
    if (profesorEditar) {
      setNombre(profesorEditar.nombre);
      setApellido(profesorEditar.apellido);
      setDni(profesorEditar.dni);
      setEmail(profesorEditar.email ?? "");
      setTelefono(profesorEditar.telefono ?? "");
      setTitulo(profesorEditar.titulo ?? "");
      setActivo(profesorEditar.activo);
    } else {
      limpiarFormulario();
    }
  }, [profesorEditar]);

  function limpiarFormulario() {
    setNombre("");
    setApellido("");
    setDni("");
    setEmail("");
    setTelefono("");
    setTitulo("");
    setActivo(true);
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();

    try {
      if (profesorEditar) {
        const profesorActualizado: UpdateProfesorRequest = {
          nombre,
          apellido,
          dni,
          email: email || null,
          telefono: telefono || null,
          titulo: titulo || null,
          activo,
        };

        await updateProfesor(
          profesorEditar.id,
          profesorActualizado
        );
      } else {
        const nuevoProfesor: CreateProfesorRequest = {
          nombre,
          apellido,
          dni,
          email: email || null,
          telefono: telefono || null,
          titulo: titulo || null,
        };

        await createProfesor(nuevoProfesor);
      }

      limpiarFormulario();
      onGuardado();
    } catch (error) {
      console.error(error);

      alert(
        profesorEditar
          ? "No se pudo actualizar el profesor"
          : "No se pudo crear el profesor"
      );
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      <h2>
        {profesorEditar
          ? "Editar profesor"
          : "Nuevo profesor"}
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
          <label>Apellido</label>
          <input
            value={apellido}
            onChange={(e) => setApellido(e.target.value)}
            required
          />
        </div>

        <div className="form-group">
          <label>DNI</label>
          <input
            value={dni}
            onChange={(e) => setDni(e.target.value)}
            required
          />
        </div>

        <div className="form-group">
          <label>Email</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </div>

        <div className="form-group">
          <label>Teléfono</label>
          <input
            value={telefono}
            onChange={(e) => setTelefono(e.target.value)}
          />
        </div>

        <div className="form-group">
          <label>Título</label>
          <input
            value={titulo}
            onChange={(e) => setTitulo(e.target.value)}
          />
        </div>
      </div>

      {profesorEditar && (
        <div className="form-group">
          <label>
            <input
              type="checkbox"
              checked={activo}
              onChange={(e) =>
                setActivo(e.target.checked)
              }
            />
            Activo
          </label>
        </div>
      )}

      <div className="form-actions">
        <button
          className="btn btn-primary"
          type="submit"
        >
          {profesorEditar
            ? "Actualizar profesor"
            : "Guardar profesor"}
        </button>

        {profesorEditar && (
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