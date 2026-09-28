using backend.Data;
using backend.DTOs.PlanEstudios;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class PlanEstudioService
{
    private readonly AppDbContext _context;

    public PlanEstudioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PlanEstudioCarreraDto>> GetPlanEstudiosAsync()
    {
        // La navegación inversa Carrera -> Materias no está declarada en la
        // entidad, así que resolvemos las materias con una consulta aparte
        // y las agrupamos por carrera en memoria (el volumen de datos de
        // un plan de estudios es chico, no hace falta optimizar esto).
        var materias = await _context.Materias
            .Include(m => m.Profesor)
            .OrderBy(m => m.Anio)
            .ThenBy(m => m.Nombre)
            .ToListAsync();

        var materiasPorCarrera = materias.GroupBy(m => m.CarreraId);

        var carrerasBase = await _context.Carreras
            .OrderBy(c => c.Nombre)
            .ToListAsync();

        var resultado = carrerasBase.Select(carrera =>
        {
            var materiasDeCarrera = materiasPorCarrera
                .FirstOrDefault(g => g.Key == carrera.Id)?
                .ToList() ?? new List<Materia>();

            var anios = Enumerable.Range(1, Math.Max(carrera.DuracionAnios, 1))
                .Select(anio => new PlanEstudioAnioDto
                {
                    Anio = anio,
                    Materias = materiasDeCarrera
                        .Where(m => m.Anio == anio)
                        .Select(m => new PlanEstudioMateriaDto
                        {
                            Id = m.Id,
                            Nombre = m.Nombre,
                            Codigo = m.Codigo,
                            Activa = m.Activa,
                            ProfesorNombre = m.Profesor != null
                                ? $"{m.Profesor.Nombre} {m.Profesor.Apellido}"
                                : null
                        })
                        .ToList()
                })
                .ToList();

            // Si hay materias cargadas con un año fuera del rango de
            // duración de la carrera (por ejemplo, un dato viejo o mal
            // cargado), las sumamos igual al final en vez de perderlas.
            var aniosFueraDeRango = materiasDeCarrera
                .Select(m => m.Anio)
                .Where(a => a < 1 || a > carrera.DuracionAnios)
                .Distinct()
                .OrderBy(a => a);

            foreach (var anioExtra in aniosFueraDeRango)
            {
                anios.Add(new PlanEstudioAnioDto
                {
                    Anio = anioExtra,
                    Materias = materiasDeCarrera
                        .Where(m => m.Anio == anioExtra)
                        .Select(m => new PlanEstudioMateriaDto
                        {
                            Id = m.Id,
                            Nombre = m.Nombre,
                            Codigo = m.Codigo,
                            Activa = m.Activa,
                            ProfesorNombre = m.Profesor != null
                                ? $"{m.Profesor.Nombre} {m.Profesor.Apellido}"
                                : null
                        })
                        .ToList()
                });
            }

            return new PlanEstudioCarreraDto
            {
                CarreraId = carrera.Id,
                CarreraNombre = carrera.Nombre,
                DuracionAnios = carrera.DuracionAnios,
                CarreraActiva = carrera.Activa,
                Anios = anios
            };
        }).ToList();

        return resultado;
    }
}
