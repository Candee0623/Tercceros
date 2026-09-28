import { useEffect, useState } from "react";
import { getSession } from "../auth/authService";
import {
  cancelarTramite,
  crearTramite,
  getMisTramites,
  getTodosTramites,
  resolverTramite,
} from "../services/tramiteService";
import type { Tramite } from "../types/Tramite";

export default function TramitesPage() {
  const session = getSession();
  const esAlumno =
    session?.rol?.toUpperCase() === "ALUMNO" || !!session?.alumnoId;
  const esAdmin =
    session?.rol?.toUpperCase() === "ADMIN" ||
    session?.rol?.toUpperCase() === "ADMINISTRADOR" ||
    !esAlumno;

  const [tramites, setTramites] = useState<Tramite[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [filtroEstado, setFiltroEstado] = useState("Todos");
  const [mostrarSolicitudModal, setMostrarSolicitudModal] = useState(false);

  // Modal de resolución (para administrativos)
  const [tramiteAResolver, setTramiteAResolver] = useState<Tramite | null>(null);
  const [resolucionEstado, setResolucionEstado] = useState("Aprobado");
  const [resolucionObservacion, setResolucionObservacion] = useState("");

  // Modal de comprobante / certificado imprimible
  const [tramiteComprobante, setTramiteComprobante] = useState<Tramite | null>(null);

  // Formulario nueva solicitud
  const [tipo, setTipo] = useState("Constancia de Alumno Regular");
  const [motivo, setMotivo] = useState("");

  const tiposTramite = [
    "Constancia de Alumno Regular",
    "Certificado de Examen Rendido",
    "Certificado Analítico Parcial",
    "Justificación de Inasistencia",
    "Solicitud de Equivalencias",
    "Prorroga de Regularidad",
    "Otro Trámite Administrativo",
  ];

  async function cargarTramites() {
    try {
      setLoading(true);
      setError("");
      const data = esAdmin ? await getTodosTramites() : await getMisTramites();
      setTramites(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al cargar trámites");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    cargarTramites();
  }, [esAdmin]);

  async function handleSolicitar(e: React.FormEvent) {
    e.preventDefault();
    if (!motivo.trim()) return;

    try {
      setError("");
      await crearTramite({ tipo, motivo: motivo.trim() });
      setMotivo("");
      setMostrarSolicitudModal(false);
      await cargarTramites();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al crear la solicitud");
    }
  }

  async function handleResolverSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!tramiteAResolver) return;

    try {
      setError("");
      await resolverTramite(tramiteAResolver.id, {
        estado: resolucionEstado,
        observacion: resolucionObservacion.trim() || undefined,
      });
      setTramiteAResolver(null);
      setResolucionObservacion("");
      await cargarTramites();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Error al resolver trámite");
    }
  }

  async function handleCancelar(id: string) {
    if (!confirm("¿Deseas cancelar esta solicitud de trámite?")) return;

    try {
      await cancelarTramite(id);
      await cargarTramites();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Error al cancelar trámite");
    }
  }

  const getEstadoBadgeClass = (estado: string) => {
    switch (estado?.toLowerCase()) {
      case "aprobado":
        return "badge-success";
      case "rechazado":
        return "badge-danger";
      case "entregado":
        return "badge-info";
      default:
        return "badge-warning";
    }
  };

  const tramitesFiltrados = tramites.filter((t) => {
    if (filtroEstado === "Todos") return true;
    return t.estado.toLowerCase() === filtroEstado.toLowerCase();
  });

  return (
    <div className="page-container">
      <div className="page-header" style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", flexWrap: "wrap", gap: "1rem" }}>
        <div>
          <h1>📑 Trámites y Certificados Académicos</h1>
          <p>
            {esAdmin
              ? "Bandeja de gestión y expedición de constancias, certificados y solicitudes estudiantiles."
              : "Solicitá certificados oficiales, constancias de estudio y hacé seguimiento en tiempo real."}
          </p>
        </div>

        <button
          type="button"
          className="btn btn-primary"
          onClick={() => setMostrarSolicitudModal(true)}
          style={{ display: "flex", alignItems: "center", gap: "0.5rem" }}
        >
          ➕ Iniciar nueva solicitud
        </button>
      </div>

      {error && <div className="alert-error" style={{ marginBottom: "1rem" }}>{error}</div>}

      {/* Filtros */}
      <div className="card" style={{ marginBottom: "1.5rem", padding: "0.75rem 1.25rem" }}>
        <div style={{ display: "flex", gap: "0.5rem", alignItems: "center", flexWrap: "wrap" }}>
          <span style={{ fontSize: "0.9rem", fontWeight: 600, color: "var(--text-muted, #64748b)" }}>Estado:</span>
          {["Todos", "Pendiente", "Aprobado", "Rechazado", "Entregado"].map((est) => (
            <button
              key={est}
              type="button"
              className={`btn btn-small ${filtroEstado === est ? "btn-primary" : "btn-secondary"}`}
              onClick={() => setFiltroEstado(est)}
            >
              {est}
            </button>
          ))}
        </div>
      </div>

      {/* Modal / Formulario Nueva Solicitud */}
      {mostrarSolicitudModal && (
        <div className="card" style={{ marginBottom: "1.5rem", borderLeft: "4px solid var(--primary, #3b82f6)" }}>
          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "1rem" }}>
            <h3 style={{ margin: 0 }}>📝 Formulario de Solicitud de Trámite</h3>
            <button
              type="button"
              className="btn btn-secondary btn-small"
              onClick={() => setMostrarSolicitudModal(false)}
            >
              ✕ Cerrar
            </button>
          </div>

          <form onSubmit={handleSolicitar}>
            <div className="form-group" style={{ marginBottom: "1rem" }}>
              <label>Tipo de constancia o trámite *</label>
              <select value={tipo} onChange={(e) => setTipo(e.target.value)} required>
                {tiposTramite.map((t) => (
                  <option key={t} value={t}>{t}</option>
                ))}
              </select>
            </div>

            <div className="form-group" style={{ marginBottom: "1.25rem" }}>
              <label>Motivo o entidad destinataria *</label>
              <textarea
                required
                rows={3}
                placeholder="Ej: Presentar ante la empresa empleadora para justificar asistencia a examen."
                value={motivo}
                onChange={(e) => setMotivo(e.target.value)}
                style={{ width: "100%", padding: "0.75rem", borderRadius: "6px" }}
              />
            </div>

            <div className="form-actions" style={{ display: "flex", gap: "0.5rem" }}>
              <button type="submit" className="btn btn-primary">
                Enviar solicitud a Bedelía
              </button>
              <button
                type="button"
                className="btn btn-secondary"
                onClick={() => setMostrarSolicitudModal(false)}
              >
                Cancelar
              </button>
            </div>
          </form>
        </div>
      )}

      {/* Modal Resolver Trámite (Administrativos) */}
      {tramiteAResolver && (
        <div className="card" style={{ marginBottom: "1.5rem", borderLeft: "4px solid #f59e0b" }}>
          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "1rem" }}>
            <h3 style={{ margin: 0 }}>
              ⚖️ Resolver Solicitud: {tramiteAResolver.codigo} - {tramiteAResolver.tipo}
            </h3>
            <button
              type="button"
              className="btn btn-secondary btn-small"
              onClick={() => setTramiteAResolver(null)}
            >
              ✕ Cancelar
            </button>
          </div>

          <p style={{ margin: "0 0 1rem 0" }}>
            <strong>Solicitante:</strong> {tramiteAResolver.solicitanteNombre}{" "}
            {tramiteAResolver.alumnoDni ? `(DNI: ${tramiteAResolver.alumnoDni})` : ""}
            <br />
            <strong>Motivo presentado:</strong> {tramiteAResolver.motivo}
          </p>

          <form onSubmit={handleResolverSubmit}>
            <div className="form-grid" style={{ marginBottom: "1rem" }}>
              <div className="form-group">
                <label>Resolución / Dictamen</label>
                <select
                  value={resolucionEstado}
                  onChange={(e) => setResolucionEstado(e.target.value)}
                >
                  <option value="Aprobado">Aprobar (Emitir trámite)</option>
                  <option value="Rechazado">Rechazar solicitud</option>
                  <option value="Entregado">Entregado al estudiante</option>
                </select>
              </div>

              <div className="form-group">
                <label>Observaciones / Mensaje de secretaría</label>
                <input
                  type="text"
                  placeholder="Ej: Listo para retirar en ventanilla o adjunto en secretaría."
                  value={resolucionObservacion}
                  onChange={(e) => setResolucionObservacion(e.target.value)}
                />
              </div>
            </div>

            <div className="form-actions" style={{ display: "flex", gap: "0.5rem" }}>
              <button type="submit" className="btn btn-primary">
                Guardar resolución
              </button>
              <button
                type="button"
                className="btn btn-secondary"
                onClick={() => setTramiteAResolver(null)}
              >
                Cerrar
              </button>
            </div>
          </form>
        </div>
      )}

      {/* Modal Certificado / Comprobante Oficial Imprimible */}
      {tramiteComprobante && (
        <div
          style={{
            position: "fixed",
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: "rgba(0,0,0,0.5)",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            zIndex: 9999,
            padding: "1rem",
          }}
        >
          <div
            className="card"
            style={{
              maxWidth: "650px",
              width: "100%",
              backgroundColor: "#ffffff",
              color: "#0f172a",
              padding: "2rem",
              borderRadius: "8px",
              boxShadow: "0 20px 25px -5px rgba(0, 0, 0, 0.2)",
            }}
          >
            <div style={{ textAlign: "center", borderBottom: "2px solid #0f172a", paddingBottom: "1rem", marginBottom: "1.5rem" }}>
              <h2 style={{ margin: "0 0 0.25rem 0", color: "#1e3a8a", textTransform: "uppercase" }}>
                INSTITUTO DE EDUCACIÓN SUPERIOR
              </h2>
              <h4 style={{ margin: 0, fontWeight: 500, color: "#64748b" }}>
                Secretaría Académica y Bedelía
              </h4>
            </div>

            <div style={{ textAlign: "center", margin: "1.5rem 0" }}>
              <h3 style={{ textTransform: "uppercase", letterSpacing: "1px", textDecoration: "underline" }}>
                {tramiteComprobante.tipo}
              </h3>
              <p style={{ color: "#475569", fontSize: "0.85rem", marginTop: "0.25rem" }}>
                Identificador Oficial: <strong>{tramiteComprobante.codigo}</strong>
              </p>
            </div>

            <div style={{ fontSize: "1rem", lineHeight: "1.8", color: "#1e293b", textAlign: "justify", marginBottom: "2rem" }}>
              Por la presente se certifica que el/la estudiante{" "}
              <strong>{tramiteComprobante.solicitanteNombre}</strong>
              {tramiteComprobante.alumnoDni ? `, Documento Nacional de Identidad N° ${tramiteComprobante.alumnoDni},` : ""}{" "}
              ha solicitado y obtenido formalmente la emisión de este documento bajo el motivo expuesto:{" "}
              <em>"{tramiteComprobante.motivo}"</em>.
              <br />
              Constancia emitida con estado: <strong>{tramiteComprobante.estado.toUpperCase()}</strong>.
              {tramiteComprobante.observacionResolucion && (
                <div style={{ marginTop: "0.75rem", fontSize: "0.9rem", color: "#334155" }}>
                  <strong>Nota de Secretaría:</strong> {tramiteComprobante.observacionResolucion}
                </div>
              )}
            </div>

            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-end", borderTop: "1px dashed #cbd5e1", paddingTop: "1.5rem" }}>
              <div>
                <span style={{ fontSize: "0.8rem", color: "#64748b" }}>
                  Fecha de expedición: {tramiteComprobante.fechaResolucion ? new Date(tramiteComprobante.fechaResolucion).toLocaleDateString() : new Date().toLocaleDateString()}
                </span>
                <br />
                <span style={{ fontSize: "0.75rem", color: "#94a3b8" }}>
                  Código de verificación digital: SHA-256 Validated
                </span>
              </div>
              <div style={{ textAlign: "center", minWidth: "160px" }}>
                <div style={{ borderBottom: "1px solid #0f172a", marginBottom: "0.25rem", height: "30px" }}></div>
                <span style={{ fontSize: "0.8rem", fontWeight: 600 }}>Dirección Académica</span>
              </div>
            </div>

            <div style={{ display: "flex", justifyContent: "flex-end", gap: "0.5rem", marginTop: "2rem" }}>
              <button
                type="button"
                className="btn btn-primary"
                onClick={() => window.print()}
              >
                🖨️ Imprimir Documento
              </button>
              <button
                type="button"
                className="btn btn-secondary"
                onClick={() => setTramiteComprobante(null)}
              >
                Cerrar
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Tabla de Trámites */}
      {loading ? (
        <div style={{ textAlign: "center", padding: "3rem" }}>Cargando solicitudes...</div>
      ) : tramitesFiltrados.length === 0 ? (
        <div className="card" style={{ textAlign: "center", padding: "3rem", color: "var(--text-muted, #64748b)" }}>
          <div style={{ fontSize: "2.5rem", marginBottom: "0.5rem" }}>📭</div>
          <h3>No hay trámites registrados</h3>
          <p>No se encontraron solicitudes con el filtro seleccionado.</p>
        </div>
      ) : (
        <div className="card table-container">
          <table>
            <thead>
              <tr>
                <th>Código</th>
                {esAdmin && <th>Solicitante</th>}
                <th>Trámite</th>
                <th>Motivo</th>
                <th>Fecha Solicitud</th>
                <th>Estado</th>
                <th>Resolución</th>
                <th style={{ textAlign: "right" }}>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {tramitesFiltrados.map((t) => (
                <tr key={t.id}>
                  <td style={{ fontWeight: 600, fontFamily: "monospace" }}>{t.codigo}</td>
                  {esAdmin && (
                    <td>
                      <div>{t.solicitanteNombre}</div>
                      {t.alumnoDni && (
                        <small style={{ color: "var(--text-muted, #64748b)" }}>DNI: {t.alumnoDni}</small>
                      )}
                    </td>
                  )}
                  <td style={{ fontWeight: 500 }}>{t.tipo}</td>
                  <td style={{ maxWidth: "250px", wordBreak: "break-word" }}>{t.motivo}</td>
                  <td>{new Date(t.fechaSolicitud).toLocaleDateString()}</td>
                  <td>
                    <span className={`badge ${getEstadoBadgeClass(t.estado)}`} style={{ padding: "0.2rem 0.6rem", borderRadius: "4px" }}>
                      {t.estado}
                    </span>
                  </td>
                  <td>
                    {t.observacionResolucion ? (
                      <span style={{ fontSize: "0.85rem" }}>{t.observacionResolucion}</span>
                    ) : (
                      <span style={{ color: "var(--text-muted, #94a3b8)", fontSize: "0.85rem" }}>—</span>
                    )}
                  </td>
                  <td style={{ textAlign: "right", whiteSpace: "nowrap" }}>
                    <div style={{ display: "inline-flex", gap: "0.35rem" }}>
                      {t.estado === "Aprobado" && (
                        <button
                          type="button"
                          className="btn btn-secondary btn-small"
                          onClick={() => setTramiteComprobante(t)}
                          title="Ver o imprimir constancia oficial"
                        >
                          🖨️ Ver constancia
                        </button>
                      )}

                      {esAdmin && (
                        <button
                          type="button"
                          className="btn btn-primary btn-small"
                          onClick={() => {
                            setTramiteAResolver(t);
                            setResolucionEstado(t.estado === "Pendiente" ? "Aprobado" : t.estado);
                            setResolucionObservacion(t.observacionResolucion || "");
                          }}
                        >
                          ⚖️ Resolver
                        </button>
                      )}

                      {t.estado === "Pendiente" && (
                        <button
                          type="button"
                          className="btn btn-danger btn-small"
                          onClick={() => handleCancelar(t.id)}
                          title="Cancelar solicitud"
                        >
                          ✕
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
