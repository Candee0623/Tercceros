namespace backend.DTOs.Roles;
public class RolDto { public Guid Id { get; set; } public string Nombre { get; set; } = string.Empty; public string? Descripcion { get; set; } public bool Activo { get; set; } public bool EsSistema { get; set; } public List<string> Permisos { get; set; } = new(); }
public class SaveRolDto { public string Nombre { get; set; } = string.Empty; public string? Descripcion { get; set; } public bool Activo { get; set; } = true; public List<string> Permisos { get; set; } = new(); }
public class PantallaDto { public string Clave { get; set; } = string.Empty; public string Nombre { get; set; } = string.Empty; }
