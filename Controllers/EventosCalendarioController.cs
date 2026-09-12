using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventosCalendarioController : ControllerBase
{
    private readonly AppDbContext _context;

    public EventosCalendarioController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventoCalendario>>> GetEventos()
    {
        var usuarioId = ObtenerUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var eventos = await _context.EventosCalendario
            .Where(e => e.UsuarioId == usuarioId.Value)
            .OrderBy(e => e.StartTime)
            .ToListAsync();

        return Ok(eventos);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventoCalendario>> GetEvento(Guid id)
    {
        var usuarioId = ObtenerUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var evento = await _context.EventosCalendario
            .FirstOrDefaultAsync(e =>
                e.Id == id &&
                e.UsuarioId == usuarioId.Value);

        if (evento == null)
            return NotFound();

        return Ok(evento);
    }

    [HttpPost]
    public async Task<ActionResult<EventoCalendario>> CrearEvento(
        [FromBody] CrearEventoRequest request)
    {
        var usuarioId = ObtenerUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var evento = new EventoCalendario
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            IsAllDay = request.IsAllDay,
            Location = request.Location,
            UsuarioId = usuarioId.Value
        };

        _context.EventosCalendario.Add(evento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEvento),
            new { id = evento.Id },
            evento);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> ActualizarEvento(
        Guid id,
        [FromBody] CrearEventoRequest request)
    {
        var usuarioId = ObtenerUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var evento = await _context.EventosCalendario
            .FirstOrDefaultAsync(e =>
                e.Id == id &&
                e.UsuarioId == usuarioId.Value);

        if (evento == null)
            return NotFound();

        evento.Title = request.Title;
        evento.Description = request.Description;
        evento.StartTime = request.StartTime;
        evento.EndTime = request.EndTime;
        evento.IsAllDay = request.IsAllDay;
        evento.Location = request.Location;

        await _context.SaveChangesAsync();

        return Ok(evento);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> EliminarEvento(Guid id)
    {
        var usuarioId = ObtenerUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var evento = await _context.EventosCalendario
            .FirstOrDefaultAsync(e =>
                e.Id == id &&
                e.UsuarioId == usuarioId.Value);

        if (evento == null)
            return NotFound();

        _context.EventosCalendario.Remove(evento);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private Guid? ObtenerUsuarioId()
    {
        var claim =
            User.FindFirst(ClaimTypes.NameIdentifier)
            ?? User.FindFirst("nameid")
            ?? User.FindFirst("sub")
            ?? User.FindFirst("id")
            ?? User.FindFirst("usuarioId");

        if (claim == null)
            return null;

        return Guid.TryParse(claim.Value, out var usuarioId)
            ? usuarioId
            : null;
    }
}

public class CrearEventoRequest
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public bool IsAllDay { get; set; }

    [MaxLength(100)]
    public string? Location { get; set; }
}