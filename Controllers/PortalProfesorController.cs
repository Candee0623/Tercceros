using System.Security.Claims;
using backend.DTOs.Asistencias;
using backend.DTOs.Calificaciones;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PortalProfesorController : ControllerBase
{
    private readonly PortalProfesorService _service;
    public PortalProfesorController(PortalProfesorService service) => _service = service;

    private Guid UsuarioId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("mis-cursadas")]
    public async Task<IActionResult> MisCursadas()
    {
        var (data, error) = await _service.GetMisCursadasAsync(UsuarioId);
        return error == null ? Ok(data) : BadRequest(error);
    }

    [HttpGet("planilla-asistencia")]
    public async Task<IActionResult> PlanillaAsistencia([FromQuery] Guid cursadaId, [FromQuery] DateTime fecha)
    {
        var (data, error) = await _service.GetPlanillaAsistenciaAsync(UsuarioId, cursadaId, fecha);
        return error == null ? Ok(data) : BadRequest(error);
    }

    [HttpPost("planilla-asistencia")]
    public async Task<IActionResult> GuardarAsistencia(GuardarAsistenciaDto dto)
    {
        var (data, error) = await _service.GuardarAsistenciaAsync(UsuarioId, dto);
        return error == null ? Ok(data) : BadRequest(error);
    }

    [HttpGet("planilla-calificaciones")]
    public async Task<IActionResult> PlanillaCalificaciones([FromQuery] Guid cursadaId)
    {
        var (data, error) = await _service.GetPlanillaCalificacionesAsync(UsuarioId, cursadaId);
        return error == null ? Ok(data) : BadRequest(error);
    }

    [HttpPost("planilla-calificaciones")]
    public async Task<IActionResult> GuardarCalificaciones(GuardarCalificacionesDto dto)
    {
        var (data, error) = await _service.GuardarCalificacionesAsync(UsuarioId, dto);
        return error == null ? Ok(data) : BadRequest(error);
    }
}