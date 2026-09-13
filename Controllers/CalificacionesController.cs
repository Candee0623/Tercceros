using backend.DTOs.Calificaciones;
using backend.Security;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ScreenPermission(ScreenKeys.Calificaciones)]
public class CalificacionesController : ControllerBase
{
    private readonly CalificacionService _service;
    public CalificacionesController(CalificacionService service) => _service = service;

    [HttpGet("planilla")]
    public async Task<IActionResult> Get([FromQuery] Guid cursadaId)
    {
        var (data, error) = await _service.GetPlanillaAsync(cursadaId);
        return error == null ? Ok(data) : BadRequest(error);
    }

    [HttpPost("generar/{cursadaId:guid}")]
    public async Task<IActionResult> Generar(Guid cursadaId)
    {
        var (data, error) = await _service.GenerarEvaluacionesAsync(cursadaId);
        return error == null ? Ok(data) : BadRequest(error);
    }

    [HttpPost]
    public async Task<IActionResult> Guardar(GuardarCalificacionesDto dto)
    {
        var (data, error) = await _service.GuardarAsync(dto);
        return error == null ? Ok(data) : BadRequest(error);
    }
}
