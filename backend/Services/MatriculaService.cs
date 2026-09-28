using backend.Data;
using backend.DTOs.Matriculas;
using backend.Entities;
using Microsoft.EntityFrameworkCore;
namespace backend.Services;
public class MatriculaService
{
    private readonly AppDbContext _context; public MatriculaService(AppDbContext context)=>_context=context;
    public async Task<List<MatriculaDto>> GetByCursadaAsync(Guid cursadaId)=>await _context.Matriculas.AsNoTracking().Where(x=>x.CursadaId==cursadaId && x.Activa).OrderBy(x=>x.Alumno.Apellido).ThenBy(x=>x.Alumno.Nombre).Select(x=>new MatriculaDto{Id=x.Id,AlumnoId=x.AlumnoId,AlumnoNombre=x.Alumno.Apellido+", "+x.Alumno.Nombre,AlumnoDni=x.Alumno.Dni,CursadaId=x.CursadaId,FechaMatriculacion=x.FechaMatriculacion,Activa=x.Activa}).ToListAsync();
    public async Task<(MatriculaDto? data,string? error)> MatricularAsync(Guid cursadaId, Guid alumnoId)
    {
        if(!await _context.Cursadas.AnyAsync(x=>x.Id==cursadaId && x.Activa))return(null,"La cursada no existe o está inactiva.");
        if(!await _context.Alumnos.AnyAsync(x=>x.Id==alumnoId && x.Activo))return(null,"El alumno no existe o está inactivo.");
        var existente=await _context.Matriculas.FirstOrDefaultAsync(x=>x.CursadaId==cursadaId&&x.AlumnoId==alumnoId);
        if(existente!=null){existente.Activa=true;await _context.SaveChangesAsync();return((await GetByCursadaAsync(cursadaId)).First(x=>x.Id==existente.Id),null);}
        var m=new Matricula{Id=Guid.NewGuid(),CursadaId=cursadaId,AlumnoId=alumnoId,FechaMatriculacion=DateTime.UtcNow,Activa=true};_context.Matriculas.Add(m);await _context.SaveChangesAsync();return((await GetByCursadaAsync(cursadaId)).First(x=>x.Id==m.Id),null);
    }
    public async Task<bool> DesmatricularAsync(Guid id){var m=await _context.Matriculas.FindAsync(id);if(m==null)return false;m.Activa=false;await _context.SaveChangesAsync();return true;}
}
