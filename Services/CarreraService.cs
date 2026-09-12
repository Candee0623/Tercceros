using backend.Data;
using backend.DTOs.Carreras;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class CarreraService
{
    private readonly AppDbContext _context;

    public CarreraService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CarreraDto>> GetAllAsync()
    {
        return await _context.Carreras
            .OrderBy(x => x.Nombre)
            .Select(x => new CarreraDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Descripcion = x.Descripcion,
                DuracionAnios = x.DuracionAnios,
                Activa = x.Activa,
                FechaCreacion = x.FechaCreacion
            })
            .ToListAsync();
    }

    public async Task<CarreraDto?> GetByIdAsync(Guid id)
    {
        return await _context.Carreras
            .Where(x => x.Id == id)
            .Select(x => new CarreraDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Descripcion = x.Descripcion,
                DuracionAnios = x.DuracionAnios,
                Activa = x.Activa,
                FechaCreacion = x.FechaCreacion
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CarreraDto> CreateAsync(CreateCarreraDto dto)
    {
        var carrera = new Carrera
        {
            Id = Guid.NewGuid(),
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            DuracionAnios = dto.DuracionAnios,
            Activa = true,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Carreras.Add(carrera);

        await _context.SaveChangesAsync();

        return new CarreraDto
        {
            Id = carrera.Id,
            Nombre = carrera.Nombre,
            Descripcion = carrera.Descripcion,
            DuracionAnios = carrera.DuracionAnios,
            Activa = carrera.Activa,
            FechaCreacion = carrera.FechaCreacion
        };
    }

    public async Task<CarreraDto?> UpdateAsync(Guid id, UpdateCarreraDto dto)
    {
        var carrera = await _context.Carreras
            .FirstOrDefaultAsync(x => x.Id == id);

        if (carrera == null)
        {
            return null;
        }

        carrera.Nombre = dto.Nombre;
        carrera.Descripcion = dto.Descripcion;
        carrera.DuracionAnios = dto.DuracionAnios;
        carrera.Activa = dto.Activa;

        await _context.SaveChangesAsync();

        return new CarreraDto
        {
            Id = carrera.Id,
            Nombre = carrera.Nombre,
            Descripcion = carrera.Descripcion,
            DuracionAnios = carrera.DuracionAnios,
            Activa = carrera.Activa,
            FechaCreacion = carrera.FechaCreacion
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var carrera = await _context.Carreras
            .FirstOrDefaultAsync(x => x.Id == id);

        if (carrera == null)
        {
            return false;
        }

        _context.Carreras.Remove(carrera);

        await _context.SaveChangesAsync();

        return true;
    }
}