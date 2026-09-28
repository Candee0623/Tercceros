using System.ComponentModel.DataAnnotations;

namespace backend.Entities;

public enum TipoDestinatarioMensaje
{
    Todos = 1,
    Carrera = 2,
    CarreraAnio = 3,
    Cursada = 4
}

public class Mensaje
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(150)]
    public string Asunto { get; set; } = string.Empty;

    [Required]
    public string Contenido { get; set; } = string.Empty;

    public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;

    // Usuario que envió el mensaje.
    [Required]
    public Guid UsuarioRemitenteId { get; set; }

    public Usuario UsuarioRemitente { get; set; } = null!;

    // Define a quién se dirige originalmente el mensaje.
    public TipoDestinatarioMensaje TipoDestinatario { get; set; }

    // Se utiliza cuando TipoDestinatario = Carrera
    // o CarreraAnio.
    public Guid? CarreraId { get; set; }

    public Carrera? Carrera { get; set; }

    // Se utiliza cuando TipoDestinatario = CarreraAnio.
    public int? Anio { get; set; }

    // Se utiliza cuando TipoDestinatario = Cursada.
    public Guid? CursadaId { get; set; }

    public Cursada? Cursada { get; set; }

    public List<MensajeDestinatario> Destinatarios { get; set; } = new();
}