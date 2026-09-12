namespace backend.Entities;

public class Calificacion
{
    public Guid Id { get; set; }
    public Guid EvaluacionId { get; set; }
    public Evaluacion Evaluacion { get; set; } = null!;
    public Guid MatriculaId { get; set; }
    public Matricula Matricula { get; set; } = null!;
    public decimal Nota { get; set; }
    public string? Observacion { get; set; }
    public DateTime FechaCarga { get; set; } = DateTime.UtcNow;
}
