using backend.Data;
using backend.DTOs.Alumnos;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class AlumnoService
{
    private readonly AppDbContext _context;

    public AlumnoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AlumnoDto>> GetAllAsync()
    {
        return await _context.Alumnos
            .OrderBy(x => x.Apellido)
            .ThenBy(x => x.Nombre)
            .Select(x => new AlumnoDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Apellido = x.Apellido,
                Dni = x.Dni,
                Email = x.Email,
                Telefono = x.Telefono,
                FechaNacimiento = x.FechaNacimiento,
                FechaIngreso = x.FechaIngreso,
                Activo = x.Activo,
                CarreraId = x.CarreraId,
                CarreraNombre = x.Carrera != null
                    ? x.Carrera.Nombre
                    : null
            })
            .ToListAsync();
    }

    public async Task<AlumnoDto?> GetByIdAsync(Guid id)
    {
        return await _context.Alumnos
            .Where(x => x.Id == id)
            .Select(x => new AlumnoDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Apellido = x.Apellido,
                Dni = x.Dni,
                Email = x.Email,
                Telefono = x.Telefono,
                FechaNacimiento = x.FechaNacimiento,
                FechaIngreso = x.FechaIngreso,
                Activo = x.Activo,
                CarreraId = x.CarreraId,
                CarreraNombre = x.Carrera != null
                    ? x.Carrera.Nombre
                    : null
            })
            .FirstOrDefaultAsync();
    }

    public async Task<AlumnoDto?> CreateAsync(CreateAlumnoDto dto)
    {
        if (dto.CarreraId.HasValue)
        {
            var carreraExiste = await _context.Carreras
                .AnyAsync(x => x.Id == dto.CarreraId.Value);

            if (!carreraExiste)
            {
                return null;
            }
        }

        var alumno = new Alumno
        {
            Id = Guid.NewGuid(),
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Dni = dto.Dni,
            Email = dto.Email,
            Telefono = dto.Telefono,
            FechaNacimiento = dto.FechaNacimiento,
            FechaIngreso = DateTime.UtcNow,
            Activo = true,
            CarreraId = dto.CarreraId
        };

        _context.Alumnos.Add(alumno);

        await _context.SaveChangesAsync();

        return await GetByIdAsync(alumno.Id);
    }

    public async Task<AlumnoDto?> UpdateAsync(
        Guid id,
        UpdateAlumnoDto dto)
    {
        var alumno = await _context.Alumnos
            .FirstOrDefaultAsync(x => x.Id == id);

        if (alumno == null)
        {
            return null;
        }

        if (dto.CarreraId.HasValue)
        {
            var carreraExiste = await _context.Carreras
                .AnyAsync(x => x.Id == dto.CarreraId.Value);

            if (!carreraExiste)
            {
                return null;
            }
        }

        alumno.Nombre = dto.Nombre;
        alumno.Apellido = dto.Apellido;
        alumno.Dni = dto.Dni;
        alumno.Email = dto.Email;
        alumno.Telefono = dto.Telefono;
        alumno.FechaNacimiento = dto.FechaNacimiento;
        alumno.CarreraId = dto.CarreraId;
        alumno.Activo = dto.Activo;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(alumno.Id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var alumno = await _context.Alumnos
            .FirstOrDefaultAsync(x => x.Id == id);

        if (alumno == null)
        {
            return false;
        }

        _context.Alumnos.Remove(alumno);

        await _context.SaveChangesAsync();

        return true;
    }
}