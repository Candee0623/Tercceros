namespace backend.Entities;

public class Profesor
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string Dni { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Telefono { get; set; }

    public string? Titulo { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}