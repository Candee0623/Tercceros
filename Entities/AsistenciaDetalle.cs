namespace backend.Entities;

public class AsistenciaDetalle
{
    public Guid Id { get; set; }
    public Guid ClaseId { get; set; }
    public Clase Clase { get; set; } = null!;
    public Guid MatriculaId { get; set; }
    public Matricula Matricula { get; set; } = null!;
    public string Estado { get; set; } = "PRESENTE"; // PRESENTE | AUSENCIA_TOTAL | AUSENCIA_PARCIAL
    public decimal HorasAusente { get; set; }
    public string? Observacion { get; set; }
}
