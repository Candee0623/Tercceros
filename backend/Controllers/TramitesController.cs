using System.Security.Claims;
using backend.DTOs.Tramites;
using backend.Security;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TramitesController : ControllerBase
{
    private readonly TramiteService _service;

    public TramitesController(TramiteService service)
    {
        _service = service;
    }

    private Guid? ObtenerUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
            ?? User.FindFirst("nameid")
            ?? User.FindFirst("sub")
            ?? User.FindFirst("id")
            ?? User.FindFirst("usuarioId");

        if (claim == null) return null;
        return Guid.TryParse(claim.Value, out var usuarioId) ? usuarioId : null;
    }

    [HttpGet("mis-tramites")]
    [ScreenPermission(ScreenKeys.Tramites)]
    public async Task<IActionResult> GetMisTramites()
    {
        var usuarioId = ObtenerUsuarioId();
        if (usuarioId == null) return Unauthorized();

        var data = await _service.GetMisTramitesAsync(usuarioId.Value);
        return Ok(data);
    }

    [HttpGet]
    [ScreenPermission(ScreenKeys.Tramites)]
    public async Task<IActionResult> GetAll()
    {
        var data = await _service.GetAllAsync();
        return Ok(data);
    }

    [HttpPost]
    [ScreenPermission(ScreenKeys.Tramites)]
    public async Task<IActionResult> Crear([FromBody] CrearTramiteDto dto)
    {
        var usuarioId = ObtenerUsuarioId();
        if (usuarioId == null) return Unauthorized();

        var (data, error) = await _service.CrearAsync(usuarioId.Value, dto);
        return error == null ? Ok(data) : BadRequest(error);
    }

    [HttpPatch("{id:guid}/resolver")]
    [ScreenPermission(ScreenKeys.Tramites)]
    public async Task<IActionResult> Resolver(Guid id, [FromBody] ResolverTramiteDto dto)
    {
        var (data, error) = await _service.ResolverAsync(id, dto);
        return error == null ? Ok(data) : BadRequest(error);
    }

    [HttpDelete("{id:guid}")]
    [ScreenPermission(ScreenKeys.Tramites)]
    public async Task<IActionResult> Cancelar(Guid id)
    {
        var usuarioId = ObtenerUsuarioId();
        if (usuarioId == null) return Unauthorized();

        var esAdmin = User.IsInRole("ADMIN") || User.HasClaim(ClaimTypes.Role, "ADMIN");
        var error = await _service.CancelarAsync(id, usuarioId.Value, esAdmin);
        return error == null ? NoContent() : BadRequest(error);
    }
}
