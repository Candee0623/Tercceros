using System.Security.Claims;
using backend.DTOs.Usuarios;
using backend.Security;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PortalAlumnoController : ControllerBase
{
    private readonly PortalAlumnoService _service;
    private readonly UsuarioService _usuarioService;
    public PortalAlumnoController(PortalAlumnoService service, UsuarioService usuarioService)
    {
        _service = service;
        _usuarioService = usuarioService;
    }

    private Guid UsuarioId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("mis-datos")]
    [ScreenPermission(ScreenKeys.MisDatos)]
    public async Task<IActionResult> MisDatos()
    {
        var (data, error) = await _service.GetMiPerfilAsync(UsuarioId);
        return error == null ? Ok(data) : BadRequest(error);
    }

    [HttpGet("mi-asistencia")]
    [ScreenPermission(ScreenKeys.MiAsistencia)]
    public async Task<IActionResult> MiAsistencia()
    {
        var (data, error) = await _service.GetMiAsistenciaAsync(UsuarioId);
        return error == null ? Ok(data) : BadRequest(error);
    }

    [HttpGet("cursadas")]
    [ScreenPermission(ScreenKeys.AutoMatriculacion)]
    public async Task<IActionResult> Cursadas()
    {
        var (data, error) = await _service.GetCursadasAsync(UsuarioId);
        return error == null ? Ok(data) : BadRequest(error);
    }

    [HttpPost("matriculas/{cursadaId:guid}")]
    [ScreenPermission(ScreenKeys.AutoMatriculacion)]
    public async Task<IActionResult> Matricularme(Guid cursadaId)
    {
        var (data, error) = await _service.MatricularmeAsync(UsuarioId, cursadaId);
        return error == null ? Ok(data) : BadRequest(error);
    }

    [HttpDelete("matriculas/{matriculaId:guid}")]
    [ScreenPermission(ScreenKeys.AutoMatriculacion)]
    public async Task<IActionResult> Desmatricularme(Guid matriculaId)
    {
        var error = await _service.DesmatricularmeAsync(UsuarioId, matriculaId);
        return error == null ? NoContent() : BadRequest(error);
    }

    [HttpPost("cambiar-clave")]
    public async Task<IActionResult> CambiarClave(CambiarClaveDto dto)
    {
        var error = await _usuarioService.CambiarClaveAsync(UsuarioId, dto);
        return error == null ? Ok(new { mensaje = "Contraseña actualizada correctamente." }) : BadRequest(error);
    }
}
