import { useEffect, useState } from "react";
import type { Alumno } from "../types/Alumno";
import {
  createAlumno,
  updateAlumno,
  type CreateAlumnoRequest,
  type UpdateAlumnoRequest,
} from "../services/alumnoService";
import { getCarreras } from "../services/carreraService";
import type { Carrera } from "../types/Carrera";

interface Props {
  alumnoEditar?: Alumno | null;
  onGuardado: () => void;
  onCancelarEdicion: () => void;
}

export default function AlumnoForm({
  alumnoEditar,
  onGuardado,
  onCancelarEdicion,
}: Props) {
  const [nombre, setNombre] = useState("");
  const [apellido, setApellido] = useState("");
  const [dni, setDni] = useState("");
  const [email, setEmail] = useState("");
  const [telefono, setTelefono] = useState("");
  const [fechaNacimiento, setFechaNacimiento] = useState("");
  const [carreraId, setCarreraId] = useState("");
  const [activo, setActivo] = useState(true);

  const [carreras, setCarreras] = useState<Carrera[]>([]);

  useEffect(() => {
    cargarCarreras();
  }, []);

  useEffect(() => {
    if (alumnoEditar) {
      setNombre(alumnoEditar.nombre);
      setApellido(alumnoEditar.apellido);
      setDni(alumnoEditar.dni);
      setEmail(alumnoEditar.email ?? "");
      setTelefono(alumnoEditar.telefono ?? "");
      setCarreraId(alumnoEditar.carreraId ?? "");
      setActivo(alumnoEditar.activo);

      setFechaNacimiento(
        alumnoEditar.fechaNacimiento
          ? alumnoEditar.fechaNacimiento.substring(0, 10)
          : ""
      );
    } else {
      limpiarFormulario();
    }
  }, [alumnoEditar]);

  async function cargarCarreras() {
    try {
      const data = await getCarreras();
      setCarreras(data);
    } catch (error) {
      console.error("Error cargando carreras:", error);
    }
  }

  function limpiarFormulario() {
    setNombre("");
    setApellido("");
    setDni("");
    setEmail("");
    setTelefono("");
    setFechaNacimiento("");
    setCarreraId("");
    setActivo(true);
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();

    try {
      const fechaNacimientoUtc =
        `${fechaNacimiento}T00:00:00Z`;

      if (alumnoEditar) {
        const alumnoActualizado: UpdateAlumnoRequest = {
          nombre,
          apellido,
          dni,
          email: email || null,
          telefono: telefono || null,
          fechaNacimiento: fechaNacimientoUtc,
          carreraId: carreraId || null,
          activo,
        };

        await updateAlumno(
          alumnoEditar.id,
          alumnoActualizado
        );
      } else {
        const nuevoAlumno: CreateAlumnoRequest = {
          nombre,
          apellido,
          dni,
          email: email || null,
          telefono: telefono || null,
          fechaNacimiento: fechaNacimientoUtc,
          carreraId: carreraId || null,
        };

        await createAlumno(nuevoAlumno);
      }

      limpiarFormulario();
      onGuardado();
    } catch (error) {
      console.error(error);
      alert(
        alumnoEditar
          ? "No se pudo actualizar el alumno"
          : "No se pudo crear el alumno"
      );
    }
  }

  return (
    <form onSubmit={handleSubmit}>
  <h2>
    {alumnoEditar ? "Editar alumno" : "Nuevo alumno"}
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
      <label>Fecha de nacimiento</label>
      <input
        type="date"
        value={fechaNacimiento}
        onChange={(e) => setFechaNacimiento(e.target.value)}
        required
      />
    </div>

    <div className="form-group">
      <label>Carrera</label>
      <select
        value={carreraId}
        onChange={(e) => setCarreraId(e.target.value)}
      >
        <option value="">Sin carrera</option>

        {carreras.map((carrera) => (
          <option key={carrera.id} value={carrera.id}>
            {carrera.nombre}
          </option>
        ))}
      </select>
    </div>
  </div>

  <div className="form-actions">
    <button className="btn btn-primary" type="submit">
      {alumnoEditar ? "Actualizar alumno" : "Guardar alumno"}
    </button>

    {alumnoEditar && (
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