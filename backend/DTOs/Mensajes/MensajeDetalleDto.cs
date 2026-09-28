namespace backend.DTOs.Mensajes;

public class MensajeDetalleDto
{
    public Guid Id { get; set; }
    public string Asunto { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public DateTime FechaEnvio { get; set; }
    public string Remitente { get; set; } = string.Empty;
    public bool Leido { get; set; }
}