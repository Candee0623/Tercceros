using backend.Data;
using backend.DTOs.Asistencias;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class AsistenciaService
{
    private readonly AppDbContext _context;
    public AsistenciaService(AppDbContext context) => _context = context;

    public async Task<(PlanillaAsistenciaDto? data, string? error)> GetPlanillaAsync(Guid cursadaId, DateTime fecha)
    {
        var cursada = await _context.Cursadas.AsNoTracking()
            .Include(x => x.Horarios)
            .FirstOrDefaultAsync(x => x.Id == cursadaId);
        if (cursada == null) return (null, "La cursada no existe.");

        var f = DateTime.SpecifyKind(fecha.Date, DateTimeKind.Utc);
        var clase = await _context.Clases.AsNoTracking()
            .Include(x => x.Detalles)
            .FirstOrDefaultAsync(x => x.CursadaId == cursadaId && x.Fecha == f);

        var dia = ((int)fecha.DayOfWeek + 6) % 7 + 1;
        var horas = (decimal)cursada.Horarios
            .Where(h => h.DiaSemana == dia)
            .Sum(h => (h.HoraFin.ToTimeSpan() - h.HoraInicio.ToTimeSpan()).TotalHours);

        var mats = await _context.Matriculas.AsNoTracking()
            .Where(x => x.CursadaId == cursadaId && x.Activa)
            .OrderBy(x => x.Alumno.Apellido).ThenBy(x => x.Alumno.Nombre)
            .Select(x => new { x.Id, x.AlumnoId, Nombre = x.Alumno.Apellido + ", " + x.Alumno.Nombre, x.Alumno.Dni })
            .ToListAsync();

        var detalles = clase?.Detalles.ToDictionary(x => x.MatriculaId) ?? new Dictionary<Guid, AsistenciaDetalle>();
        return (new PlanillaAsistenciaDto
        {
            ClaseId = clase?.Id,
            CursadaId = cursadaId,
            Fecha = f,
            HorasProgramadas = clase?.HorasProgramadas ?? horas,
            Alumnos = mats.Select(m => detalles.TryGetValue(m.Id, out var d)
                ? new AsistenciaAlumnoDto { MatriculaId = m.Id, AlumnoId = m.AlumnoId, AlumnoNombre = m.Nombre, AlumnoDni = m.Dni, Estado = d.Estado, HorasAusente = d.HorasAusente, Observacion = d.Observacion }
                : new AsistenciaAlumnoDto { MatriculaId = m.Id, AlumnoId = m.AlumnoId, AlumnoNombre = m.Nombre, AlumnoDni = m.Dni, Estado = "PRESENTE", HorasAusente = 0 }).ToList()
        }, null);
    }

    public async Task<(PlanillaAsistenciaDto? data, string? error)> GuardarAsync(GuardarAsistenciaDto dto)
    {
        if (dto.HorasProgramadas <= 0) return (null, "Las horas programadas deben ser mayores a cero.");
        if (!await _context.Cursadas.AsNoTracking().AnyAsync(x => x.Id == dto.CursadaId)) return (null, "La cursada no existe.");

        var f = DateTime.SpecifyKind(dto.Fecha.Date, DateTimeKind.Utc);
        var matriculas = await _context.Matriculas.AsNoTracking()
            .Where(x => x.CursadaId == dto.CursadaId && x.Activa)
            .Select(x => x.Id)
            .ToListAsync();
        var matriculaIds = matriculas.ToHashSet();

        var nuevosDetalles = new List<(Guid matriculaId, string estado, decimal horas, string? observacion)>();
        foreach (var a in dto.Alumnos)
        {
            if (!matriculaIds.Contains(a.MatriculaId)) continue;
            var estado = (a.Estado ?? "").Trim().ToUpperInvariant();
            if (estado is not ("PRESENTE" or "AUSENCIA_TOTAL" or "AUSENCIA_PARCIAL"))
                return (null, "Estado de asistencia inválido.");

            var horas = estado switch
            {
                "PRESENTE" => 0m,
                "AUSENCIA_TOTAL" => dto.HorasProgramadas,
                _ => a.HorasAusente
            };
            if (horas < 0 || horas > dto.HorasProgramadas)
                return (null, "Las horas ausentes no pueden superar las horas de clase.");

            nuevosDetalles.Add((a.MatriculaId, estado, horas, a.Observacion));
        }

        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            // No trackeamos Clase/Detalles para guardar. El patrón anterior (Include + mutación de
            // la colección) podía dejar estados Deleted/Modified inconsistentes y disparar
            // DbUpdateConcurrencyException. Ahora cabecera y detalle se actualizan set-based.
            var claseId = await _context.Clases.AsNoTracking()
                .Where(x => x.CursadaId == dto.CursadaId && x.Fecha == f)
                .Select(x => (Guid?)x.Id)
                .FirstOrDefaultAsync();

            if (claseId.HasValue)
            {
                await _context.Clases
                    .Where(x => x.Id == claseId.Value)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(x => x.HorasProgramadas, dto.HorasProgramadas)
                        .SetProperty(x => x.FechaCarga, DateTime.UtcNow));
            }
            else
            {
                claseId = Guid.NewGuid();
                _context.Clases.Add(new Clase
                {
                    Id = claseId.Value,
                    CursadaId = dto.CursadaId,
                    Fecha = f,
                    HorasProgramadas = dto.HorasProgramadas,
                    FechaCarga = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
            }

            await _context.AsistenciaDetalles
                .Where(x => x.ClaseId == claseId.Value)
                .ExecuteDeleteAsync();

            var entidades = nuevosDetalles.Select(d => new AsistenciaDetalle
            {
                Id = Guid.NewGuid(),
                ClaseId = claseId.Value,
                MatriculaId = d.matriculaId,
                Estado = d.estado,
                HorasAusente = d.horas,
                Observacion = d.observacion
            }).ToList();

            if (entidades.Count > 0)
            {
                await _context.AsistenciaDetalles.AddRangeAsync(entidades);
                await _context.SaveChangesAsync();
            }

            await tx.CommitAsync();
            _context.ChangeTracker.Clear();
            return await GetPlanillaAsync(dto.CursadaId, f);
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}
