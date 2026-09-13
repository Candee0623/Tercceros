namespace backend.Entities;

public class Evaluacion
{
    public Guid Id { get; set; }
    public Guid CursadaId { get; set; }
    public Cursada Cursada { get; set; } = null!;
    public int Numero { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime? Fecha { get; set; }
    public List<Calificacion> Calificaciones { get; set; } = new();
}
