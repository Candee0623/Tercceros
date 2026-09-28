import { useEffect, useState } from "react";
import { getSession } from "../auth/authService";
import { getCarreras } from "../services/carreraService";
import {
enviarMensaje,
obtenerBandeja,
obtenerNoLeidos,
obtenerDetalle,
marcarComoLeido,
obtenerEnviados,
obtenerCursadasProfesor,
obtenerCursadasDisponibles,
type MensajeBandeja,
type MensajeEnviado,
type MensajeCursada,
} from "../services/mensajesService";

const TIPOS = {
TODOS: 1,
CARRERA: 2,
CARRERA_ANIO: 3,
CURSADA: 4,
};

interface CarreraOption {
id: string;
nombre: string;
duracionAnios: number;
}

export default function MensajesPage() {
const session = getSession();

const esAlumno =
session?.rol?.toUpperCase() === "ALUMNO" ||
!!session?.alumnoId;

const esProfesor =
session?.rol?.toUpperCase() === "PROFESOR";

const puedeEnviar = !esAlumno;

const [vista, setVista] = useState<"bandeja" | "enviados">(
esAlumno ? "bandeja" : "enviados"
);

const [mensajes, setMensajes] = useState<MensajeBandeja[]>([]);
const [enviados, setEnviados] = useState<MensajeEnviado[]>([]);
const [noLeidos, setNoLeidos] = useState(0);

const [carreras, setCarreras] = useState<CarreraOption[]>([]);
const [cursadas, setCursadas] = useState<MensajeCursada[]>([]);

const [asunto, setAsunto] = useState("");
const [contenido, setContenido] = useState("");

const [tipoDestinatario, setTipoDestinatario] = useState(
esProfesor ? TIPOS.CURSADA : TIPOS.TODOS
);

const [carreraId, setCarreraId] = useState("");
const [anio, setAnio] = useState("");
const [cursadaId, setCursadaId] = useState("");

const [mensajeSeleccionado, setMensajeSeleccionado] =
useState<MensajeBandeja | null>(null);

const [cargando, setCargando] = useState(false);
const [enviando, setEnviando] = useState(false);
const [error, setError] = useState("");
const [exito, setExito] = useState("");

useEffect(() => {
void cargarDatosIniciales();
}, []);

async function cargarDatosIniciales() {
setError("");

if (esAlumno) {
  await cargarBandeja();
  return;
}

await Promise.all([
  cargarEnviados(),
  cargarOpcionesEnvio(),
]);

}

async function cargarBandeja() {
try {
setCargando(true);
setError("");

  const [data, cantidad] = await Promise.all([
    obtenerBandeja(),
    obtenerNoLeidos(),
  ]);

  setMensajes(data);
  setNoLeidos(cantidad);
} catch (err) {
  setError(
    err instanceof Error
      ? err.message
      : "No se pudo cargar la bandeja."
  );
} finally {
  setCargando(false);
}

}

async function cargarEnviados() {
try {
setCargando(true);
setError("");

  const data = await obtenerEnviados();
  setEnviados(data);
} catch (err) {
  setError(
    err instanceof Error
      ? err.message
      : "No se pudieron cargar los mensajes enviados."
  );
} finally {
  setCargando(false);
}

}

async function cargarOpcionesEnvio() {
try {
setError("");

  if (!esProfesor) {
    const carrerasData = await getCarreras();

    setCarreras(
      carrerasData
        .filter((carrera) => carrera.activa)
        .map((carrera) => ({
          id: carrera.id,
          nombre: carrera.nombre,
          duracionAnios: carrera.duracionAnios,
        }))
    );

    const cursadasData =
      await obtenerCursadasDisponibles();

    setCursadas(cursadasData);
  } else {
    const cursadasData =
      await obtenerCursadasProfesor();

    setCursadas(cursadasData);
  }
} catch (err) {
  setError(
    err instanceof Error
      ? err.message
      : "No se pudieron cargar las opciones de destinatarios."
  );
}

}

async function abrirMensaje(id: string) {
try {
setError("");

  const detalle = await obtenerDetalle(id);
  setMensajeSeleccionado(detalle);

  if (!detalle.leido) {
    await marcarComoLeido(id);

    setMensajes((actuales) =>
      actuales.map((mensaje) =>
        mensaje.id === id
          ? { ...mensaje, leido: true }
          : mensaje
      )
    );

    setNoLeidos((cantidad) =>
      Math.max(0, cantidad - 1)
    );
  }
} catch (err) {
  setError(
    err instanceof Error
      ? err.message
      : "No se pudo abrir el mensaje."
  );
}

}

function limpiarFormulario() {
setAsunto("");
setContenido("");
setCarreraId("");
setAnio("");
setCursadaId("");

setTipoDestinatario(
  esProfesor ? TIPOS.CURSADA : TIPOS.TODOS
);

}

async function handleEnviar(e: React.FormEvent) {
e.preventDefault();

setError("");
setExito("");

if (!asunto.trim()) {
  setError("El asunto es obligatorio.");
  return;
}

if (!contenido.trim()) {
  setError("El contenido es obligatorio.");
  return;
}

if (
  tipoDestinatario === TIPOS.CARRERA &&
  !carreraId
) {
  setError("Debés seleccionar una carrera.");
  return;
}

if (
  tipoDestinatario === TIPOS.CARRERA_ANIO &&
  (!carreraId || !anio)
) {
  setError("Debés seleccionar una carrera y un año.");
  return;
}

if (
  tipoDestinatario === TIPOS.CURSADA &&
  !cursadaId
) {
  setError("Debés seleccionar una cursada.");
  return;
}

if (tipoDestinatario === TIPOS.CARRERA_ANIO) {
  const carreraSeleccionada = carreras.find(
    (carrera) => carrera.id === carreraId
  );

  if (!carreraSeleccionada) {
    setError("La carrera seleccionada no es válida.");
    return;
  }

  const anioSeleccionado = Number(anio);

  if (
    anioSeleccionado < 1 ||
    anioSeleccionado > carreraSeleccionada.duracionAnios
  ) {
    setError(
      `El año debe estar entre 1 y ${carreraSeleccionada.duracionAnios}.`
    );
    return;
  }
}

try {
  setEnviando(true);

  await enviarMensaje({
    asunto: asunto.trim(),
    contenido: contenido.trim(),
    tipoDestinatario,
    carreraId:
      tipoDestinatario === TIPOS.CARRERA ||
      tipoDestinatario === TIPOS.CARRERA_ANIO
        ? carreraId
        : null,
    anio:
      tipoDestinatario === TIPOS.CARRERA_ANIO
        ? Number(anio)
        : null,
    cursadaId:
      tipoDestinatario === TIPOS.CURSADA
        ? cursadaId
        : null,
  });

  limpiarFormulario();

  setExito("Mensaje enviado correctamente.");

  await cargarEnviados();

  setVista("enviados");
} catch (err) {
  setError(
    err instanceof Error
      ? err.message
      : "No se pudo enviar el mensaje."
  );
} finally {
  setEnviando(false);
}

}

function cambiarTipoDestinatario(valor: number) {
setTipoDestinatario(valor);
setCarreraId("");
setAnio("");
setCursadaId("");
setError("");
}

function cambiarCarrera(valor: string) {
setCarreraId(valor);
setAnio("");
setError("");
}

function descripcionCursada(cursada: MensajeCursada) {
return `${cursada.materiaNombre} - ${cursada.cicloLectivo} - ${cursada.periodo}`;
}

function descripcionTipo(tipo: string) {
switch (tipo) {
case "Todos":
return "Todos los alumnos";
case "Carrera":
return "Carrera";
case "CarreraAnio":
return "Carrera y año";
case "Cursada":
return "Cursada";
default:
return tipo;
}
}

const carreraSeleccionada = carreras.find(
(carrera) => carrera.id === carreraId
);

const cantidadAnios =
carreraSeleccionada?.duracionAnios ?? 0;

return (
<div style={{ padding: "24px" }}>
<div
style={{
display: "flex",
justifyContent: "space-between",
alignItems: "center",
marginBottom: "24px",
gap: "16px",
flexWrap: "wrap",
}}
> <div>
<h1 style={{ margin: 0 }}>Mensajes</h1>

      {esAlumno && (
        <p
          style={{
            marginTop: "8px",
            color: "#666",
          }}
        >
          Bandeja de mensajes institucionales
        </p>
      )}

      {!esAlumno && (
        <p
          style={{
            marginTop: "8px",
            color: "#666",
          }}
        >
          Administración de mensajes institucionales
        </p>
      )}
    </div>

    {esAlumno && (
      <div
        style={{
          padding: "8px 14px",
          borderRadius: "20px",
          backgroundColor:
            noLeidos > 0 ? "#dc3545" : "#6c757d",
          color: "white",
          fontWeight: 600,
        }}
      >
        {noLeidos} sin leer
      </div>
    )}
  </div>

  {error && (
    <div
      style={{
        marginBottom: "16px",
        padding: "12px 16px",
        borderRadius: "6px",
        backgroundColor: "#f8d7da",
        color: "#842029",
        border: "1px solid #f5c2c7",
      }}
    >
      {error}
    </div>
  )}

  {exito && (
    <div
      style={{
        marginBottom: "16px",
        padding: "12px 16px",
        borderRadius: "6px",
        backgroundColor: "#d1e7dd",
        color: "#0f5132",
        border: "1px solid #badbcc",
      }}
    >
      {exito}
    </div>
  )}

  <div
    style={{
      display: "flex",
      gap: "8px",
      marginBottom: "24px",
      borderBottom: "1px solid #ddd",
      paddingBottom: "10px",
    }}
  >
    {esAlumno && (
      <button
        type="button"
        onClick={() => {
          setVista("bandeja");
          setMensajeSeleccionado(null);
          void cargarBandeja();
        }}
        style={{
          padding: "10px 16px",
          border: "none",
          borderRadius: "6px",
          cursor: "pointer",
          backgroundColor:
            vista === "bandeja"
              ? "#0d6efd"
              : "#e9ecef",
          color:
            vista === "bandeja"
              ? "white"
              : "#212529",
        }}
      >
        Bandeja
      </button>
    )}

    {!esAlumno && (
      <>
        <button
          type="button"
          onClick={() => {
            setVista("enviados");
            setMensajeSeleccionado(null);
            void cargarEnviados();
          }}
          style={{
            padding: "10px 16px",
            border: "none",
            borderRadius: "6px",
            cursor: "pointer",
            backgroundColor:
              vista === "enviados"
                ? "#0d6efd"
                : "#e9ecef",
            color:
              vista === "enviados"
                ? "white"
                : "#212529",
          }}
        >
          Enviados
        </button>

        {puedeEnviar && (
          <button
            type="button"
            onClick={() => {
              setVista("enviados");
              setMensajeSeleccionado(null);
              setExito("");
              setError("");
            }}
            style={{
              padding: "10px 16px",
              border: "none",
              borderRadius: "6px",
              cursor: "pointer",
              backgroundColor: "#198754",
              color: "white",
            }}
          >
            Nuevo mensaje
          </button>
        )}
      </>
    )}
  </div>

  {vista === "bandeja" && esAlumno && (
    <div>
      {cargando ? (
        <p>Cargando mensajes...</p>
      ) : mensajes.length === 0 ? (
        <div
          style={{
            padding: "32px",
            textAlign: "center",
            border: "1px solid #ddd",
            borderRadius: "8px",
            color: "#666",
          }}
        >
          No tenés mensajes recibidos.
        </div>
      ) : (
        <div
          style={{
            display: "flex",
            flexDirection: "column",
            gap: "10px",
          }}
        >
          {mensajes.map((mensaje) => (
            <button
              key={mensaje.id}
              type="button"
              onClick={() =>
                void abrirMensaje(mensaje.id)
              }
              style={{
                textAlign: "left",
                padding: "16px",
                borderRadius: "8px",
                border: "1px solid #ddd",
                backgroundColor: mensaje.leido
                  ? "white"
                  : "#eef5ff",
                cursor: "pointer",
              }}
            >
              <div
                style={{
                  display: "flex",
                  justifyContent: "space-between",
                  gap: "16px",
                }}
              >
                <strong>{mensaje.asunto}</strong>

                <span
                  style={{
                    fontSize: "13px",
                    color: "#666",
                  }}
                >
                  {new Date(
                    mensaje.fechaEnvio
                  ).toLocaleString()}
                </span>
              </div>

              <div
                style={{
                  marginTop: "6px",
                  color: "#555",
                }}
              >
                De: {mensaje.remitente}
              </div>

              {!mensaje.leido && (
                <div
                  style={{
                    marginTop: "8px",
                    fontWeight: 600,
                    color: "#0d6efd",
                  }}
                >
                  Sin leer
                </div>
              )}
            </button>
          ))}
        </div>
      )}
    </div>
  )}

  {!esAlumno && vista === "enviados" && (
    <div
      style={{
        display: "grid",
        gridTemplateColumns:
          "minmax(0, 1fr) minmax(320px, 420px)",
        gap: "24px",
      }}
    >
      <div>
        <h2>Mensajes enviados</h2>

        {cargando ? (
          <p>Cargando mensajes...</p>
        ) : enviados.length === 0 ? (
          <div
            style={{
              padding: "32px",
              border: "1px solid #ddd",
              borderRadius: "8px",
              color: "#666",
            }}
          >
            Todavía no enviaste mensajes.
          </div>
        ) : (
          <div
            style={{
              display: "flex",
              flexDirection: "column",
              gap: "10px",
            }}
          >
            {enviados.map((mensaje) => (
              <div
                key={mensaje.id}
                style={{
                  padding: "16px",
                  border: "1px solid #ddd",
                  borderRadius: "8px",
                  backgroundColor: "white",
                }}
              >
                <strong>{mensaje.asunto}</strong>

                <div
                  style={{
                    marginTop: "8px",
                    color: "#555",
                  }}
                >
                  Destinatario:{" "}
                  {mensaje.destinatarioDescripcion}
                </div>

                <div
                  style={{
                    marginTop: "4px",
                    color: "#555",
                  }}
                >
                  Tipo:{" "}
                  {descripcionTipo(
                    mensaje.tipoDestinatario
                  )}
                </div>

                <div
                  style={{
                    marginTop: "4px",
                    color: "#555",
                  }}
                >
                  Destinatarios:{" "}
                  {mensaje.cantidadDestinatarios}
                </div>

                <div
                  style={{
                    marginTop: "8px",
                    fontSize: "13px",
                    color: "#777",
                  }}
                >
                  {new Date(
                    mensaje.fechaEnvio
                  ).toLocaleString()}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {puedeEnviar && (
        <div
          style={{
            border: "1px solid #ddd",
            borderRadius: "8px",
            padding: "20px",
            backgroundColor: "#fff",
            alignSelf: "start",
          }}
        >
          <h2 style={{ marginTop: 0 }}>
            Nuevo mensaje
          </h2>

          <form onSubmit={handleEnviar}>
            <div style={{ marginBottom: "16px" }}>
              <label
                htmlFor="asunto"
                style={{
                  display: "block",
                  marginBottom: "6px",
                  fontWeight: 600,
                }}
              >
                Asunto
              </label>

              <input
                id="asunto"
                type="text"
                value={asunto}
                onChange={(e) =>
                  setAsunto(e.target.value)
                }
                maxLength={150}
                style={{
                  width: "100%",
                  boxSizing: "border-box",
                  padding: "10px",
                  border: "1px solid #ccc",
                  borderRadius: "6px",
                }}
              />
            </div>

            <div style={{ marginBottom: "16px" }}>
              <label
                htmlFor="tipoDestinatario"
                style={{
                  display: "block",
                  marginBottom: "6px",
                  fontWeight: 600,
                }}
              >
                Destinatario
              </label>

              <select
                id="tipoDestinatario"
                value={tipoDestinatario}
                onChange={(e) =>
                  cambiarTipoDestinatario(
                    Number(e.target.value)
                  )
                }
                disabled={esProfesor}
                style={{
                  width: "100%",
                  boxSizing: "border-box",
                  padding: "10px",
                  border: "1px solid #ccc",
                  borderRadius: "6px",
                }}
              >
                {!esProfesor && (
                  <>
                    <option value={TIPOS.TODOS}>
                      Todos los alumnos
                    </option>

                    <option value={TIPOS.CARRERA}>
                      Una carrera
                    </option>

                    <option value={TIPOS.CARRERA_ANIO}>
                      Carrera y año
                    </option>
                  </>
                )}

                <option value={TIPOS.CURSADA}>
                  Cursada
                </option>
              </select>
            </div>

            {(tipoDestinatario === TIPOS.CARRERA ||
              tipoDestinatario === TIPOS.CARRERA_ANIO) && (
              <div style={{ marginBottom: "16px" }}>
                <label
                  htmlFor="carrera"
                  style={{
                    display: "block",
                    marginBottom: "6px",
                    fontWeight: 600,
                  }}
                >
                  Carrera
                </label>

                <select
                  id="carrera"
                  value={carreraId}
                  onChange={(e) =>
                    cambiarCarrera(e.target.value)
                  }
                  style={{
                    width: "100%",
                    boxSizing: "border-box",
                    padding: "10px",
                    border: "1px solid #ccc",
                    borderRadius: "6px",
                  }}
                >
                  <option value="">
                    Seleccionar carrera
                  </option>

                  {carreras.map((carrera) => (
                    <option
                      key={carrera.id}
                      value={carrera.id}
                    >
                      {carrera.nombre} (
                      {carrera.duracionAnios}{" "}
                      {carrera.duracionAnios === 1
                        ? "año"
                        : "años"}
                      )
                    </option>
                  ))}
                </select>
              </div>
            )}

            {tipoDestinatario ===
              TIPOS.CARRERA_ANIO && (
              <div style={{ marginBottom: "16px" }}>
                <label
                  htmlFor="anio"
                  style={{
                    display: "block",
                    marginBottom: "6px",
                    fontWeight: 600,
                  }}
                >
                  Año
                </label>

                <select
                  id="anio"
                  value={anio}
                  onChange={(e) =>
                    setAnio(e.target.value)
                  }
                  disabled={!carreraId}
                  style={{
                    width: "100%",
                    boxSizing: "border-box",
                    padding: "10px",
                    border: "1px solid #ccc",
                    borderRadius: "6px",
                    backgroundColor: !carreraId
                      ? "#e9ecef"
                      : "white",
                  }}
                >
                  <option value="">
                    {!carreraId
                      ? "Primero seleccioná una carrera"
                      : "Seleccionar año"}
                  </option>

                  {Array.from(
                    { length: cantidadAnios },
                    (_, index) => index + 1
                  ).map((numero) => (
                    <option
                      key={numero}
                      value={numero}
                    >
                      {numero}°
                    </option>
                  ))}
                </select>
              </div>
            )}

            {tipoDestinatario ===
              TIPOS.CURSADA && (
              <div style={{ marginBottom: "16px" }}>
                <label
                  htmlFor="cursada"
                  style={{
                    display: "block",
                    marginBottom: "6px",
                    fontWeight: 600,
                  }}
                >
                  Cursada
                </label>

                <select
                  id="cursada"
                  value={cursadaId}
                  onChange={(e) =>
                    setCursadaId(e.target.value)
                  }
                  style={{
                    width: "100%",
                    boxSizing: "border-box",
                    padding: "10px",
                    border: "1px solid #ccc",
                    borderRadius: "6px",
                  }}
                >
                  <option value="">
                    Seleccionar cursada
                  </option>

                  {cursadas.map((cursada) => (
                    <option
                      key={cursada.id}
                      value={cursada.id}
                    >
                      {descripcionCursada(cursada)}
                    </option>
                  ))}
                </select>
              </div>
            )}

            <div style={{ marginBottom: "16px" }}>
              <label
                htmlFor="contenido"
                style={{
                  display: "block",
                  marginBottom: "6px",
                  fontWeight: 600,
                }}
              >
                Mensaje
              </label>

              <textarea
                id="contenido"
                value={contenido}
                onChange={(e) =>
                  setContenido(e.target.value)
                }
                rows={8}
                style={{
                  width: "100%",
                  boxSizing: "border-box",
                  padding: "10px",
                  border: "1px solid #ccc",
                  borderRadius: "6px",
                  resize: "vertical",
                }}
              />
            </div>

            <button
              type="submit"
              disabled={enviando}
              style={{
                width: "100%",
                padding: "11px",
                border: "none",
                borderRadius: "6px",
                backgroundColor: enviando
                  ? "#6c757d"
                  : "#198754",
                color: "white",
                fontWeight: 600,
                cursor: enviando
                  ? "not-allowed"
                  : "pointer",
              }}
            >
              {enviando
                ? "Enviando..."
                : "Enviar mensaje"}
            </button>
          </form>
        </div>
      )}
    </div>
  )}

  {mensajeSeleccionado && (
    <div
      style={{
        position: "fixed",
        inset: 0,
        backgroundColor: "rgba(0, 0, 0, 0.45)",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        padding: "20px",
        zIndex: 1000,
      }}
    >
      <div
        style={{
          width: "100%",
          maxWidth: "650px",
          maxHeight: "80vh",
          overflowY: "auto",
          backgroundColor: "white",
          borderRadius: "10px",
          padding: "24px",
        }}
      >
        <div
          style={{
            display: "flex",
            justifyContent: "space-between",
            gap: "16px",
            alignItems: "flex-start",
          }}
        >
          <h2 style={{ marginTop: 0 }}>
            {mensajeSeleccionado.asunto}
          </h2>

          <button
            type="button"
            onClick={() =>
              setMensajeSeleccionado(null)
            }
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

        <div
          style={{
            marginBottom: "16px",
            color: "#666",
          }}
        >
          <strong>De:</strong>{" "}
          {mensajeSeleccionado.remitente}
        </div>

        <div
          style={{
            marginBottom: "20px",
            color: "#666",
          }}
        >
          <strong>Fecha:</strong>{" "}
          {new Date(
            mensajeSeleccionado.fechaEnvio
          ).toLocaleString()}
        </div>

        <div
          style={{
            whiteSpace: "pre-wrap",
            lineHeight: 1.6,
          }}
        >
          {mensajeSeleccionado.contenido}
        </div>

        <div
          style={{
            marginTop: "24px",
            textAlign: "right",
          }}
        >
          <button
            type="button"
            onClick={() =>
              setMensajeSeleccionado(null)
            }
            style={{
              padding: "10px 18px",
              border: "none",
              borderRadius: "6px",
              backgroundColor: "#6c757d",
              color: "white",
              cursor: "pointer",
            }}
          >
            Cerrar
          </button>
        </div>
      </div>
    </div>
  )}
</div>

);
}
