namespace backend.DTOs.Cuotas;

public class EstadoCuotaDto
{
    public Guid AlumnoId { get; set; }
    public string AlumnoNombre { get; set; } = "";
    public string AlumnoDni { get; set; } = "";
    public bool AlDia { get; set; }
    public bool Registrado { get; set; }
    public string? UltimoPeriodoPagado { get; set; }
    public DateTime? FechaUltimoPago { get; set; }
    public decimal? ImporteUltimoPago { get; set; }
    public string? Observacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}

public class GuardarEstadoCuotaDto
{
    public bool AlDia { get; set; }
    public string? UltimoPeriodoPagado { get; set; }
    public DateTime? FechaUltimoPago { get; set; }
    public decimal? ImporteUltimoPago { get; set; }
    public string? Observacion { get; set; }
}
