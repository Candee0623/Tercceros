using backend.Data;
using backend.DTOs.Tramites;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class TramiteService
{
    private readonly AppDbContext _context;

    public TramiteService(AppDbContext context)
    {
        _context = context;
    }

    private static TramiteDto MapToDto(SolicitudTramite t)
    {
        return new TramiteDto
        {
            Id = t.Id,
            Codigo = t.Codigo,
            Tipo = t.Tipo,
            Motivo = t.Motivo,
            Estado = t.Estado,
            FechaSolicitud = t.FechaSolicitud,
            FechaResolucion = t.FechaResolucion,
            ObservacionResolucion = t.ObservacionResolucion,
            UsuarioId = t.UsuarioId,
            SolicitanteNombre = t.Usuario != null ? $"{t.Usuario.Nombre} {t.Usuario.Apellido}".Trim() : "Desconocido",
            AlumnoDni = t.Alumno != null ? t.Alumno.Dni : null
        };
    }

    public async Task<List<TramiteDto>> GetAllAsync()
    {
        var tramites = await _context.SolicitudesTramite
            .Include(t => t.Usuario)
            .Include(t => t.Alumno)
            .OrderByDescending(t => t.FechaSolicitud)
            .ToListAsync();

        return tramites.Select(MapToDto).ToList();
    }

    public async Task<List<TramiteDto>> GetMisTramitesAsync(Guid usuarioId)
    {
        var tramites = await _context.SolicitudesTramite
            .Include(t => t.Usuario)
            .Include(t => t.Alumno)
            .Where(t => t.UsuarioId == usuarioId)
            .OrderByDescending(t => t.FechaSolicitud)
            .ToListAsync();

        return tramites.Select(MapToDto).ToList();
    }

    public async Task<(TramiteDto? Data, string? Error)> CrearAsync(Guid usuarioId, CrearTramiteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Tipo))
            return (null, "El tipo de trámite es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.Motivo))
            return (null, "El motivo o detalle de la solicitud es obligatorio.");

        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null)
            return (null, "Usuario no encontrado.");

        var anio = DateTime.UtcNow.Year;
        var cantidadExistente = await _context.SolicitudesTramite
            .CountAsync(t => t.FechaSolicitud.Year == anio);
        var codigo = $"TR-{anio}-{(cantidadExistente + 1):D4}";

        var tramite = new SolicitudTramite
        {
            Id = Guid.NewGuid(),
            Codigo = codigo,
            Tipo = dto.Tipo.Trim(),
            Motivo = dto.Motivo.Trim(),
            Estado = "Pendiente",
            FechaSolicitud = DateTime.UtcNow,
            UsuarioId = usuarioId,
            AlumnoId = usuario.AlumnoId
        };

        _context.SolicitudesTramite.Add(tramite);
        await _context.SaveChangesAsync();

        if (usuario.AlumnoId.HasValue)
        {
            await _context.Entry(tramite).Reference(t => t.Alumno).LoadAsync();
        }
        tramite.Usuario = usuario;

        return (MapToDto(tramite), null);
    }

    public async Task<(TramiteDto? Data, string? Error)> ResolverAsync(Guid id, ResolverTramiteDto dto)
    {
        var tramite = await _context.SolicitudesTramite
            .Include(t => t.Usuario)
            .Include(t => t.Alumno)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tramite == null)
            return (null, "Trámite no encontrado.");

        tramite.Estado = string.IsNullOrWhiteSpace(dto.Estado) ? "Aprobado" : dto.Estado.Trim();
        tramite.ObservacionResolucion = dto.Observacion?.Trim();
        tramite.FechaResolucion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (MapToDto(tramite), null);
    }

    public async Task<string?> CancelarAsync(Guid id, Guid usuarioId, bool esAdmin)
    {
        var tramite = await _context.SolicitudesTramite.FindAsync(id);
        if (tramite == null)
            return "Trámite no encontrado.";

        if (!esAdmin && tramite.UsuarioId != usuarioId)
            return "No tienes permiso para cancelar este trámite.";

        if (!esAdmin && tramite.Estado != "Pendiente")
            return "Solo se pueden cancelar trámites en estado 'Pendiente'.";

        _context.SolicitudesTramite.Remove(tramite);
        await _context.SaveChangesAsync();
        return null;
    }
}
