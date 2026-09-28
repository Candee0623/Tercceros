namespace backend.DTOs.Tramites;

public class TramiteDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaResolucion { get; set; }
    public string? ObservacionResolucion { get; set; }
    public Guid UsuarioId { get; set; }
    public string SolicitanteNombre { get; set; } = string.Empty;
    public string? AlumnoDni { get; set; }
}

public class CrearTramiteDto
{
    public string Tipo { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
}

public class ResolverTramiteDto
{
    public string Estado { get; set; } = "Aprobado"; // Aprobado, Rechazado, Entregado
    public string? Observacion { get; set; }
}
