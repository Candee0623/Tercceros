namespace backend.Entities;

public class Anuncio
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public string Categoria { get; set; } = "General"; // General, Académico, Exámenes, Urgente
    public string Prioridad { get; set; } = "Normal"; // Baja, Normal, Alta, Urgente
    public DateTime FechaPublicacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaVencimiento { get; set; }
    public bool Activo { get; set; } = true;

    // Autor
    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
}
