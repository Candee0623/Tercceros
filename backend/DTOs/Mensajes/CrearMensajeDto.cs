using backend.Entities;
using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Mensajes;

public class CrearMensajeDto
{
    [Required]
    [MaxLength(150)]
    public string Asunto { get; set; } = string.Empty;

    [Required]
    public string Contenido { get; set; } = string.Empty;

    [Required]
    public TipoDestinatarioMensaje TipoDestinatario { get; set; }

    public Guid? CarreraId { get; set; }

    public int? Anio { get; set; }

    public Guid? CursadaId { get; set; }
}