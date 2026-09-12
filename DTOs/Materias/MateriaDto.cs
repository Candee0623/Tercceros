namespace backend.DTOs.Materias;

public class MateriaDto
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Codigo { get; set; } = string.Empty;

    public int Anio { get; set; }

    public bool Activa { get; set; }

    public Guid CarreraId { get; set; }

    public string CarreraNombre { get; set; } = string.Empty;

    public Guid? ProfesorId { get; set; }

    public string? ProfesorNombre { get; set; }

    public int CantidadEvaluaciones { get; set; }
    public bool EsPromocionable { get; set; }
    public decimal? NotaPromocion { get; set; }
    public decimal NotaRegularizacion { get; set; }
}