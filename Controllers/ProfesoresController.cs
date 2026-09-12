using backend.DTOs.Profesores;
using backend.Services;
using backend.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ScreenPermission(ScreenKeys.Profesores)]
public class ProfesoresController : ControllerBase
{
    private readonly ProfesorService _profesorService;

    public ProfesoresController(ProfesorService profesorService)
    {
        _profesorService = profesorService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProfesorDto>>> GetAll()
    {
        var profesores = await _profesorService.GetAllAsync();

        return Ok(profesores);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProfesorDto>> GetById(Guid id)
    {
        var profesor = await _profesorService.GetByIdAsync(id);

        if (profesor == null)
        {
            return NotFound();
        }

        return Ok(profesor);
    }

    [HttpPost]
    public async Task<ActionResult<ProfesorDto>> Create(
        CreateProfesorDto dto)
    {
        var profesor = await _profesorService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = profesor.Id },
            profesor
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProfesorDto>> Update(
        Guid id,
        UpdateProfesorDto dto)
    {
        var profesor = await _profesorService.UpdateAsync(id, dto);

        if (profesor == null)
        {
            return NotFound();
        }

        return Ok(profesor);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var eliminado = await _profesorService.DeleteAsync(id);

        if (!eliminado)
        {
            return NotFound();
        }

        return NoContent();
    }
}