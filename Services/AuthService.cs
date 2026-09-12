using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Data;
using backend.DTOs.Auth;
using backend.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    public AuthService(AppDbContext context, IConfiguration configuration) { _context = context; _configuration = configuration; }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var username = request.NombreUsuario.Trim().ToLowerInvariant();
        var usuario = await _context.Usuarios.Include(u => u.Rol).ThenInclude(r => r.Permisos)
            .FirstOrDefaultAsync(u => u.NombreUsuario.ToLower() == username && u.Activo && u.Rol.Activo);
        if (usuario == null || !PasswordHasher.Verify(request.Password, usuario.PasswordHash)) return null;

        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Falta Jwt:Key");
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()), new Claim(ClaimTypes.Name, usuario.NombreUsuario), new Claim(ClaimTypes.Role, usuario.Rol.Nombre) };
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"], audience: _configuration["Jwt:Audience"], claims: claims,
            expires: DateTime.UtcNow.AddHours(12), signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256));

        return new LoginResponse { Token = new JwtSecurityTokenHandler().WriteToken(token), UsuarioId = usuario.Id, NombreUsuario = usuario.NombreUsuario, NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}".Trim(), Rol = usuario.Rol.Nombre, AlumnoId = usuario.AlumnoId, Permisos = usuario.Rol.EsSistema ? ScreenKeys.Todas.Keys.OrderBy(x => x).ToList() : usuario.Rol.Permisos.Select(p => p.Pantalla).OrderBy(x => x).ToList() };
    }
}
