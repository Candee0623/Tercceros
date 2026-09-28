namespace backend.DTOs.Anuncios;

public class AnuncioDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public bool Activo { get; set; }
    public Guid UsuarioId { get; set; }
    public string AutorNombre { get; set; } = string.Empty;
}

public class CrearAnuncioDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public string Categoria { get; set; } = "General";
    public string Prioridad { get; set; } = "Normal";
    public DateTime? FechaVencimiento { get; set; }
}

public class ActualizarAnuncioDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public string Categoria { get; set; } = "General";
    public string Prioridad { get; set; } = "Normal";
    public DateTime? FechaVencimiento { get; set; }
    public bool Activo { get; set; } = true;
}
