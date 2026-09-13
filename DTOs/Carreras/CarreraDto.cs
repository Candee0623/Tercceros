namespace backend.DTOs.Carreras;

public class CarreraDto
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public int DuracionAnios { get; set; }

    public bool Activa { get; set; }

    public DateTime FechaCreacion { get; set; }
}