namespace backend.DTOs.Mensajes;

public class MensajeEnviadoDto
{
    public Guid Id { get; set; }
    public string Asunto { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public DateTime FechaEnvio { get; set; }
    public string TipoDestinatario { get; set; } = string.Empty;
    public string DestinatarioDescripcion { get; set; } = string.Empty;
    public int CantidadDestinatarios { get; set; }
}