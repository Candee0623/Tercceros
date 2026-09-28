namespace backend.Entities;

public class Matricula
{
    public Guid Id { get; set; }
    public Guid AlumnoId { get; set; }
    public Alumno Alumno { get; set; } = null!;
    public Guid CursadaId { get; set; }
    public Cursada Cursada { get; set; } = null!;
    public DateTime FechaMatriculacion { get; set; } = DateTime.UtcNow;
    public bool Activa { get; set; } = true;
    public List<AsistenciaDetalle> Asistencias { get; set; } = new();
    public List<Calificacion> Calificaciones { get; set; } = new();
}
