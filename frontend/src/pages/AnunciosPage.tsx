import { useEffect, useState } from "react";
import { getSession } from "../auth/authService";
import {
  actualizarAnuncio,
  crearAnuncio,
  eliminarAnuncio,
  getAnuncios,
} from "../services/anuncioService";
import type { Anuncio, CrearAnuncioRequest } from "../types/Anuncio";

export default function AnunciosPage() {
  const session = getSession();
  const esAdmin =
    session?.rol?.toUpperCase() === "ADMIN" ||
    session?.rol?.toUpperCase() === "ADMINISTRADOR";
  const esAlumno =
    session?.rol?.toUpperCase() === "ALUMNO" || !!session?.alumnoId;
  const puedePublicar = !esAlumno || esAdmin;

  const [anuncios, setAnuncios] = useState<Anuncio[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [filtroCategoria, setFiltroCategoria] = useState("Todas");
  const [busqueda, setBusqueda] = useState("");
  const [mostrarModal, setMostrarModal] = useState(false);

  // Formulario nuevo anuncio
  const [form, setForm] = useState<CrearAnuncioRequest>({
    titulo: "",
    contenido: "",
    categoria: "General",
    prioridad: "Normal",
    fechaVencimiento: "",
  });

  async function cargarAnuncios() {
    try {
      setLoading(true);
      setError("");
      const data = await getAnuncios(esAdmin);
      setAnuncios(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al cargar los avisos");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    cargarAnuncios();
  }, []);

  async function handlePublicar(e: React.FormEvent) {
    e.preventDefault();
    if (!form.titulo.trim() || !form.contenido.trim()) return;

    try {
      setError("");
      await crearAnuncio({
        ...form,
        fechaVencimiento: form.fechaVencimiento ? form.fechaVencimiento : null,
      });
      setForm({
        titulo: "",
        contenido: "",
        categoria: "General",
        prioridad: "Normal",
        fechaVencimiento: "",
      });
      setMostrarModal(false);
      await cargarAnuncios();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al publicar aviso");
    }
  }

  async function handleEliminar(id: string) {
    if (!confirm("¿Deseas eliminar este aviso definitivamente?")) return;
    try {
      await eliminarAnuncio(id);
      await cargarAnuncios();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Error al eliminar aviso");
    }
  }

  async function handleToggleActivo(anuncio: Anuncio) {
    try {
      await actualizarAnuncio(anuncio.id, {
        titulo: anuncio.titulo,
        contenido: anuncio.contenido,
        categoria: anuncio.categoria,
        prioridad: anuncio.prioridad,
        fechaVencimiento: anuncio.fechaVencimiento,
        activo: !anuncio.activo,
      });
      await cargarAnuncios();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Error al actualizar aviso");
    }
  }

  const categorias = ["Todas", "General", "Académico", "Exámenes", "Urgente", "Administrativo"];

  const anunciosFiltrados = anuncios.filter((a) => {
    const coincideCat = filtroCategoria === "Todas" || a.categoria === filtroCategoria;
    const coincideTexto =
      a.titulo.toLowerCase().includes(busqueda.toLowerCase()) ||
      a.contenido.toLowerCase().includes(busqueda.toLowerCase()) ||
      a.autorNombre.toLowerCase().includes(busqueda.toLowerCase());
    return coincideCat && coincideTexto;
  });

  const getPrioridadBadgeClass = (prioridad: string) => {
    switch (prioridad?.toLowerCase()) {
      case "urgente":
        return "badge-danger";
      case "alta":
        return "badge-warning";
      case "baja":
        return "badge-secondary";
      default:
        return "badge-info";
    }
  };

  return (
    <div className="page-container">
      <div className="page-header" style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", flexWrap: "wrap", gap: "1rem" }}>
        <div>
          <h1>📢 Avisos y Cartelera Institucional</h1>
          <p>
            Comunicados oficiales, novedades académicas y avisos importantes para toda la comunidad educativa.
          </p>
        </div>
        {puedePublicar && (
          <button
            type="button"
            className="btn btn-primary"
            onClick={() => setMostrarModal(true)}
            style={{ display: "flex", alignItems: "center", gap: "0.5rem" }}
          >
            ➕ Publicar nuevo aviso
          </button>
        )}
      </div>

      {error && <div className="alert-error" style={{ marginBottom: "1rem" }}>{error}</div>}

      {/* Barra de Filtros y Búsqueda */}
      <div className="card" style={{ marginBottom: "1.5rem", padding: "1rem 1.25rem" }}>
        <div style={{ display: "flex", flexWrap: "wrap", gap: "1rem", alignItems: "center", justifyContent: "space-between" }}>
          {/* Categorías */}
          <div style={{ display: "flex", gap: "0.5rem", flexWrap: "wrap", alignItems: "center" }}>
            <span style={{ fontSize: "0.9rem", fontWeight: 600, color: "var(--text-muted, #64748b)" }}>Categoría:</span>
            {categorias.map((cat) => (
              <button
                key={cat}
                type="button"
                className={`btn btn-small ${filtroCategoria === cat ? "btn-primary" : "btn-secondary"}`}
                onClick={() => setFiltroCategoria(cat)}
              >
                {cat}
              </button>
            ))}
          </div>

          {/* Buscador */}
          <div style={{ minWidth: "240px", flex: "1 1 200px", maxWidth: "360px" }}>
            <input
              type="text"
              placeholder="🔍 Buscar por título o contenido..."
              value={busqueda}
              onChange={(e) => setBusqueda(e.target.value)}
              style={{ width: "100%", padding: "0.5rem 0.75rem", borderRadius: "6px" }}
            />
          </div>
        </div>
      </div>

      {/* Modal / Panel para Crear Aviso */}
      {mostrarModal && (
        <div className="card" style={{ marginBottom: "1.5rem", borderLeft: "4px solid var(--primary, #3b82f6)" }}>
          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "1rem" }}>
            <h3 style={{ margin: 0 }}>✍️ Redactar Comunicado Oficial</h3>
            <button
              type="button"
              className="btn btn-secondary btn-small"
              onClick={() => setMostrarModal(false)}
            >
              ✕ Cerrar
            </button>
          </div>

          <form onSubmit={handlePublicar}>
            <div className="form-grid" style={{ marginBottom: "1rem" }}>
              <div className="form-group" style={{ gridColumn: "span 2" }}>
                <label>Título del aviso *</label>
                <input
                  type="text"
                  required
                  placeholder="Ej: Fechas de mesas de examen finales - Diciembre"
                  value={form.titulo}
                  onChange={(e) => setForm({ ...form, titulo: e.target.value })}
                />
              </div>

              <div className="form-group">
                <label>Categoría</label>
                <select
                  value={form.categoria}
                  onChange={(e) => setForm({ ...form, categoria: e.target.value })}
                >
                  <option value="General">General</option>
                  <option value="Académico">Académico</option>
                  <option value="Exámenes">Exámenes</option>
                  <option value="Urgente">Urgente</option>
                  <option value="Administrativo">Administrativo</option>
                </select>
              </div>

              <div className="form-group">
                <label>Prioridad</label>
                <select
                  value={form.prioridad}
                  onChange={(e) => setForm({ ...form, prioridad: e.target.value })}
                >
                  <option value="Baja">Baja</option>
                  <option value="Normal">Normal</option>
                  <option value="Alta">Alta (Destacado)</option>
                  <option value="Urgente">Urgente (Alerta)</option>
                </select>
              </div>

              <div className="form-group">
                <label>Fecha de expiración (opcional)</label>
                <input
                  type="date"
                  value={form.fechaVencimiento || ""}
                  onChange={(e) => setForm({ ...form, fechaVencimiento: e.target.value })}
                />
              </div>
            </div>

            <div className="form-group" style={{ marginBottom: "1rem" }}>
              <label>Cuerpo del comunicado *</label>
              <textarea
                required
                rows={4}
                placeholder="Escribe el mensaje detallado para los estudiantes y docentes..."
                value={form.contenido}
                onChange={(e) => setForm({ ...form, contenido: e.target.value })}
                style={{ width: "100%", padding: "0.75rem", borderRadius: "6px" }}
              />
            </div>

            <div className="form-actions" style={{ display: "flex", gap: "0.5rem" }}>
              <button type="submit" className="btn btn-primary">
                Publicar aviso
              </button>
              <button
                type="button"
                className="btn btn-secondary"
                onClick={() => setMostrarModal(false)}
              >
                Cancelar
              </button>
            </div>
          </form>
        </div>
      )}

      {/* Grid de Anuncios */}
      {loading ? (
        <div style={{ textAlign: "center", padding: "3rem" }}>Cargando cartelera...</div>
      ) : anunciosFiltrados.length === 0 ? (
        <div className="card" style={{ textAlign: "center", padding: "3rem", color: "var(--text-muted, #64748b)" }}>
          <div style={{ fontSize: "2.5rem", marginBottom: "0.5rem" }}>📭</div>
          <h3>No hay avisos disponibles</h3>
          <p>Actualmente no hay comunicados activos en esta categoría.</p>
        </div>
      ) : (
        <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(320px, 1fr))", gap: "1.25rem" }}>
          {anunciosFiltrados.map((anuncio) => {
            const esUrgente = anuncio.prioridad?.toLowerCase() === "urgente";
            const esAlta = anuncio.prioridad?.toLowerCase() === "alta";

            return (
              <div
                key={anuncio.id}
                className="card"
                style={{
                  display: "flex",
                  flexDirection: "column",
                  justifyContent: "space-between",
                  borderTop: esUrgente
                    ? "4px solid #ef4444"
                    : esAlta
                    ? "4px solid #f59e0b"
                    : "4px solid #3b82f6",
                  position: "relative",
                  boxShadow: "0 4px 6px -1px rgba(0, 0, 0, 0.07), 0 2px 4px -1px rgba(0, 0, 0, 0.04)",
                  transition: "transform 0.15s ease",
                  opacity: anuncio.activo ? 1 : 0.65,
                }}
              >
                <div>
                  <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", gap: "0.5rem", marginBottom: "0.75rem" }}>
                    <div style={{ display: "flex", gap: "0.35rem", flexWrap: "wrap" }}>
                      <span className={`badge ${getPrioridadBadgeClass(anuncio.prioridad)}`} style={{ padding: "0.2rem 0.5rem", fontSize: "0.75rem", borderRadius: "4px" }}>
                        {anuncio.prioridad}
                      </span>
                      <span className="badge badge-secondary" style={{ padding: "0.2rem 0.5rem", fontSize: "0.75rem", borderRadius: "4px", background: "var(--surface-variant, #e2e8f0)" }}>
                        {anuncio.categoria}
                      </span>
                    </div>

                    {!anuncio.activo && (
                      <span style={{ fontSize: "0.75rem", color: "#ef4444", fontWeight: 600 }}>
                        (Archivado)
                      </span>
                    )}
                  </div>

                  <h3 style={{ margin: "0 0 0.5rem 0", fontSize: "1.15rem", lineHeight: 1.3 }}>
                    {anuncio.titulo}
                  </h3>

                  <p style={{ whiteSpace: "pre-line", margin: "0 0 1rem 0", color: "var(--text-body, #334155)", fontSize: "0.925rem" }}>
                    {anuncio.contenido}
                  </p>
                </div>

                <div style={{ borderTop: "1px solid var(--border-color, #e2e8f0)", paddingTop: "0.75rem", marginTop: "auto", fontSize: "0.8rem", color: "var(--text-muted, #64748b)" }}>
                  <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                    <div>
                      <div>👤 {anuncio.autorNombre}</div>
                      <div>📅 {new Date(anuncio.fechaPublicacion).toLocaleDateString()}</div>
                    </div>

                    {puedePublicar && (
                      <div style={{ display: "flex", gap: "0.3rem" }}>
                        {esAdmin && (
                          <button
                            type="button"
                            className="btn btn-secondary btn-small"
                            onClick={() => handleToggleActivo(anuncio)}
                            title={anuncio.activo ? "Archivar aviso" : "Reactivar aviso"}
                            style={{ padding: "0.2rem 0.4rem", fontSize: "0.75rem" }}
                          >
                            {anuncio.activo ? "Archivar" : "Activar"}
                          </button>
                        )}
                        <button
                          type="button"
                          className="btn btn-danger btn-small"
                          onClick={() => handleEliminar(anuncio.id)}
                          title="Eliminar aviso"
                          style={{ padding: "0.2rem 0.4rem", fontSize: "0.75rem" }}
                        >
                          🗑️
                        </button>
                      </div>
                    )}
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
