namespace backend.DTOs.Carreras;

public class CreateCarreraDto
{
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public int DuracionAnios { get; set; }
}