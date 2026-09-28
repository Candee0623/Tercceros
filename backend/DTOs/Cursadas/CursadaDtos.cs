namespace backend.DTOs.Cursadas;

public class HorarioDto { public Guid? Id { get; set; } public int DiaSemana { get; set; } public string HoraInicio { get; set; } = ""; public string HoraFin { get; set; } = ""; }
public class CursadaDto
{
    public Guid Id { get; set; }
    public Guid MateriaId { get; set; }
    public string MateriaNombre { get; set; } = "";
    public Guid? ProfesorId { get; set; }
    public string? ProfesorNombre { get; set; }
    public int CicloLectivo { get; set; }
    public string Periodo { get; set; } = "";
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Activa { get; set; }
    public decimal PorcentajePromocion { get; set; }
    public decimal PorcentajeRegularidad { get; set; }
    public decimal PorcentajeLibre => PorcentajeRegularidad;
    public decimal HorasSemanales { get; set; }
    public decimal HorasTotalesPlanificadas { get; set; }
    public List<HorarioDto> Horarios { get; set; } = new();
}
public class SaveCursadaDto
{
    public Guid MateriaId { get; set; }
    public Guid? ProfesorId { get; set; }
    public int CicloLectivo { get; set; }
    public string Periodo { get; set; } = "";
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Activa { get; set; } = true;
    public decimal PorcentajePromocion { get; set; } = 80m;
    public decimal PorcentajeRegularidad { get; set; } = 75m;
    public List<HorarioDto> Horarios { get; set; } = new();
}
