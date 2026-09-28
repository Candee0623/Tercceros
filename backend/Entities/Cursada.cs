namespace backend.Entities;

public class Cursada
{
    public Guid Id { get; set; }
    public Guid MateriaId { get; set; }
    public Materia Materia { get; set; } = null!;
    public Guid? ProfesorId { get; set; }
    public Profesor? Profesor { get; set; }
    public int CicloLectivo { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Activa { get; set; } = true;
    public decimal PorcentajePromocion { get; set; } = 80m;
    public decimal PorcentajeRegularidad { get; set; } = 75m;
    public List<CursadaHorario> Horarios { get; set; } = new();
    public List<Matricula> Matriculas { get; set; } = new();
    public List<Clase> Clases { get; set; } = new();
    public List<Evaluacion> Evaluaciones { get; set; } = new();
}
