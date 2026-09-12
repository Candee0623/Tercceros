using backend.Data;
using backend.DTOs.Cuotas;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class CuotaService
{
    private readonly AppDbContext _context;
    public CuotaService(AppDbContext context) => _context = context;

    public async Task<List<EstadoCuotaDto>> GetAllAsync()
    {
        return await _context.Alumnos.AsNoTracking()
            .OrderBy(x => x.Apellido).ThenBy(x => x.Nombre)
            .Select(a => new EstadoCuotaDto
            {
                AlumnoId = a.Id,
                AlumnoNombre = a.Apellido + ", " + a.Nombre,
                AlumnoDni = a.Dni,
                Registrado = _context.EstadosCuotaAlumno.Any(c => c.AlumnoId == a.Id),
                AlDia = _context.EstadosCuotaAlumno.Where(c => c.AlumnoId == a.Id).Select(c => c.AlDia).FirstOrDefault(),
                UltimoPeriodoPagado = _context.EstadosCuotaAlumno.Where(c => c.AlumnoId == a.Id).Select(c => c.UltimoPeriodoPagado).FirstOrDefault(),
                FechaUltimoPago = _context.EstadosCuotaAlumno.Where(c => c.AlumnoId == a.Id).Select(c => c.FechaUltimoPago).FirstOrDefault(),
                ImporteUltimoPago = _context.EstadosCuotaAlumno.Where(c => c.AlumnoId == a.Id).Select(c => c.ImporteUltimoPago).FirstOrDefault(),
                Observacion = _context.EstadosCuotaAlumno.Where(c => c.AlumnoId == a.Id).Select(c => c.Observacion).FirstOrDefault(),
                FechaActualizacion = _context.EstadosCuotaAlumno.Where(c => c.AlumnoId == a.Id).Select(c => (DateTime?)c.FechaActualizacion).FirstOrDefault()
            }).ToListAsync();
    }

    public async Task<(EstadoCuotaDto? data, string? error)> GuardarAsync(Guid alumnoId, GuardarEstadoCuotaDto dto)
    {
        if (!await _context.Alumnos.AnyAsync(x => x.Id == alumnoId)) return (null, "El alumno no existe.");
        if (dto.ImporteUltimoPago < 0) return (null, "El importe no puede ser negativo.");
        var fechaPago = dto.FechaUltimoPago.HasValue ? DateTime.SpecifyKind(dto.FechaUltimoPago.Value.Date, DateTimeKind.Utc) : (DateTime?)null;

        var existente = await _context.EstadosCuotaAlumno.FirstOrDefaultAsync(x => x.AlumnoId == alumnoId);
        if (existente == null)
        {
            existente = new EstadoCuotaAlumno { Id = Guid.NewGuid(), AlumnoId = alumnoId };
            _context.EstadosCuotaAlumno.Add(existente);
        }
        existente.AlDia = dto.AlDia;
        existente.UltimoPeriodoPagado = string.IsNullOrWhiteSpace(dto.UltimoPeriodoPagado) ? null : dto.UltimoPeriodoPagado.Trim();
        existente.FechaUltimoPago = fechaPago;
        existente.ImporteUltimoPago = dto.ImporteUltimoPago;
        existente.Observacion = string.IsNullOrWhiteSpace(dto.Observacion) ? null : dto.Observacion.Trim();
        existente.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var all = await GetAllAsync();
        return (all.First(x => x.AlumnoId == alumnoId), null);
    }
}
