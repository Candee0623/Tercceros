namespace backend.DTOs.Carreras;

public class UpdateCarreraDto
{
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public int DuracionAnios { get; set; }

    public bool Activa { get; set; }
}