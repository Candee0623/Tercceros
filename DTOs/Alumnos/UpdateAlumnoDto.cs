namespace backend.DTOs.Alumnos;

public class UpdateAlumnoDto
{
    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string Dni { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Telefono { get; set; }

    public DateTime FechaNacimiento { get; set; }

    public Guid? CarreraId { get; set; }

    public bool Activo { get; set; }
}