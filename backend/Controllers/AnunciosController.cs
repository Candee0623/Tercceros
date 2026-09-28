using System.Security.Claims;
using backend.DTOs.Anuncios;
using backend.Security;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnunciosController : ControllerBase
{
    private readonly AnuncioService _service;

    public AnunciosController(AnuncioService service)
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

    [HttpGet]
    [ScreenPermission(ScreenKeys.Anuncios)]
    public async Task<IActionResult> GetAll([FromQuery] bool todos = false)
    {
        var data = await _service.GetAllAsync(!todos);
        return Ok(data);
    }

    [HttpPost]
    [ScreenPermission(ScreenKeys.Anuncios)]
    public async Task<IActionResult> Crear([FromBody] CrearAnuncioDto dto)
    {
        var usuarioId = ObtenerUsuarioId();
        if (usuarioId == null) return Unauthorized();

        var (data, error) = await _service.CrearAsync(usuarioId.Value, dto);
        return error == null ? Ok(data) : BadRequest(error);
    }

    [HttpPut("{id:guid}")]
    [ScreenPermission(ScreenKeys.Anuncios)]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarAnuncioDto dto)
    {
        var (data, error) = await _service.ActualizarAsync(id, dto);
        return error == null ? Ok(data) : BadRequest(error);
    }

    [HttpDelete("{id:guid}")]
    [ScreenPermission(ScreenKeys.Anuncios)]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        var error = await _service.EliminarAsync(id);
        return error == null ? NoContent() : BadRequest(error);
    }
}
