using backend.Data;
using backend.DTOs.Materias;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class MateriaService
{
    private readonly AppDbContext _context;

    public MateriaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MateriaDto>> GetAllAsync()
    {
        return await _context.Materias
            .OrderBy(x => x.Anio)
            .ThenBy(x => x.Nombre)
            .Select(x => new MateriaDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Codigo = x.Codigo,
                Anio = x.Anio,
                Activa = x.Activa,

                CarreraId = x.CarreraId,
                CarreraNombre = x.Carrera.Nombre,

                ProfesorId = x.ProfesorId,
                ProfesorNombre = x.Profesor != null
                    ? x.Profesor.Nombre + " " + x.Profesor.Apellido
                    : null,
                CantidadEvaluaciones = x.CantidadEvaluaciones,
                EsPromocionable = x.EsPromocionable,
                NotaPromocion = x.NotaPromocion,
                NotaRegularizacion = x.NotaRegularizacion
            })
            .ToListAsync();
    }

    public async Task<MateriaDto?> GetByIdAsync(Guid id)
    {
        return await _context.Materias
            .Where(x => x.Id == id)
            .Select(x => new MateriaDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Codigo = x.Codigo,
                Anio = x.Anio,
                Activa = x.Activa,

                CarreraId = x.CarreraId,
                CarreraNombre = x.Carrera.Nombre,

                ProfesorId = x.ProfesorId,
                ProfesorNombre = x.Profesor != null
                    ? x.Profesor.Nombre + " " + x.Profesor.Apellido
                    : null,
                CantidadEvaluaciones = x.CantidadEvaluaciones,
                EsPromocionable = x.EsPromocionable,
                NotaPromocion = x.NotaPromocion,
                NotaRegularizacion = x.NotaRegularizacion
            })
            .FirstOrDefaultAsync();
    }

    public async Task<MateriaDto?> CreateAsync(CreateMateriaDto dto)
    {
        if (dto.CantidadEvaluaciones < 1 || dto.CantidadEvaluaciones > 20) return null;
        if (dto.NotaRegularizacion < 0 || dto.NotaRegularizacion > 10) return null;
        if (dto.EsPromocionable && (!dto.NotaPromocion.HasValue || dto.NotaPromocion < dto.NotaRegularizacion || dto.NotaPromocion > 10)) return null;

        var carreraExiste = await _context.Carreras
            .AnyAsync(x => x.Id == dto.CarreraId);

        if (!carreraExiste)
        {
            return null;
        }

        if (dto.ProfesorId.HasValue)
        {
            var profesorExiste = await _context.Profesores
                .AnyAsync(x => x.Id == dto.ProfesorId.Value);

            if (!profesorExiste)
            {
                return null;
            }
        }

        var materia = new Materia
        {
            Id = Guid.NewGuid(),
            Nombre = dto.Nombre,
            Codigo = dto.Codigo,
            Anio = dto.Anio,
            Activa = true,
            CarreraId = dto.CarreraId,
            ProfesorId = dto.ProfesorId,
            CantidadEvaluaciones = dto.CantidadEvaluaciones,
            EsPromocionable = dto.EsPromocionable,
            NotaPromocion = dto.EsPromocionable ? dto.NotaPromocion : null,
            NotaRegularizacion = dto.NotaRegularizacion
        };

        _context.Materias.Add(materia);

        await _context.SaveChangesAsync();

        return await GetByIdAsync(materia.Id);
    }

    public async Task<MateriaDto?> UpdateAsync(
        Guid id,
        UpdateMateriaDto dto)
    {
        var materia = await _context.Materias
            .FirstOrDefaultAsync(x => x.Id == id);

        if (materia == null)
        {
            return null;
        }

        if (dto.CantidadEvaluaciones < 1 || dto.CantidadEvaluaciones > 20) return null;
        if (dto.NotaRegularizacion < 0 || dto.NotaRegularizacion > 10) return null;
        if (dto.EsPromocionable && (!dto.NotaPromocion.HasValue || dto.NotaPromocion < dto.NotaRegularizacion || dto.NotaPromocion > 10)) return null;

        var carreraExiste = await _context.Carreras
            .AnyAsync(x => x.Id == dto.CarreraId);

        if (!carreraExiste)
        {
            return null;
        }

        if (dto.ProfesorId.HasValue)
        {
            var profesorExiste = await _context.Profesores
                .AnyAsync(x => x.Id == dto.ProfesorId.Value);

            if (!profesorExiste)
            {
                return null;
            }
        }

        materia.Nombre = dto.Nombre;
        materia.Codigo = dto.Codigo;
        materia.Anio = dto.Anio;
        materia.Activa = dto.Activa;
        materia.CarreraId = dto.CarreraId;
        materia.ProfesorId = dto.ProfesorId;
        materia.CantidadEvaluaciones = dto.CantidadEvaluaciones;
        materia.EsPromocionable = dto.EsPromocionable;
        materia.NotaPromocion = dto.EsPromocionable ? dto.NotaPromocion : null;
        materia.NotaRegularizacion = dto.NotaRegularizacion;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(materia.Id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var materia = await _context.Materias
            .FirstOrDefaultAsync(x => x.Id == id);

        if (materia == null)
        {
            return false;
        }

        _context.Materias.Remove(materia);

        await _context.SaveChangesAsync();

        return true;
    }
}