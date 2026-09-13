using backend.Data;
using backend.DTOs.EstadoAsistencia;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class EstadoAsistenciaService
{
    private readonly AppDbContext _context;
    public EstadoAsistenciaService(AppDbContext context)=>_context=context;

    public async Task<List<AlumnoConsultaDto>> GetAlumnosAsync() => await _context.Matriculas.AsNoTracking()
        .Where(m=>m.Activa && m.Alumno.Activo).Select(m=>new AlumnoConsultaDto{Id=m.AlumnoId,NombreCompleto=m.Alumno.Apellido+", "+m.Alumno.Nombre,Dni=m.Alumno.Dni})
        .Distinct().OrderBy(x=>x.NombreCompleto).ToListAsync();

    public async Task<EstadoAsistenciaAlumnoDto?> GetAsync(Guid alumnoId)
    {
        var alumno=await _context.Alumnos.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==alumnoId); if(alumno==null)return null;
        var mats=await _context.Matriculas.AsNoTracking().Where(m=>m.AlumnoId==alumnoId&&m.Activa)
            .Include(m=>m.Cursada).ThenInclude(c=>c.Materia)
            .Include(m=>m.Cursada).ThenInclude(c=>c.Profesor)
            .Include(m=>m.Cursada).ThenInclude(c=>c.Horarios)
            .Include(m=>m.Asistencias).ToListAsync();

        var result=new EstadoAsistenciaAlumnoDto{AlumnoId=alumno.Id,AlumnoNombre=alumno.Apellido+", "+alumno.Nombre,Dni=alumno.Dni};
        foreach(var m in mats.OrderByDescending(x=>x.Cursada.CicloLectivo).ThenBy(x=>x.Cursada.Materia.Nombre))
        {
            var total=CursadaService.CalcularHorasTotales(m.Cursada.FechaInicio,m.Cursada.FechaFin,m.Cursada.Horarios);
            var ausentes=m.Asistencias.Sum(a=>a.HorasAusente);
            if(ausentes>total) ausentes=total;
            var asistidas=Math.Max(0,total-ausentes);
            var porcentaje=total<=0?100m:Math.Round(asistidas/total*100m,2);
            var condicion=porcentaje>=m.Cursada.PorcentajePromocion?"PROMOCIONA":porcentaje>=m.Cursada.PorcentajeRegularidad?"REGULAR": "LIBRE";
            result.Cursadas.Add(new EstadoAsistenciaCursadaDto{MatriculaId=m.Id,CursadaId=m.CursadaId,Materia=m.Cursada.Materia.Nombre,Periodo=m.Cursada.Periodo,CicloLectivo=m.Cursada.CicloLectivo,Profesor=m.Cursada.Profesor==null?null:m.Cursada.Profesor.Apellido+", "+m.Cursada.Profesor.Nombre,HorasTotalesPlanificadas=total,HorasAusentes=ausentes,HorasAsistidas=asistidas,PorcentajeAsistencia=porcentaje,PorcentajePromocion=m.Cursada.PorcentajePromocion,PorcentajeRegularidad=m.Cursada.PorcentajeRegularidad,PorcentajeLibre=m.Cursada.PorcentajeRegularidad,Condicion=condicion});
        }
        return result;
    }
}
