namespace backend.DTOs.Materias;

public class UpdateMateriaDto
{
    public string Nombre { get; set; } = string.Empty;

    public string Codigo { get; set; } = string.Empty;

    public int Anio { get; set; }

    public bool Activa { get; set; }

    public Guid CarreraId { get; set; }

    public Guid? ProfesorId { get; set; }

    public int CantidadEvaluaciones { get; set; } = 2;
    public bool EsPromocionable { get; set; } = true;
    public decimal? NotaPromocion { get; set; } = 7m;
    public decimal NotaRegularizacion { get; set; } = 4m;
}