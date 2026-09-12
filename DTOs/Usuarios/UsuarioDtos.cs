namespace backend.DTOs.Usuarios;

public class UsuarioDto
{
    public Guid Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public Guid RolId { get; set; }
    public string RolNombre { get; set; } = string.Empty;
    public Guid? AlumnoId { get; set; }
    public string? AlumnoNombre { get; set; }
}

public class CreateUsuarioDto
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid RolId { get; set; }
    public Guid? AlumnoId { get; set; }
    public bool Activo { get; set; } = true;
}

public class UpdateUsuarioDto
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? Password { get; set; }
    public Guid RolId { get; set; }
    public Guid? AlumnoId { get; set; }
    public bool Activo { get; set; } = true;
}

public class UsuarioRolOptionDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class UsuarioAlumnoOptionDto
{
    public Guid Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
}
