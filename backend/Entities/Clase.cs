namespace backend.Entities;

public class Clase
{
    public Guid Id { get; set; }
    public Guid CursadaId { get; set; }
    public Cursada Cursada { get; set; } = null!;
    public DateTime Fecha { get; set; }
    public decimal HorasProgramadas { get; set; }
    public DateTime FechaCarga { get; set; } = DateTime.UtcNow;
    public List<AsistenciaDetalle> Detalles { get; set; } = new();
}
