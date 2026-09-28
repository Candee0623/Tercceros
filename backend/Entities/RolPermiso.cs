namespace backend.Entities;

public class RolPermiso
{
    public Guid RolId { get; set; }
    public Rol Rol { get; set; } = null!;
    public string Pantalla { get; set; } = string.Empty;
}
