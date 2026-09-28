namespace backend.DTOs.Calificaciones;

public class EvaluacionDto
{
    public Guid Id { get; set; }
    public int Numero { get; set; }
    public string Nombre { get; set; } = "";
    public DateTime? Fecha { get; set; }
}

public class NotaEvaluacionDto
{
    public Guid EvaluacionId { get; set; }
    public decimal? Nota { get; set; }
    public string? Observacion { get; set; }
}

public class AlumnoCalificacionDto
{
    public Guid MatriculaId { get; set; }
    public Guid AlumnoId { get; set; }
    public string AlumnoNombre { get; set; } = "";
    public string AlumnoDni { get; set; } = "";
    public bool CuotaAlDia { get; set; }
    public decimal? Promedio { get; set; }
    public string Condicion { get; set; } = "EN_CURSO";
    public List<NotaEvaluacionDto> Notas { get; set; } = new();
}

public class PlanillaCalificacionesDto
{
    public Guid CursadaId { get; set; }
    public string CursadaNombre { get; set; } = "";
    public bool EsPromocionable { get; set; }
    public decimal? NotaPromocion { get; set; }
    public decimal NotaRegularizacion { get; set; }
    public int CantidadEvaluaciones { get; set; }
    public List<EvaluacionDto> Evaluaciones { get; set; } = new();
    public List<AlumnoCalificacionDto> Alumnos { get; set; } = new();
}

public class GuardarNotaDto
{
    public Guid MatriculaId { get; set; }
    public Guid EvaluacionId { get; set; }
    public decimal? Nota { get; set; }
    public string? Observacion { get; set; }
}

public class GuardarCalificacionesDto
{
    public Guid CursadaId { get; set; }
    public List<GuardarNotaDto> Notas { get; set; } = new();
}
