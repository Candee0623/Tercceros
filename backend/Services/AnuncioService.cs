using backend.Data;
using backend.DTOs.Anuncios;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class AnuncioService
{
    private readonly AppDbContext _context;

    public AnuncioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AnuncioDto>> GetAllAsync(bool soloActivos = true)
    {
        var query = _context.Anuncios
            .Include(a => a.Usuario)
            .AsNoTracking();

        if (soloActivos)
        {
            var hoy = DateTime.UtcNow;
            query = query.Where(a => a.Activo && (a.FechaVencimiento == null || a.FechaVencimiento >= hoy));
        }

        return await query
            .OrderByDescending(a => a.Prioridad == "Urgente")
            .ThenByDescending(a => a.FechaPublicacion)
            .Select(a => new AnuncioDto
            {
                Id = a.Id,
                Titulo = a.Titulo,
                Contenido = a.Contenido,
                Categoria = a.Categoria,
                Prioridad = a.Prioridad,
                FechaPublicacion = a.FechaPublicacion,
                FechaVencimiento = a.FechaVencimiento,
                Activo = a.Activo,
                UsuarioId = a.UsuarioId,
                AutorNombre = a.Usuario != null ? $"{a.Usuario.Nombre} {a.Usuario.Apellido}".Trim() : "Institucional"
            })
            .ToListAsync();
    }

    public async Task<(AnuncioDto? Data, string? Error)> CrearAsync(Guid usuarioId, CrearAnuncioDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Titulo))
            return (null, "El título es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.Contenido))
            return (null, "El contenido es obligatorio.");

        var anuncio = new Anuncio
        {
            Id = Guid.NewGuid(),
            Titulo = dto.Titulo.Trim(),
            Contenido = dto.Contenido.Trim(),
            Categoria = string.IsNullOrWhiteSpace(dto.Categoria) ? "General" : dto.Categoria.Trim(),
            Prioridad = string.IsNullOrWhiteSpace(dto.Prioridad) ? "Normal" : dto.Prioridad.Trim(),
            FechaPublicacion = DateTime.UtcNow,
            FechaVencimiento = dto.FechaVencimiento,
            Activo = true,
            UsuarioId = usuarioId
        };

        _context.Anuncios.Add(anuncio);
        await _context.SaveChangesAsync();

        var usuario = await _context.Usuarios.FindAsync(usuarioId);

        return (new AnuncioDto
        {
            Id = anuncio.Id,
            Titulo = anuncio.Titulo,
            Contenido = anuncio.Contenido,
            Categoria = anuncio.Categoria,
            Prioridad = anuncio.Prioridad,
            FechaPublicacion = anuncio.FechaPublicacion,
            FechaVencimiento = anuncio.FechaVencimiento,
            Activo = anuncio.Activo,
            UsuarioId = anuncio.UsuarioId,
            AutorNombre = usuario != null ? $"{usuario.Nombre} {usuario.Apellido}".Trim() : "Institucional"
        }, null);
    }

    public async Task<(AnuncioDto? Data, string? Error)> ActualizarAsync(Guid id, ActualizarAnuncioDto dto)
    {
        var anuncio = await _context.Anuncios.Include(a => a.Usuario).FirstOrDefaultAsync(a => a.Id == id);
        if (anuncio == null)
            return (null, "Anuncio no encontrado.");

        anuncio.Titulo = dto.Titulo.Trim();
        anuncio.Contenido = dto.Contenido.Trim();
        anuncio.Categoria = dto.Categoria.Trim();
        anuncio.Prioridad = dto.Prioridad.Trim();
        anuncio.FechaVencimiento = dto.FechaVencimiento;
        anuncio.Activo = dto.Activo;

        await _context.SaveChangesAsync();

        return (new AnuncioDto
        {
            Id = anuncio.Id,
            Titulo = anuncio.Titulo,
            Contenido = anuncio.Contenido,
            Categoria = anuncio.Categoria,
            Prioridad = anuncio.Prioridad,
            FechaPublicacion = anuncio.FechaPublicacion,
            FechaVencimiento = anuncio.FechaVencimiento,
            Activo = anuncio.Activo,
            UsuarioId = anuncio.UsuarioId,
            AutorNombre = anuncio.Usuario != null ? $"{anuncio.Usuario.Nombre} {anuncio.Usuario.Apellido}".Trim() : "Institucional"
        }, null);
    }

    public async Task<string?> EliminarAsync(Guid id)
    {
        var anuncio = await _context.Anuncios.FindAsync(id);
        if (anuncio == null)
            return "Anuncio no encontrado.";

        _context.Anuncios.Remove(anuncio);
        await _context.SaveChangesAsync();
        return null;
    }
}
