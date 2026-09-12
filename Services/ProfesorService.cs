using backend.Data;
using backend.DTOs.Profesores;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ProfesorService
{
    private readonly AppDbContext _context;

    public ProfesorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProfesorDto>> GetAllAsync()
    {
        return await _context.Profesores
            .OrderBy(x => x.Apellido)
            .ThenBy(x => x.Nombre)
            .Select(x => new ProfesorDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Apellido = x.Apellido,
                Dni = x.Dni,
                Email = x.Email,
                Telefono = x.Telefono,
                Titulo = x.Titulo,
                Activo = x.Activo,
                FechaCreacion = x.FechaCreacion
            })
            .ToListAsync();
    }

    public async Task<ProfesorDto?> GetByIdAsync(Guid id)
    {
        return await _context.Profesores
            .Where(x => x.Id == id)
            .Select(x => new ProfesorDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Apellido = x.Apellido,
                Dni = x.Dni,
                Email = x.Email,
                Telefono = x.Telefono,
                Titulo = x.Titulo,
                Activo = x.Activo,
                FechaCreacion = x.FechaCreacion
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProfesorDto> CreateAsync(CreateProfesorDto dto)
    {
        var profesor = new Profesor
        {
            Id = Guid.NewGuid(),
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Dni = dto.Dni,
            Email = dto.Email,
            Telefono = dto.Telefono,
            Titulo = dto.Titulo,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Profesores.Add(profesor);

        await _context.SaveChangesAsync();

        return new ProfesorDto
        {
            Id = profesor.Id,
            Nombre = profesor.Nombre,
            Apellido = profesor.Apellido,
            Dni = profesor.Dni,
            Email = profesor.Email,
            Telefono = profesor.Telefono,
            Titulo = profesor.Titulo,
            Activo = profesor.Activo,
            FechaCreacion = profesor.FechaCreacion
        };
    }

    public async Task<ProfesorDto?> UpdateAsync(
        Guid id,
        UpdateProfesorDto dto)
    {
        var profesor = await _context.Profesores
            .FirstOrDefaultAsync(x => x.Id == id);

        if (profesor == null)
        {
            return null;
        }

        profesor.Nombre = dto.Nombre;
        profesor.Apellido = dto.Apellido;
        profesor.Dni = dto.Dni;
        profesor.Email = dto.Email;
        profesor.Telefono = dto.Telefono;
        profesor.Titulo = dto.Titulo;
        profesor.Activo = dto.Activo;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(profesor.Id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var profesor = await _context.Profesores
            .FirstOrDefaultAsync(x => x.Id == id);

        if (profesor == null)
        {
            return false;
        }

        _context.Profesores.Remove(profesor);

        await _context.SaveChangesAsync();

        return true;
    }
}