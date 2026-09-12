namespace backend.Entities;

public class EstadoCuotaAlumno
{
    public Guid Id { get; set; }
    public Guid AlumnoId { get; set; }
    public Alumno Alumno { get; set; } = null!;
    public bool AlDia { get; set; }
    public string? UltimoPeriodoPagado { get; set; }
    public DateTime? FechaUltimoPago { get; set; }
    public decimal? ImporteUltimoPago { get; set; }
    public string? Observacion { get; set; }
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
}
