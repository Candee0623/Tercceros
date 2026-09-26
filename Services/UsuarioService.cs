using backend.Data;
using backend.DTOs.Usuarios;
using backend.Entities;
using backend.Security;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class UsuarioService
{
    private readonly AppDbContext _context;
    public UsuarioService(AppDbContext context) { _context = context; }

    public async Task<List<UsuarioDto>> GetAllAsync() => await _context.Usuarios
        .AsNoTracking()
        .OrderBy(u => u.NombreUsuario)
        .Select(u => new UsuarioDto
        {
            Id = u.Id,
            NombreUsuario = u.NombreUsuario,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            Activo = u.Activo,
            RolId = u.RolId,
            RolNombre = u.Rol.Nombre,
            AlumnoId = u.AlumnoId,
            AlumnoNombre = u.Alumno == null ? null : u.Alumno.Apellido + ", " + u.Alumno.Nombre,
            ProfesorId = u.ProfesorId,
            ProfesorNombre = u.Profesor == null ? null : u.Profesor.Apellido + ", " + u.Profesor.Nombre
        }).ToListAsync();

    public async Task<List<UsuarioRolOptionDto>> GetRolesAsync() => await _context.Roles
        .Where(r => r.Activo)
        .OrderBy(r => r.Nombre)
        .Select(r => new UsuarioRolOptionDto { Id = r.Id, Nombre = r.Nombre })
        .ToListAsync();

    public async Task<List<UsuarioAlumnoOptionDto>> GetAlumnosAsync(Guid? includeAlumnoId = null) => await _context.Alumnos
        .AsNoTracking()
        .Where(a => a.Activo && (a.Id == includeAlumnoId || !_context.Usuarios.Any(u => u.AlumnoId == a.Id)))
        .OrderBy(a => a.Apellido).ThenBy(a => a.Nombre)
        .Select(a => new UsuarioAlumnoOptionDto
        {
            Id = a.Id,
            NombreCompleto = a.Apellido + ", " + a.Nombre,
            Dni = a.Dni
        }).ToListAsync();

    public async Task<List<UsuarioProfesorOptionDto>> GetProfesoresAsync(Guid? includeProfesorId = null) => await _context.Profesores
        .AsNoTracking()
        .Where(p => p.Activo && (p.Id == includeProfesorId || !_context.Usuarios.Any(u => u.ProfesorId == p.Id)))
        .OrderBy(p => p.Apellido).ThenBy(p => p.Nombre)
        .Select(p => new UsuarioProfesorOptionDto
        {
            Id = p.Id,
            NombreCompleto = p.Apellido + ", " + p.Nombre,
            Dni = p.Dni
        }).ToListAsync();

    private async Task<(Rol? Rol, string? Error, Guid? AlumnoId, Guid? ProfesorId)> ValidarRolUsuarioAsync(Guid rolId, Guid? alumnoId, Guid? profesorId, Guid? usuarioEditadoId = null)
    {
        var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Id == rolId && r.Activo);
        if (rol == null) return (null, "El rol indicado no existe o está inactivo.", null, null);

        var esAlumno = rol.Nombre.Equals("ALUMNO", StringComparison.OrdinalIgnoreCase);
        var esProfesor = rol.Nombre.Equals("PROFESOR", StringComparison.OrdinalIgnoreCase);

        if (!esAlumno && !esProfesor)
            return (rol, null, null, null); // Una cuenta no académica no conserva vínculo con la ficha.

        if (esAlumno)
        {
            if (!alumnoId.HasValue)
                return (rol, "Para un usuario con rol ALUMNO debés seleccionar el alumno asociado.", null, null);

            if (!await _context.Alumnos.AnyAsync(a => a.Id == alumnoId.Value && a.Activo))
                return (rol, "El alumno seleccionado no existe o está inactivo.", null, null);

            var yaUsado = await _context.Usuarios.AnyAsync(u => u.AlumnoId == alumnoId.Value && u.Id != usuarioEditadoId);
            if (yaUsado)
                return (rol, "Ese alumno ya está asociado a otro usuario.", null, null);

            return (rol, null, alumnoId, null);
        }

        if (!profesorId.HasValue)
            return (rol, "Para un usuario con rol PROFESOR debés seleccionar el profesor asociado.", null, null);

        if (!await _context.Profesores.AnyAsync(p => p.Id == profesorId.Value && p.Activo))
            return (rol, "El profesor seleccionado no existe o está inactivo.", null, null);

        var yaUsadoPorProfesor = await _context.Usuarios.AnyAsync(u => u.ProfesorId == profesorId.Value && u.Id != usuarioEditadoId);
        if (yaUsadoPorProfesor)
            return (rol, "Ese profesor ya está asociado a otro usuario.", null, null);

        return (rol, null, null, profesorId);
    }

    public async Task<(UsuarioDto? Usuario, string? Error)> CreateAsync(CreateUsuarioDto dto)
    {
        var username = dto.NombreUsuario.Trim();
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(dto.Password))
            return (null, "Usuario y contraseña son obligatorios.");
        if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario.ToLower() == username.ToLower()))
            return (null, "El nombre de usuario ya existe.");

        var (_, error, alumnoId, profesorId) = await ValidarRolUsuarioAsync(dto.RolId, dto.AlumnoId, dto.ProfesorId);
        if (error != null) return (null, error);

        var u = new Usuario
        {
            Id = Guid.NewGuid(),
            NombreUsuario = username,
            Nombre = dto.Nombre.Trim(),
            Apellido = dto.Apellido.Trim(),
            PasswordHash = PasswordHasher.Hash(dto.Password),
            RolId = dto.RolId,
            AlumnoId = alumnoId,
            ProfesorId = profesorId,
            Activo = dto.Activo
        };
        _context.Usuarios.Add(u);
        await _context.SaveChangesAsync();
        return ((await GetAllAsync()).First(x => x.Id == u.Id), null);
    }

    public async Task<(UsuarioDto? Usuario, string? Error)> UpdateAsync(Guid id, UpdateUsuarioDto dto, Guid currentUserId)
    {
        var u = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
        if (u == null) return (null, "Usuario no encontrado.");

        var username = dto.NombreUsuario.Trim();
        if (string.IsNullOrWhiteSpace(username)) return (null, "El nombre de usuario es obligatorio.");
        if (await _context.Usuarios.AnyAsync(x => x.Id != id && x.NombreUsuario.ToLower() == username.ToLower()))
            return (null, "El nombre de usuario ya existe.");
        if (id == currentUserId && !dto.Activo)
            return (null, "No podés desactivar el usuario con el que estás conectado.");

        var (_, error, alumnoId, profesorId) = await ValidarRolUsuarioAsync(dto.RolId, dto.AlumnoId, dto.ProfesorId, id);
        if (error != null) return (null, error);

        u.NombreUsuario = username;
        u.Nombre = dto.Nombre.Trim();
        u.Apellido = dto.Apellido.Trim();
        u.RolId = dto.RolId;
        u.AlumnoId = alumnoId;
        u.ProfesorId = profesorId;
        u.Activo = dto.Activo;
        if (!string.IsNullOrWhiteSpace(dto.Password)) u.PasswordHash = PasswordHasher.Hash(dto.Password);

        await _context.SaveChangesAsync();
        return ((await GetAllAsync()).First(x => x.Id == id), null);
    }

    public async Task<string?> CambiarClaveAsync(Guid usuarioId, CambiarClaveDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ClaveActual) || string.IsNullOrWhiteSpace(dto.NuevaClave))
            return "La clave actual y la nueva son obligatorias.";
        if (dto.NuevaClave.Length < 4)
            return "La nueva clave debe tener al menos 4 caracteres.";

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioId && u.Activo);
        if (usuario == null) return "El usuario no existe o está inactivo.";
        if (!PasswordHasher.Verify(dto.ClaveActual, usuario.PasswordHash))
            return "La clave actual es incorrecta.";

        usuario.PasswordHash = PasswordHasher.Hash(dto.NuevaClave);
        await _context.SaveChangesAsync();
        return null;
    }
}
