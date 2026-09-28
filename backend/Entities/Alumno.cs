namespace backend.Entities;

public class Alumno
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string Dni { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Telefono { get; set; }

    public DateTime FechaNacimiento { get; set; }

    public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;

    public bool Activo { get; set; } = true;

    public Guid? CarreraId { get; set; }

    public Carrera? Carrera { get; set; }
}