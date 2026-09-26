namespace backend.DTOs.Auth;
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public Guid? AlumnoId { get; set; }
    public Guid? ProfesorId { get; set; }
    public List<string> Permisos { get; set; } = new();
}
