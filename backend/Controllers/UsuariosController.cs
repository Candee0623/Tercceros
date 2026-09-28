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
[ScreenPermission(ScreenKeys.Usuarios)]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _service;

    public UsuariosController(UsuarioService service)
    {
        _service = service;
    }

    // ============================================================
    // USUARIO ACTUAL
    // ============================================================

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

        return Guid.TryParse(
            claim.Value,
            out var usuarioId)
                ? usuarioId
                : null;
    }

    // ============================================================
    // LISTADO
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _service.GetAllAsync();

        return Ok(data);
    }

    // ============================================================
    // ROLES
    // ============================================================

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        var data = await _service.GetRolesAsync();

        return Ok(data);
    }

    // ============================================================
    // ALUMNOS DISPONIBLES
    // ============================================================

    [HttpGet("alumnos")]
    public async Task<IActionResult> GetAlumnos(
        [FromQuery] Guid? includeAlumnoId = null)
    {
        var data =
            await _service.GetAlumnosAsync(
                includeAlumnoId);

        return Ok(data);
    }

    // ============================================================
    // PROFESORES DISPONIBLES
    // ============================================================

    [HttpGet("profesores")]
    public async Task<IActionResult> GetProfesores(
        [FromQuery] Guid? includeProfesorId = null)
    {
        var data =
            await _service.GetProfesoresAsync(
                includeProfesorId);

        return Ok(data);
    }

    // ============================================================
    // CREAR
    // ============================================================

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUsuarioDto dto)
    {
        var (usuario, error) =
            await _service.CreateAsync(dto);

        if (error != null)
            return BadRequest(error);

        return Ok(usuario);
    }

    // ============================================================
    // ACTUALIZAR
    // ============================================================

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateUsuarioDto dto)
    {
        var currentUserId = ObtenerUsuarioId();

        if (currentUserId == null)
            return Unauthorized();

        var (usuario, error) =
            await _service.UpdateAsync(
                id,
                dto,
                currentUserId.Value);

        if (error != null)
            return BadRequest(error);

        return Ok(usuario);
    }
}