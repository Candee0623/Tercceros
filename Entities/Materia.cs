namespace backend.Entities;

public class Materia
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Codigo { get; set; } = string.Empty;

    public int Anio { get; set; }

    public bool Activa { get; set; } = true;

    public int CantidadEvaluaciones { get; set; } = 2;
    public bool EsPromocionable { get; set; } = true;
    public decimal? NotaPromocion { get; set; } = 7m;
    public decimal NotaRegularizacion { get; set; } = 4m;

    public Guid CarreraId { get; set; }

    public Carrera Carrera { get; set; } = null!;

    public Guid? ProfesorId { get; set; }

    public Profesor? Profesor { get; set; }
}