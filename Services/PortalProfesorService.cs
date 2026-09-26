using backend.Data;
using backend.DTOs.Asistencias;
using backend.DTOs.Calificaciones;
using backend.DTOs.PortalProfesor;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class PortalProfesorService
{
    private readonly AppDbContext _context;
    private readonly AsistenciaService _asistencia;
    private readonly CalificacionService _calificacion;

    public PortalProfesorService(AppDbContext context, AsistenciaService asistencia, CalificacionService calificacion)
    {
        _context = context;
        _asistencia = asistencia;
        _calificacion = calificacion;
    }

    private async Task<Guid?> GetProfesorIdAsync(Guid usuarioId) => await _context.Usuarios
        .AsNoTracking()
        .Where(u => u.Id == usuarioId && u.Activo)
        .Select(u => u.ProfesorId)
        .FirstOrDefaultAsync();

    private async Task<string?> VerificarCursadaAsync(Guid usuarioId, Guid cursadaId)
    {
        var profesorId = await GetProfesorIdAsync(usuarioId);
        if (!profesorId.HasValue) return "El usuario no tiene un profesor asociado.";

        var esSuya = await _context.Cursadas.AsNoTracking()
            .AnyAsync(c => c.Id == cursadaId && c.ProfesorId == profesorId.Value);
        return esSuya ? null : "La cursada no existe o no te pertenece.";
    }

    public async Task<(List<MiCursadaProfesorDto>? Data, string? Error)> GetMisCursadasAsync(Guid usuarioId)
    {
        var profesorId = await GetProfesorIdAsync(usuarioId);
        if (!profesorId.HasValue) return (null, "El usuario no tiene un profesor asociado.");

        var cursadas = await _context.Cursadas.AsNoTracking()
            .Where(c => c.ProfesorId == profesorId.Value && c.Activa)
            .Include(c => c.Materia)
            .Include(c => c.Horarios)
            .Include(c => c.Matriculas)
            .OrderByDescending(c => c.CicloLectivo)
            .ThenBy(c => c.Materia.Nombre)
            .ToListAsync();

        var datos = cursadas.Select(c => new MiCursadaProfesorDto
        {
            CursadaId = c.Id,
            Materia = c.Materia.Nombre,
            CicloLectivo = c.CicloLectivo,
            Periodo = c.Periodo,
            FechaInicio = c.FechaInicio,
            FechaFin = c.FechaFin,
            PorcentajePromocion = c.PorcentajePromocion,
            PorcentajeRegularidad = c.PorcentajeRegularidad,
            CantidadAlumnos = c.Matriculas.Count(m => m.Activa),
            Horarios = c.Horarios.OrderBy(h => h.DiaSemana).ThenBy(h => h.HoraInicio)
                .Select(h => $"{NombreDia(h.DiaSemana)} {h.HoraInicio:HH\\:mm}-{h.HoraFin:HH\\:mm}")
                .ToList()
        }).ToList();

        return (datos, null);
    }

    public async Task<(PlanillaAsistenciaDto? Data, string? Error)> GetPlanillaAsistenciaAsync(Guid usuarioId, Guid cursadaId, DateTime fecha)
    {
        var error = await VerificarCursadaAsync(usuarioId, cursadaId);
        if (error != null) return (null, error);
        return await _asistencia.GetPlanillaAsync(cursadaId, fecha);
    }

    public async Task<(PlanillaAsistenciaDto? Data, string? Error)> GuardarAsistenciaAsync(Guid usuarioId, GuardarAsistenciaDto dto)
    {
        var error = await VerificarCursadaAsync(usuarioId, dto.CursadaId);
        if (error != null) return (null, error);
        return await _asistencia.GuardarAsync(dto);
    }

    public async Task<(PlanillaCalificacionesDto? Data, string? Error)> GetPlanillaCalificacionesAsync(Guid usuarioId, Guid cursadaId)
    {
        var error = await VerificarCursadaAsync(usuarioId, cursadaId);
        if (error != null) return (null, error);
        return await _calificacion.GetPlanillaAsync(cursadaId);
    }

    public async Task<(PlanillaCalificacionesDto? Data, string? Error)> GuardarCalificacionesAsync(Guid usuarioId, GuardarCalificacionesDto dto)
    {
        var error = await VerificarCursadaAsync(usuarioId, dto.CursadaId);
        if (error != null) return (null, error);
        return await _calificacion.GuardarAsync(dto);
    }

    private static string NombreDia(int dia) => dia switch
    {
        1 => "Lunes",
        2 => "Martes",
        3 => "Miércoles",
        4 => "Jueves",
        5 => "Viernes",
        6 => "Sábado",
        7 => "Domingo",
        _ => $"Día {dia}"
    };
}