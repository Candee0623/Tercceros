namespace backend.Entities;

public class Usuario
{
    public Guid Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public Guid RolId { get; set; }
    public Rol Rol { get; set; } = null!;

    // Relación opcional con la ficha académica. Para el rol ALUMNO es obligatoria.
    public Guid? AlumnoId { get; set; }
    public Alumno? Alumno { get; set; }
}
