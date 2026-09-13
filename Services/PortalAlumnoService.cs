using backend.Data;
using backend.DTOs.EstadoAsistencia;
using backend.DTOs.Matriculas;
using backend.DTOs.PortalAlumno;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class PortalAlumnoService
{
    private readonly AppDbContext _context;
    private readonly EstadoAsistenciaService _estadoAsistencia;

    public PortalAlumnoService(AppDbContext context, EstadoAsistenciaService estadoAsistencia)
    {
        _context = context;
        _estadoAsistencia = estadoAsistencia;
    }

    private async Task<Guid?> GetAlumnoIdAsync(Guid usuarioId) => await _context.Usuarios
        .AsNoTracking()
        .Where(u => u.Id == usuarioId && u.Activo)
        .Select(u => u.AlumnoId)
        .FirstOrDefaultAsync();

    public async Task<(MiPerfilAlumnoDto? Data, string? Error)> GetMiPerfilAsync(Guid usuarioId)
    {
        var alumnoId = await GetAlumnoIdAsync(usuarioId);
        if (!alumnoId.HasValue) return (null, "El usuario no tiene un alumno asociado.");

        var dto = await _context.Alumnos.AsNoTracking()
            .Where(a => a.Id == alumnoId.Value)
            .Select(a => new MiPerfilAlumnoDto
            {
                AlumnoId = a.Id,
                Nombre = a.Nombre,
                Apellido = a.Apellido,
                Dni = a.Dni,
                Email = a.Email,
                Telefono = a.Telefono,
                FechaNacimiento = a.FechaNacimiento,
                FechaIngreso = a.FechaIngreso,
                CarreraNombre = a.Carrera == null ? null : a.Carrera.Nombre
            }).FirstOrDefaultAsync();

        return dto == null ? (null, "El alumno asociado no existe.") : (dto, null);
    }

    public async Task<(EstadoAsistenciaAlumnoDto? Data, string? Error)> GetMiAsistenciaAsync(Guid usuarioId)
    {
        var alumnoId = await GetAlumnoIdAsync(usuarioId);
        if (!alumnoId.HasValue) return (null, "El usuario no tiene un alumno asociado.");
        var estado = await _estadoAsistencia.GetAsync(alumnoId.Value);
        return estado == null ? (null, "El alumno asociado no existe.") : (estado, null);
    }

    public async Task<(List<MiCursadaDisponibleDto>? Data, string? Error)> GetCursadasAsync(Guid usuarioId)
    {
        var alumnoId = await GetAlumnoIdAsync(usuarioId);
        if (!alumnoId.HasValue) return (null, "El usuario no tiene un alumno asociado.");

        var cursadas = await _context.Cursadas.AsNoTracking()
            .Where(c => c.Activa)
            .Include(c => c.Materia)
            .Include(c => c.Profesor)
            .Include(c => c.Horarios)
            .OrderByDescending(c => c.CicloLectivo)
            .ThenBy(c => c.Materia.Nombre)
            .ToListAsync();

        var matriculas = await _context.Matriculas.AsNoTracking()
            .Where(m => m.AlumnoId == alumnoId.Value)
            .Include(m => m.Asistencias)
            .ToListAsync();
        var porCursada = matriculas.ToDictionary(m => m.CursadaId);

        return (cursadas.Select(c =>
        {
            porCursada.TryGetValue(c.Id, out var m);
            var matriculado = m?.Activa == true;
            var tieneAsistencia = m?.Asistencias.Any() == true;
            return new MiCursadaDisponibleDto
            {
                CursadaId = c.Id,
                MateriaId = c.MateriaId,
                Materia = c.Materia.Nombre,
                Profesor = c.Profesor == null ? null : c.Profesor.Apellido + ", " + c.Profesor.Nombre,
                CicloLectivo = c.CicloLectivo,
                Periodo = c.Periodo,
                FechaInicio = c.FechaInicio,
                FechaFin = c.FechaFin,
                HorasSemanales = c.Horarios.Sum(h => (decimal)(h.HoraFin.ToTimeSpan() - h.HoraInicio.ToTimeSpan()).TotalHours),
                HorasTotalesPlanificadas = CursadaService.CalcularHorasTotales(c.FechaInicio, c.FechaFin, c.Horarios),
                PorcentajePromocion = c.PorcentajePromocion,
                PorcentajeRegularidad = c.PorcentajeRegularidad,
                Matriculado = matriculado,
                MatriculaId = m?.Id,
                FechaMatriculacion = matriculado ? m!.FechaMatriculacion : null,
                PuedeDesmatricularse = matriculado && !tieneAsistencia,
                Horarios = c.Horarios.OrderBy(h => h.DiaSemana).ThenBy(h => h.HoraInicio).Select(h => new MiHorarioCursadaDto
                {
                    DiaSemana = h.DiaSemana,
                    Dia = NombreDia(h.DiaSemana),
                    HoraInicio = h.HoraInicio.ToString("HH:mm"),
                    HoraFin = h.HoraFin.ToString("HH:mm")
                }).ToList()
            };
        }).ToList(), null);
    }

    public async Task<(MatriculaDto? Data, string? Error)> MatricularmeAsync(Guid usuarioId, Guid cursadaId)
    {
        var alumnoId = await GetAlumnoIdAsync(usuarioId);
        if (!alumnoId.HasValue) return (null, "El usuario no tiene un alumno asociado.");

        if (!await _context.Cursadas.AnyAsync(c => c.Id == cursadaId && c.Activa))
            return (null, "La cursada no existe o no está disponible.");

        var existente = await _context.Matriculas.FirstOrDefaultAsync(m => m.AlumnoId == alumnoId.Value && m.CursadaId == cursadaId);
        if (existente != null)
        {
            if (existente.Activa) return (null, "Ya estás matriculado en esta cursada.");
            existente.Activa = true;
            existente.FechaMatriculacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return (await MapMatriculaAsync(existente.Id), null);
        }

        var matricula = new Matricula
        {
            Id = Guid.NewGuid(),
            AlumnoId = alumnoId.Value,
            CursadaId = cursadaId,
            FechaMatriculacion = DateTime.UtcNow,
            Activa = true
        };
        _context.Matriculas.Add(matricula);
        await _context.SaveChangesAsync();
        return (await MapMatriculaAsync(matricula.Id), null);
    }

    public async Task<string?> DesmatricularmeAsync(Guid usuarioId, Guid matriculaId)
    {
        var alumnoId = await GetAlumnoIdAsync(usuarioId);
        if (!alumnoId.HasValue) return "El usuario no tiene un alumno asociado.";

        var matricula = await _context.Matriculas
            .Include(m => m.Asistencias)
            .FirstOrDefaultAsync(m => m.Id == matriculaId && m.AlumnoId == alumnoId.Value);
        if (matricula == null) return "La matrícula no existe o no te pertenece.";
        if (!matricula.Activa) return "La matrícula ya está inactiva.";
        if (matricula.Asistencias.Any())
            return "Ya existen registros de asistencia para esta cursada. La baja debe gestionarla administración.";

        matricula.Activa = false;
        await _context.SaveChangesAsync();
        return null;
    }

    private async Task<MatriculaDto?> MapMatriculaAsync(Guid id) => await _context.Matriculas.AsNoTracking()
        .Where(m => m.Id == id)
        .Select(m => new MatriculaDto
        {
            Id = m.Id,
            AlumnoId = m.AlumnoId,
            AlumnoNombre = m.Alumno.Apellido + ", " + m.Alumno.Nombre,
            AlumnoDni = m.Alumno.Dni,
            CursadaId = m.CursadaId,
            FechaMatriculacion = m.FechaMatriculacion,
            Activa = m.Activa
        }).FirstOrDefaultAsync();

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
