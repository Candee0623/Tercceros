using backend.Data;
using backend.DTOs.Cursadas;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class CursadaService
{
    private readonly AppDbContext _context;
    public CursadaService(AppDbContext context) => _context = context;

    public async Task<List<CursadaDto>> GetAllAsync()
    {
        var cursadas = await _context.Cursadas.AsNoTracking()
            .Include(x => x.Materia).Include(x => x.Profesor).Include(x => x.Horarios)
            .OrderByDescending(x => x.CicloLectivo).ThenBy(x => x.Materia.Nombre).ToListAsync();

        return cursadas.Select(Map).ToList();
    }

    public async Task<CursadaDto?> GetByIdAsync(Guid id)
    {
        var c = await _context.Cursadas.AsNoTracking().Include(x=>x.Materia).Include(x=>x.Profesor).Include(x=>x.Horarios).FirstOrDefaultAsync(x=>x.Id==id);
        return c == null ? null : Map(c);
    }

    public async Task<(CursadaDto? data,string? error)> CreateAsync(SaveCursadaDto dto)
    {
        var validation = await Validate(dto); if(validation!=null) return (null,validation);
        var c = new Cursada { Id=Guid.NewGuid(), MateriaId=dto.MateriaId, ProfesorId=dto.ProfesorId, CicloLectivo=dto.CicloLectivo, Periodo=dto.Periodo.Trim(), FechaInicio=Utc(dto.FechaInicio), FechaFin=Utc(dto.FechaFin), Activa=dto.Activa, PorcentajePromocion=dto.PorcentajePromocion, PorcentajeRegularidad=dto.PorcentajeRegularidad };
        c.Horarios = dto.Horarios.Select(h=>new CursadaHorario{Id=Guid.NewGuid(),CursadaId=c.Id,DiaSemana=h.DiaSemana,HoraInicio=TimeOnly.Parse(h.HoraInicio),HoraFin=TimeOnly.Parse(h.HoraFin)}).ToList();
        _context.Cursadas.Add(c); await _context.SaveChangesAsync(); return (await GetByIdAsync(c.Id),null);
    }

    public async Task<(CursadaDto? data,string? error)> UpdateAsync(Guid id, SaveCursadaDto dto)
    {
        var validation = await Validate(dto);
        if (validation != null) return (null, validation);

        // IMPORTANTE:
        // Los horarios se reemplazan de forma explícita en base de datos en lugar de
        // sincronizar una navegación trackeada. Esto evita DbUpdateConcurrencyException
        // al agregar/quitar días de una cursada existente.
        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            var c = await _context.Cursadas.FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return (null, "La cursada no existe.");

            c.MateriaId = dto.MateriaId;
            c.ProfesorId = dto.ProfesorId;
            c.CicloLectivo = dto.CicloLectivo;
            c.Periodo = dto.Periodo.Trim();
            c.FechaInicio = Utc(dto.FechaInicio);
            c.FechaFin = Utc(dto.FechaFin);
            c.Activa = dto.Activa;
            c.PorcentajePromocion = dto.PorcentajePromocion;
            c.PorcentajeRegularidad = dto.PorcentajeRegularidad;

            // Persistimos primero la cabecera.
            await _context.SaveChangesAsync();

            // Borrado set-based: no entra en el ChangeTracker y por lo tanto no puede
            // producir el conflicto de concurrencia que teníamos con RemoveRange.
            await _context.CursadaHorarios
                .Where(x => x.CursadaId == id)
                .ExecuteDeleteAsync();

            var nuevosHorarios = dto.Horarios.Select(h => new CursadaHorario
            {
                Id = Guid.NewGuid(),
                CursadaId = id,
                DiaSemana = h.DiaSemana,
                HoraInicio = TimeOnly.Parse(h.HoraInicio),
                HoraFin = TimeOnly.Parse(h.HoraFin)
            }).ToList();

            await _context.CursadaHorarios.AddRangeAsync(nuevosHorarios);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return (await GetByIdAsync(id), null);
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    private async Task<string?> Validate(SaveCursadaDto dto)
    {
        if(!await _context.Materias.AnyAsync(x=>x.Id==dto.MateriaId && x.Activa)) return "La materia indicada no existe o está inactiva.";
        if(dto.ProfesorId.HasValue && !await _context.Profesores.AnyAsync(x=>x.Id==dto.ProfesorId && x.Activo)) return "El profesor indicado no existe o está inactivo.";
        if(dto.FechaFin.Date<dto.FechaInicio.Date) return "La fecha de fin no puede ser anterior a la de inicio.";
        if(dto.PorcentajePromocion<0||dto.PorcentajePromocion>100||dto.PorcentajeRegularidad<0||dto.PorcentajeRegularidad>100) return "Los porcentajes de asistencia deben estar entre 0 y 100.";
        if(dto.PorcentajePromocion<dto.PorcentajeRegularidad) return "El porcentaje para promocionar no puede ser menor al porcentaje para regularizar.";
        if(dto.Horarios.Count==0) return "Debe cargar al menos un día y horario de cursada.";
        foreach(var h in dto.Horarios){ if(h.DiaSemana<1||h.DiaSemana>7)return "Día de semana inválido."; if(!TimeOnly.TryParse(h.HoraInicio,out var ini)||!TimeOnly.TryParse(h.HoraFin,out var fin)||fin<=ini)return "Los horarios son inválidos."; }
        return null;
    }

    private static CursadaDto Map(Cursada c)
    {
        return new CursadaDto
        {
            Id=c.Id,MateriaId=c.MateriaId,MateriaNombre=c.Materia.Nombre,ProfesorId=c.ProfesorId,
            ProfesorNombre=c.Profesor==null?null:c.Profesor.Nombre+" "+c.Profesor.Apellido,CicloLectivo=c.CicloLectivo,Periodo=c.Periodo,
            FechaInicio=c.FechaInicio,FechaFin=c.FechaFin,Activa=c.Activa,PorcentajePromocion=c.PorcentajePromocion,PorcentajeRegularidad=c.PorcentajeRegularidad,
            HorasSemanales=c.Horarios.Sum(Horas),HorasTotalesPlanificadas=CalcularHorasTotales(c.FechaInicio,c.FechaFin,c.Horarios),
            Horarios=c.Horarios.OrderBy(h=>h.DiaSemana).ThenBy(h=>h.HoraInicio).Select(h=>new HorarioDto{Id=h.Id,DiaSemana=h.DiaSemana,HoraInicio=h.HoraInicio.ToString("HH:mm"),HoraFin=h.HoraFin.ToString("HH:mm")}).ToList()
        };
    }

    public static decimal CalcularHorasTotales(DateTime desde, DateTime hasta, IEnumerable<CursadaHorario> horarios)
    {
        var porDia = horarios.GroupBy(h=>h.DiaSemana).ToDictionary(g=>g.Key,g=>g.Sum(Horas));
        decimal total=0;
        for(var d=desde.Date;d<=hasta.Date;d=d.AddDays(1))
        {
            var dia=((int)d.DayOfWeek+6)%7+1;
            if(porDia.TryGetValue(dia,out var horas)) total+=horas;
        }
        return total;
    }

    private static decimal Horas(CursadaHorario h)=>(decimal)(h.HoraFin.ToTimeSpan()-h.HoraInicio.ToTimeSpan()).TotalHours;
    private static DateTime Utc(DateTime d)=>DateTime.SpecifyKind(d.Date,DateTimeKind.Utc);
}
