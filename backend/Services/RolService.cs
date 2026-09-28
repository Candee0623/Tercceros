using backend.Data;
using backend.DTOs.Roles;
using backend.Entities;
using backend.Security;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class RolService
{
    private readonly AppDbContext _context;
    public RolService(AppDbContext context) { _context = context; }

    public List<PantallaDto> GetPantallas() => ScreenKeys.Todas.Select(x => new PantallaDto { Clave = x.Key, Nombre = x.Value }).ToList();
    public async Task<List<RolDto>> GetAllAsync() => await _context.Roles.Include(r => r.Permisos).OrderBy(r => r.Nombre).Select(r => new RolDto { Id = r.Id, Nombre = r.Nombre, Descripcion = r.Descripcion, Activo = r.Activo, EsSistema = r.EsSistema, Permisos = r.Permisos.Select(p => p.Pantalla).ToList() }).ToListAsync();

    public async Task<(RolDto? Rol, string? Error)> CreateAsync(SaveRolDto dto)
    {
        var nombre = dto.Nombre.Trim();
        if (string.IsNullOrWhiteSpace(nombre)) return (null, "El nombre es obligatorio.");
        if (await _context.Roles.AnyAsync(r => r.Nombre.ToLower() == nombre.ToLower())) return (null, "Ya existe un rol con ese nombre.");
        var permisos = dto.Permisos.Distinct().Where(ScreenKeys.Todas.ContainsKey).ToList();
        // Comodidad: si se crea el rol ALUMNO sin marcar permisos, nace listo para autogestión.
        if (nombre.Equals("ALUMNO", StringComparison.OrdinalIgnoreCase) && permisos.Count == 0)
            permisos = ScreenKeys.PortalAlumno.ToList();
        var rol = new Rol { Id = Guid.NewGuid(), Nombre = nombre, Descripcion = dto.Descripcion?.Trim(), Activo = dto.Activo, Permisos = permisos.Select(p => new RolPermiso { Pantalla = p }).ToList() };
        _context.Roles.Add(rol); await _context.SaveChangesAsync();
        return ((await GetAllAsync()).First(r => r.Id == rol.Id), null);
    }

    public async Task<(RolDto? Rol, string? Error)> UpdateAsync(Guid id, SaveRolDto dto)
    {
        var rol = await _context.Roles.Include(r => r.Permisos).FirstOrDefaultAsync(r => r.Id == id);
        if (rol == null) return (null, "Rol no encontrado.");
        if (rol.EsSistema && !dto.Activo) return (null, "El rol de sistema no puede desactivarse.");
        var nombre = dto.Nombre.Trim();
        if (await _context.Roles.AnyAsync(r => r.Id != id && r.Nombre.ToLower() == nombre.ToLower())) return (null, "Ya existe un rol con ese nombre.");
        rol.Nombre = nombre; rol.Descripcion = dto.Descripcion?.Trim(); rol.Activo = dto.Activo;
        _context.RolPermisos.RemoveRange(rol.Permisos);
        rol.Permisos = dto.Permisos.Distinct().Where(ScreenKeys.Todas.ContainsKey).Select(p => new RolPermiso { RolId = id, Pantalla = p }).ToList();
        if (rol.EsSistema)
        {
            foreach (var key in ScreenKeys.Todas.Keys.Where(k => rol.Permisos.All(p => p.Pantalla != k))) rol.Permisos.Add(new RolPermiso { RolId = id, Pantalla = key });
        }
        await _context.SaveChangesAsync();
        return ((await GetAllAsync()).First(r => r.Id == id), null);
    }

    public async Task<string?> DeleteAsync(Guid id)
    {
        var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
        if (rol == null) return "Rol no encontrado.";
        if (rol.EsSistema) return "El rol de sistema no puede eliminarse.";
        if (await _context.Usuarios.AnyAsync(u => u.RolId == id)) return "No se puede eliminar un rol que tiene usuarios asignados.";
        _context.Roles.Remove(rol); await _context.SaveChangesAsync(); return null;
    }
}
