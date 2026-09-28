namespace backend.DTOs.Mensajes;

public class MensajeCursadaDto
{
    public Guid Id { get; set; }
    public string MateriaNombre { get; set; } = string.Empty;
    public int CicloLectivo { get; set; }
    public string Periodo { get; set; } = string.Empty;
}