using backend.DTOs.Alumnos;
using backend.Services;
using backend.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ScreenPermission(ScreenKeys.Alumnos)]
public class AlumnosController : ControllerBase
{
    private readonly AlumnoService _alumnoService;

    public AlumnosController(AlumnoService alumnoService)
    {
        _alumnoService = alumnoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AlumnoDto>>> GetAll()
    {
        var alumnos = await _alumnoService.GetAllAsync();

        return Ok(alumnos);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AlumnoDto>> GetById(Guid id)
    {
        var alumno = await _alumnoService.GetByIdAsync(id);

        if (alumno == null)
        {
            return NotFound();
        }

        return Ok(alumno);
    }

    [HttpPost]
    public async Task<ActionResult<AlumnoDto>> Create(
        CreateAlumnoDto dto)
    {
        var alumno = await _alumnoService.CreateAsync(dto);

        if (alumno == null)
        {
            return BadRequest(
                "La carrera indicada no existe."
            );
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = alumno.Id },
            alumno
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AlumnoDto>> Update(
        Guid id,
        UpdateAlumnoDto dto)
    {
        var alumno = await _alumnoService.UpdateAsync(id, dto);

        if (alumno == null)
        {
            return NotFound(
                "El alumno no existe o la carrera indicada no es válida."
            );
        }

        return Ok(alumno);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var eliminado = await _alumnoService.DeleteAsync(id);

        if (!eliminado)
        {
            return NotFound();
        }

        return NoContent();
    }
}