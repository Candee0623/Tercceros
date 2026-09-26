namespace backend.DTOs.PortalProfesor;

public class MiCursadaProfesorDto
{
    public Guid CursadaId { get; set; }
    public string Materia { get; set; } = string.Empty;
    public int CicloLectivo { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal PorcentajePromocion { get; set; }
    public decimal PorcentajeRegularidad { get; set; }
    public int CantidadAlumnos { get; set; }
    public List<string> Horarios { get; set; } = new();
}