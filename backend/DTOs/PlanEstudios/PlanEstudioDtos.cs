namespace backend.DTOs.PlanEstudios;

public class PlanEstudioMateriaDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public bool Activa { get; set; }
    public string? ProfesorNombre { get; set; }
}

public class PlanEstudioAnioDto
{
    public int Anio { get; set; }
    public List<PlanEstudioMateriaDto> Materias { get; set; } = new();
}

public class PlanEstudioCarreraDto
{
    public Guid CarreraId { get; set; }
    public string CarreraNombre { get; set; } = string.Empty;
    public int DuracionAnios { get; set; }
    public bool CarreraActiva { get; set; }
    public List<PlanEstudioAnioDto> Anios { get; set; } = new();
}
