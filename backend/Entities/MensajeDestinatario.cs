namespace backend.Entities;

public class MensajeDestinatario
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid MensajeId { get; set; }

    public Mensaje Mensaje { get; set; } = null!;

    public Guid AlumnoId { get; set; }

    public Alumno Alumno { get; set; } = null!;

    public bool Leido { get; set; } = false;

    public DateTime? FechaLectura { get; set; }
}