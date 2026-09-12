namespace backend.DTOs.EstadoAsistencia;

public class AlumnoConsultaDto { public Guid Id { get; set; } public string NombreCompleto { get; set; } = ""; public string Dni { get; set; } = ""; }
public class EstadoAsistenciaCursadaDto
{
    public Guid MatriculaId { get; set; }
    public Guid CursadaId { get; set; }
    public string Materia { get; set; } = "";
    public string Periodo { get; set; } = "";
    public int CicloLectivo { get; set; }
    public string? Profesor { get; set; }
    public decimal HorasTotalesPlanificadas { get; set; }
    public decimal HorasAusentes { get; set; }
    public decimal HorasAsistidas { get; set; }
    public decimal PorcentajeAsistencia { get; set; }
    public decimal PorcentajePromocion { get; set; }
    public decimal PorcentajeRegularidad { get; set; }
    public decimal PorcentajeLibre { get; set; }
    public string Condicion { get; set; } = "";
}
public class EstadoAsistenciaAlumnoDto
{
    public Guid AlumnoId { get; set; }
    public string AlumnoNombre { get; set; } = "";
    public string Dni { get; set; } = "";
    public List<EstadoAsistenciaCursadaDto> Cursadas { get; set; } = new();
}
