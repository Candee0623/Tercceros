using System.Security.Claims;
using backend.DTOs.Mensajes;
using backend.Security;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ScreenPermission(ScreenKeys.Mensajes)]
public class MensajesController : ControllerBase
{
private readonly MensajeService _service;

public MensajesController(MensajeService service)
{
    _service = service;
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

private async Task<Guid?> ObtenerAlumnoIdAsync(Guid usuarioId)
{
    return await _service.ObtenerAlumnoIdUsuarioAsync(usuarioId);
}

[HttpPost]
public async Task<IActionResult> Enviar([FromBody] CrearMensajeDto dto)
{
    var usuarioId = ObtenerUsuarioId();

    if (usuarioId == null)
        return Unauthorized();

    var (data, error) = await _service.EnviarAsync(
        usuarioId.Value,
        dto
    );

    if (error != null)
        return BadRequest(error);

    return Ok(data);
}

[HttpGet("bandeja")]
public async Task<IActionResult> Bandeja()
{
    var usuarioId = ObtenerUsuarioId();

    if (usuarioId == null)
        return Unauthorized();

    var alumnoId = await ObtenerAlumnoIdAsync(usuarioId.Value);

    if (alumnoId == null)
        return BadRequest(
            "El usuario actual no tiene un alumno asociado."
        );

    var data =
        await _service.ObtenerBandejaAlumnoAsync(
            alumnoId.Value
        );

    return Ok(data);
}

[HttpGet("no-leidos")]
public async Task<IActionResult> NoLeidos()
{
    var usuarioId = ObtenerUsuarioId();

    if (usuarioId == null)
        return Unauthorized();

    var alumnoId = await ObtenerAlumnoIdAsync(usuarioId.Value);

    if (alumnoId == null)
        return BadRequest(
            "El usuario actual no tiene un alumno asociado."
        );

    var cantidad =
        await _service.ContarNoLeidosAsync(
            alumnoId.Value
        );

    return Ok(new { cantidad });
}

[HttpGet("{id:guid}")]
public async Task<IActionResult> Detalle(Guid id)
{
    var usuarioId = ObtenerUsuarioId();

    if (usuarioId == null)
        return Unauthorized();

    var alumnoId = await ObtenerAlumnoIdAsync(usuarioId.Value);

    if (alumnoId == null)
        return BadRequest(
            "El usuario actual no tiene un alumno asociado."
        );

    var data =
        await _service.ObtenerDetalleAlumnoAsync(
            alumnoId.Value,
            id
        );

    if (data == null)
        return NotFound(
            "El mensaje no existe o no pertenece al usuario."
        );

    return Ok(data);
}

[HttpPut("{id:guid}/leido")]
public async Task<IActionResult> MarcarLeido(Guid id)
{
    var usuarioId = ObtenerUsuarioId();

    if (usuarioId == null)
        return Unauthorized();

    var alumnoId = await ObtenerAlumnoIdAsync(usuarioId.Value);

    if (alumnoId == null)
        return BadRequest(
            "El usuario actual no tiene un alumno asociado."
        );

    var actualizado =
        await _service.MarcarLeidoAsync(
            alumnoId.Value,
            id
        );

    if (!actualizado)
        return NotFound(
            "El mensaje no existe o no pertenece al usuario."
        );

    return NoContent();
}

[HttpGet("enviados")]
public async Task<IActionResult> Enviados()
{
    var usuarioId = ObtenerUsuarioId();

    if (usuarioId == null)
        return Unauthorized();

    var data =
        await _service.ObtenerEnviadosAsync(
            usuarioId.Value
        );

    return Ok(data);
}

[HttpGet("cursadas-profesor")]
public async Task<IActionResult> CursadasProfesor()
{
    var usuarioId = ObtenerUsuarioId();

    if (usuarioId == null)
        return Unauthorized();

    var profesorId =
        await _service.ObtenerProfesorIdUsuarioAsync(
            usuarioId.Value
        );

    if (profesorId == null)
        return BadRequest(
            "El usuario actual no tiene un profesor asociado."
        );

    var data =
        await _service.ObtenerCursadasProfesorAsync(
            profesorId.Value
        );

    return Ok(data);
}

[HttpGet("cursadas")]
public async Task<IActionResult> Cursadas()
{
    var usuarioId = ObtenerUsuarioId();

    if (usuarioId == null)
        return Unauthorized();

    var esSistema =
        await _service.UsuarioEsSistemaAsync(
            usuarioId.Value
        );

    if (!esSistema)
        return Forbid();

    var data =
        await _service.ObtenerCursadasDisponiblesAsync();

    return Ok(data);
}

}
