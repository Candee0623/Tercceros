import { useEffect, useState } from "react";
import FullCalendar from "@fullcalendar/react";
import dayGridPlugin from "@fullcalendar/daygrid";
import timeGridPlugin from "@fullcalendar/timegrid";
import interactionPlugin from "@fullcalendar/interaction";
import type { DateClickArg } from "@fullcalendar/interaction";
import type { EventClickArg } from "@fullcalendar/core";

import {
  getEventos,
  createEvento,
  updateEvento,
  deleteEvento,
  type EventoCalendario,
  type CrearEventoRequest,
} from "../services/calendarioService";

interface FormularioEvento {
  id?: string;
  title: string;
  description: string;
  startTime: string;
  endTime: string;
  isAllDay: boolean;
  location: string;
}

const formularioInicial: FormularioEvento = {
  title: "",
  description: "",
  startTime: "",
  endTime: "",
  isAllDay: false,
  location: "",
};

function convertirAFechaLocalISO(fecha: Date): string {
  const year = fecha.getFullYear();
  const month = String(fecha.getMonth() + 1).padStart(2, "0");
  const day = String(fecha.getDate()).padStart(2, "0");
  const hours = String(fecha.getHours()).padStart(2, "0");
  const minutes = String(fecha.getMinutes()).padStart(2, "0");

  return `${year}-${month}-${day}T${hours}:${minutes}`;
}

function convertirEventoAFormulario(
  evento: EventoCalendario
): FormularioEvento {
  return {
    id: evento.id,
    title: evento.title,
    description: evento.description ?? "",
    startTime: evento.startTime.slice(0, 16),
    endTime: evento.endTime.slice(0, 16),
    isAllDay: evento.isAllDay,
    location: evento.location ?? "",
  };
}

function convertirFormularioARequest(
  formulario: FormularioEvento
): CrearEventoRequest {
  return {
    title: formulario.title,
    description: formulario.description || null,
    startTime: `${formulario.startTime}:00`,
    endTime: `${formulario.endTime}:00`,
    isAllDay: formulario.isAllDay,
    location: formulario.location || null,
  };
}

export default function CalendarioPage() {
  const [eventos, setEventos] = useState<EventoCalendario[]>([]);
  const [formulario, setFormulario] =
    useState<FormularioEvento>(formularioInicial);

  const [modalAbierto, setModalAbierto] = useState(false);
  const [cargando, setCargando] = useState(true);
  const [guardando, setGuardando] = useState(false);
  const [error, setError] = useState("");

  async function cargarEventos() {
    try {
      setCargando(true);
      setError("");

      const datos = await getEventos();
      setEventos(datos);
    } catch (err) {
      console.error(err);
      setError(
        err instanceof Error
          ? err.message
          : "No se pudieron cargar los eventos."
      );
    } finally {
      setCargando(false);
    }
  }

  useEffect(() => {
    cargarEventos();
  }, []);

  function abrirNuevoEvento(fecha?: Date) {
    const inicio = fecha ?? new Date();

    const inicioLocal = convertirAFechaLocalISO(inicio);

    const fin = new Date(inicio.getTime() + 60 * 60 * 1000);
    const finLocal = convertirAFechaLocalISO(fin);

    setFormulario({
      ...formularioInicial,
      startTime: inicioLocal,
      endTime: finLocal,
    });

    setError("");
    setModalAbierto(true);
  }

  function manejarClickFecha(info: DateClickArg) {
    abrirNuevoEvento(info.date);
  }

  function manejarClickEvento(info: EventClickArg) {
    const evento = eventos.find(
      (item) => item.id === info.event.id
    );

    if (!evento) {
      return;
    }

    setFormulario(convertirEventoAFormulario(evento));
    setError("");
    setModalAbierto(true);
  }

  function cerrarModal() {
    if (guardando) {
      return;
    }

    setModalAbierto(false);
    setFormulario(formularioInicial);
    setError("");
  }

  function actualizarCampo(
    campo: keyof FormularioEvento,
    valor: string | boolean
  ) {
    setFormulario((actual) => ({
      ...actual,
      [campo]: valor,
    }));
  }

  async function guardarEvento(
    e: React.FormEvent<HTMLFormElement>
  ) {
    e.preventDefault();

    if (!formulario.title.trim()) {
      setError("El título es obligatorio.");
      return;
    }

    if (!formulario.startTime || !formulario.endTime) {
      setError("La fecha y hora son obligatorias.");
      return;
    }

    if (formulario.endTime <= formulario.startTime) {
      setError(
        "La fecha de finalización debe ser posterior a la fecha de inicio."
      );
      return;
    }

    try {
      setGuardando(true);
      setError("");

      const datos = convertirFormularioARequest(formulario);

      if (formulario.id) {
        await updateEvento(formulario.id, datos);
      } else {
        await createEvento(datos);
      }

      await cargarEventos();

      setModalAbierto(false);
      setFormulario(formularioInicial);
    } catch (err) {
      console.error(err);

      setError(
        err instanceof Error
          ? err.message
          : "No se pudo guardar el evento."
      );
    } finally {
      setGuardando(false);
    }
  }

  async function eliminarEvento() {
    if (!formulario.id) {
      return;
    }

    const confirmar = window.confirm(
      "¿Estás seguro de que querés eliminar este evento?"
    );

    if (!confirmar) {
      return;
    }

    try {
      setGuardando(true);
      setError("");

      await deleteEvento(formulario.id);
      await cargarEventos();

      setModalAbierto(false);
      setFormulario(formularioInicial);
    } catch (err) {
      console.error(err);

      setError(
        err instanceof Error
          ? err.message
          : "No se pudo eliminar el evento."
      );
    } finally {
      setGuardando(false);
    }
  }

  const eventosFullCalendar = eventos.map((evento) => ({
    id: evento.id,
    title: evento.title,
    start: evento.startTime,
    end: evento.endTime,
    allDay: evento.isAllDay,
    extendedProps: {
      description: evento.description,
      location: evento.location,
    },
  }));

  return (
    <div
      style={{
        padding: "24px",
        maxWidth: "1400px",
        margin: "0 auto",
      }}
    >
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginBottom: "20px",
          gap: "16px",
        }}
      >
        <div>
          <h1
            style={{
              margin: 0,
              fontSize: "28px",
            }}
          >
            Calendario
          </h1>

          <p
            style={{
              marginTop: "6px",
              color: "#666",
            }}
          >
            Organizá tus eventos y actividades.
          </p>
        </div>

        <button
          type="button"
          onClick={() => abrirNuevoEvento()}
          style={{
            border: "none",
            borderRadius: "8px",
            padding: "10px 16px",
            cursor: "pointer",
            fontWeight: 600,
          }}
        >
          + Nuevo evento
        </button>
      </div>

      {error && !modalAbierto && (
        <div
          style={{
            marginBottom: "16px",
            padding: "12px",
            borderRadius: "8px",
            background: "#ffecec",
            border: "1px solid #f5b5b5",
          }}
        >
          {error}
        </div>
      )}

      {cargando ? (
        <div style={{ padding: "30px", textAlign: "center" }}>
          Cargando calendario...
        </div>
      ) : (
        <div
          style={{
            background: "white",
            borderRadius: "10px",
            padding: "16px",
          }}
        >
          <FullCalendar
            plugins={[
              dayGridPlugin,
              timeGridPlugin,
              interactionPlugin,
            ]}
            initialView="dayGridMonth"
            locale="es"
            headerToolbar={{
              left: "prev,next today",
              center: "title",
              right: "dayGridMonth,timeGridWeek,timeGridDay",
            }}
            buttonText={{
              today: "Hoy",
              month: "Mes",
              week: "Semana",
              day: "Día",
            }}
            events={eventosFullCalendar}
            dateClick={manejarClickFecha}
            eventClick={manejarClickEvento}
            height="auto"
            dayMaxEvents={3}
            nowIndicator
          />
        </div>
      )}

      {modalAbierto && (
        <div
          onMouseDown={(e) => {
            if (e.target === e.currentTarget) {
              cerrarModal();
            }
          }}
          style={{
            position: "fixed",
            inset: 0,
            background: "rgba(0, 0, 0, 0.45)",
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            padding: "20px",
            zIndex: 1000,
          }}
        >
          <div
            style={{
              background: "white",
              borderRadius: "12px",
              width: "100%",
              maxWidth: "520px",
              maxHeight: "90vh",
              overflowY: "auto",
              padding: "24px",
            }}
          >
            <div
              style={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                marginBottom: "20px",
              }}
            >
              <h2 style={{ margin: 0 }}>
                {formulario.id
                  ? "Editar evento"
                  : "Nuevo evento"}
              </h2>

              <button
                type="button"
                onClick={cerrarModal}
                disabled={guardando}
                style={{
                  border: "none",
                  background: "transparent",
                  fontSize: "24px",
                  cursor: "pointer",
                }}
              >
                ×
              </button>
            </div>

            {error && (
              <div
                style={{
                  marginBottom: "16px",
                  padding: "10px",
                  borderRadius: "8px",
                  background: "#ffecec",
                  border: "1px solid #f5b5b5",
                }}
              >
                {error}
              </div>
            )}

            <form onSubmit={guardarEvento}>
              <div style={{ marginBottom: "14px" }}>
                <label
                  htmlFor="title"
                  style={{
                    display: "block",
                    marginBottom: "6px",
                    fontWeight: 600,
                  }}
                >
                  Título *
                </label>

                <input
                  id="title"
                  type="text"
                  maxLength={150}
                  value={formulario.title}
                  onChange={(e) =>
                    actualizarCampo("title", e.target.value)
                  }
                  required
                  style={{
                    width: "100%",
                    boxSizing: "border-box",
                    padding: "10px",
                    borderRadius: "6px",
                    border: "1px solid #ccc",
                  }}
                />
              </div>

              <div style={{ marginBottom: "14px" }}>
                <label
                  htmlFor="description"
                  style={{
                    display: "block",
                    marginBottom: "6px",
                    fontWeight: 600,
                  }}
                >
                  Descripción
                </label>

                <textarea
                  id="description"
                  value={formulario.description}
                  onChange={(e) =>
                    actualizarCampo(
                      "description",
                      e.target.value
                    )
                  }
                  rows={3}
                  style={{
                    width: "100%",
                    boxSizing: "border-box",
                    padding: "10px",
                    borderRadius: "6px",
                    border: "1px solid #ccc",
                    resize: "vertical",
                  }}
                />
              </div>

              <div style={{ marginBottom: "14px" }}>
                <label
                  htmlFor="startTime"
                  style={{
                    display: "block",
                    marginBottom: "6px",
                    fontWeight: 600,
                  }}
                >
                  Inicio *
                </label>

                <input
                  id="startTime"
                  type="datetime-local"
                  value={formulario.startTime}
                  onChange={(e) =>
                    actualizarCampo(
                      "startTime",
                      e.target.value
                    )
                  }
                  required
                  style={{
                    width: "100%",
                    boxSizing: "border-box",
                    padding: "10px",
                    borderRadius: "6px",
                    border: "1px solid #ccc",
                  }}
                />
              </div>

              <div style={{ marginBottom: "14px" }}>
                <label
                  htmlFor="endTime"
                  style={{
                    display: "block",
                    marginBottom: "6px",
                    fontWeight: 600,
                  }}
                >
                  Fin *
                </label>

                <input
                  id="endTime"
                  type="datetime-local"
                  value={formulario.endTime}
                  onChange={(e) =>
                    actualizarCampo(
                      "endTime",
                      e.target.value
                    )
                  }
                  required
                  style={{
                    width: "100%",
                    boxSizing: "border-box",
                    padding: "10px",
                    borderRadius: "6px",
                    border: "1px solid #ccc",
                  }}
                />
              </div>

              <div style={{ marginBottom: "14px" }}>
                <label
                  style={{
                    display: "flex",
                    alignItems: "center",
                    gap: "8px",
                    cursor: "pointer",
                  }}
                >
                  <input
                    type="checkbox"
                    checked={formulario.isAllDay}
                    onChange={(e) =>
                      actualizarCampo(
                        "isAllDay",
                        e.target.checked
                      )
                    }
                  />

                  Todo el día
                </label>
              </div>

              <div style={{ marginBottom: "20px" }}>
                <label
                  htmlFor="location"
                  style={{
                    display: "block",
                    marginBottom: "6px",
                    fontWeight: 600,
                  }}
                >
                  Ubicación
                </label>

                <input
                  id="location"
                  type="text"
                  maxLength={100}
                  value={formulario.location}
                  onChange={(e) =>
                    actualizarCampo(
                      "location",
                      e.target.value
                    )
                  }
                  style={{
                    width: "100%",
                    boxSizing: "border-box",
                    padding: "10px",
                    borderRadius: "6px",
                    border: "1px solid #ccc",
                  }}
                />
              </div>

              <div
                style={{
                  display: "flex",
                  justifyContent: "space-between",
                  gap: "10px",
                }}
              >
                <div>
                  {formulario.id && (
                    <button
                      type="button"
                      onClick={eliminarEvento}
                      disabled={guardando}
                      style={{
                        padding: "10px 14px",
                        borderRadius: "6px",
                        border: "1px solid #d33",
                        cursor: "pointer",
                      }}
                    >
                      Eliminar
                    </button>
                  )}
                </div>

                <div
                  style={{
                    display: "flex",
                    gap: "10px",
                  }}
                >
                  <button
                    type="button"
                    onClick={cerrarModal}
                    disabled={guardando}
                    style={{
                      padding: "10px 14px",
                      borderRadius: "6px",
                      border: "1px solid #ccc",
                      cursor: "pointer",
                    }}
                  >
                    Cancelar
                  </button>

                  <button
                    type="submit"
                    disabled={guardando}
                    style={{
                      padding: "10px 14px",
                      borderRadius: "6px",
                      border: "none",
                      cursor: "pointer",
                      fontWeight: 600,
                    }}
                  >
                    {guardando
                      ? "Guardando..."
                      : formulario.id
                        ? "Guardar cambios"
                        : "Crear evento"}
                  </button>
                </div>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}