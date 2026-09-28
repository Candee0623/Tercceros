namespace backend.Entities;

public class SolicitudTramite
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Codigo { get; set; } = string.Empty; // Ej: TR-2026-0001
    public string Tipo { get; set; } = string.Empty; // Constancia de Alumno Regular, Certificado de Examen, Analítico Parcial, Equivalencias, Justificación de Inasistencia, etc.
    public string Motivo { get; set; } = string.Empty;
    public string Estado { get; set; } = "Pendiente"; // Pendiente, Aprobado, Rechazado, Entregado
    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
    public DateTime? FechaResolucion { get; set; }
    public string? ObservacionResolucion { get; set; }

    // Usuario que solicitó
    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    // Alumno asociado si corresponde
    public Guid? AlumnoId { get; set; }
    public Alumno? Alumno { get; set; }
}
