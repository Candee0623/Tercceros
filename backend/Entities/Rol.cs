namespace backend.Entities;

public class Rol
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    public bool EsSistema { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public ICollection<RolPermiso> Permisos { get; set; } = new List<RolPermiso>();
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
