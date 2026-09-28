namespace backend.DTOs.Matriculas;
public class MatriculaDto { public Guid Id { get; set; } public Guid AlumnoId { get; set; } public string AlumnoNombre { get; set; } = ""; public string AlumnoDni { get; set; } = ""; public Guid CursadaId { get; set; } public DateTime FechaMatriculacion { get; set; } public bool Activa { get; set; } }
public class MatricularAlumnoDto { public Guid AlumnoId { get; set; } }
