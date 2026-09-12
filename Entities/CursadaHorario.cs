namespace backend.Entities;

public class CursadaHorario
{
    public Guid Id { get; set; }
    public Guid CursadaId { get; set; }
    public Cursada Cursada { get; set; } = null!;
    public int DiaSemana { get; set; } // 1=Lunes ... 7=Domingo
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
}
