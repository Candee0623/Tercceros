using backend.Data;
using backend.DTOs.Calificaciones;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class CalificacionService
{
    private readonly AppDbContext _context;
    public CalificacionService(AppDbContext context) => _context = context;

    public async Task<(PlanillaCalificacionesDto? data, string? error)> GetPlanillaAsync(Guid cursadaId)
    {
        var cursada = await _context.Cursadas.AsNoTracking().Include(x => x.Materia).FirstOrDefaultAsync(x => x.Id == cursadaId);
        if (cursada == null) return (null, "La cursada no existe.");

        var evaluaciones = await _context.Evaluaciones.AsNoTracking().Where(x => x.CursadaId == cursadaId).OrderBy(x => x.Numero).ToListAsync();
        var alumnos = await _context.Matriculas.AsNoTracking().Where(x => x.CursadaId == cursadaId && x.Activa)
            .OrderBy(x => x.Alumno.Apellido).ThenBy(x => x.Alumno.Nombre)
            .Select(x => new { x.Id, x.AlumnoId, Nombre = x.Alumno.Apellido + ", " + x.Alumno.Nombre, x.Alumno.Dni }).ToListAsync();
        var matIds = alumnos.Select(x => x.Id).ToList();
        var notas = await _context.Calificaciones.AsNoTracking().Where(x => matIds.Contains(x.MatriculaId) && x.Evaluacion.CursadaId == cursadaId).ToListAsync();
        var cuotaIds = await _context.EstadosCuotaAlumno.AsNoTracking().Where(x => x.AlDia).Select(x => x.AlumnoId).ToListAsync();
        var alDia = cuotaIds.ToHashSet();

        var dto = new PlanillaCalificacionesDto
        {
            CursadaId = cursadaId,
            CursadaNombre = $"{cursada.Materia.Nombre} · {cursada.CicloLectivo} · {cursada.Periodo}",
            EsPromocionable = cursada.Materia.EsPromocionable,
            NotaPromocion = cursada.Materia.NotaPromocion,
            NotaRegularizacion = cursada.Materia.NotaRegularizacion,
            CantidadEvaluaciones = cursada.Materia.CantidadEvaluaciones,
            Evaluaciones = evaluaciones.Select(e => new EvaluacionDto { Id = e.Id, Numero = e.Numero, Nombre = e.Nombre, Fecha = e.Fecha }).ToList()
        };

        foreach (var a in alumnos)
        {
            var fila = new AlumnoCalificacionDto { MatriculaId = a.Id, AlumnoId = a.AlumnoId, AlumnoNombre = a.Nombre, AlumnoDni = a.Dni, CuotaAlDia = alDia.Contains(a.AlumnoId) };
            fila.Notas = evaluaciones.Select(e => {
                var n = notas.FirstOrDefault(x => x.MatriculaId == a.Id && x.EvaluacionId == e.Id);
                return new NotaEvaluacionDto { EvaluacionId = e.Id, Nota = n?.Nota, Observacion = n?.Observacion };
            }).ToList();
            var cargadas = fila.Notas.Where(x => x.Nota.HasValue).Select(x => x.Nota!.Value).ToList();
            fila.Promedio = cargadas.Count == 0 ? null : Math.Round(cargadas.Average(), 2);
            if (!fila.CuotaAlDia) fila.Condicion = "BLOQUEADO_CUOTA";
            else if (evaluaciones.Count == 0 || cargadas.Count < evaluaciones.Count) fila.Condicion = "EN_CURSO";
            else if (cursada.Materia.EsPromocionable && cursada.Materia.NotaPromocion.HasValue && fila.Promedio >= cursada.Materia.NotaPromocion) fila.Condicion = "PROMOCIONA";
            else if (fila.Promedio >= cursada.Materia.NotaRegularizacion) fila.Condicion = "REGULAR";
            else fila.Condicion = "NO_REGULARIZA";
            dto.Alumnos.Add(fila);
        }
        return (dto, null);
    }

    public async Task<(PlanillaCalificacionesDto? data, string? error)> GenerarEvaluacionesAsync(Guid cursadaId)
    {
        var cursada = await _context.Cursadas.AsNoTracking().Include(x => x.Materia).FirstOrDefaultAsync(x => x.Id == cursadaId);
        if (cursada == null) return (null, "La cursada no existe.");
        var actuales = await _context.Evaluaciones.Where(x => x.CursadaId == cursadaId).OrderBy(x => x.Numero).ToListAsync();
        if (actuales.Count > cursada.Materia.CantidadEvaluaciones)
        {
            var sobrantes = actuales.Skip(cursada.Materia.CantidadEvaluaciones).Select(x => x.Id).ToList();
            if (await _context.Calificaciones.AnyAsync(x => sobrantes.Contains(x.EvaluacionId)))
                return (null, "No se puede reducir la cantidad de evaluaciones porque las instancias sobrantes ya tienen calificaciones.");
            await _context.Evaluaciones.Where(x => sobrantes.Contains(x.Id)).ExecuteDeleteAsync();
        }
        for (var i = actuales.Count + 1; i <= cursada.Materia.CantidadEvaluaciones; i++)
            _context.Evaluaciones.Add(new Evaluacion { Id = Guid.NewGuid(), CursadaId = cursadaId, Numero = i, Nombre = $"Evaluación {i}" });
        await _context.SaveChangesAsync();
        return await GetPlanillaAsync(cursadaId);
    }

    public async Task<(PlanillaCalificacionesDto? data, string? error)> GuardarAsync(GuardarCalificacionesDto dto)
    {
        var evalIds = await _context.Evaluaciones.AsNoTracking().Where(x => x.CursadaId == dto.CursadaId).Select(x => x.Id).ToListAsync();
        var validEval = evalIds.ToHashSet();
        var mats = await _context.Matriculas.AsNoTracking().Where(x => x.CursadaId == dto.CursadaId && x.Activa).Select(x => new { x.Id, x.AlumnoId, Nombre = x.Alumno.Apellido + ", " + x.Alumno.Nombre }).ToListAsync();
        var matMap = mats.ToDictionary(x => x.Id);
        var habilitados = (await _context.EstadosCuotaAlumno.AsNoTracking().Where(x => x.AlDia).Select(x => x.AlumnoId).ToListAsync()).ToHashSet();

        foreach (var n in dto.Notas)
        {
            if (!validEval.Contains(n.EvaluacionId) || !matMap.TryGetValue(n.MatriculaId, out var mat)) return (null, "La evaluación o matrícula no corresponde a la cursada.");
            if (n.Nota.HasValue && (n.Nota < 0 || n.Nota > 10)) return (null, "Las notas deben estar entre 0 y 10.");
            if (n.Nota.HasValue && !habilitados.Contains(mat.AlumnoId)) return (null, $"{mat.Nombre} no puede ser calificado porque no tiene la cuota al día.");
        }

        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var n in dto.Notas)
            {
                await _context.Calificaciones.Where(x => x.MatriculaId == n.MatriculaId && x.EvaluacionId == n.EvaluacionId).ExecuteDeleteAsync();
                if (n.Nota.HasValue)
                    _context.Calificaciones.Add(new Calificacion { Id = Guid.NewGuid(), MatriculaId = n.MatriculaId, EvaluacionId = n.EvaluacionId, Nota = n.Nota.Value, Observacion = n.Observacion, FechaCarga = DateTime.UtcNow });
            }
            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return await GetPlanillaAsync(dto.CursadaId);
        }
        catch { await tx.RollbackAsync(); throw; }
    }
}
