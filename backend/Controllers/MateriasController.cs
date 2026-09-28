using backend.DTOs.Materias;
using backend.Services;
using backend.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ScreenPermission(ScreenKeys.Materias)]
public class MateriasController : ControllerBase
{
    private readonly MateriaService _materiaService;

    public MateriasController(MateriaService materiaService)
    {
        _materiaService = materiaService;
    }

    [HttpGet]
    public async Task<ActionResult<List<MateriaDto>>> GetAll()
    {
        var materias = await _materiaService.GetAllAsync();

        return Ok(materias);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MateriaDto>> GetById(Guid id)
    {
        var materia = await _materiaService.GetByIdAsync(id);

        if (materia == null)
        {
            return NotFound();
        }

        return Ok(materia);
    }

    [HttpPost]
    public async Task<ActionResult<MateriaDto>> Create(
        CreateMateriaDto dto)
    {
        var materia = await _materiaService.CreateAsync(dto);

        if (materia == null)
        {
            return BadRequest(
                "La carrera o el profesor indicado no existe."
            );
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = materia.Id },
            materia
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<MateriaDto>> Update(
        Guid id,
        UpdateMateriaDto dto)
    {
        var materia = await _materiaService.UpdateAsync(id, dto);

        if (materia == null)
        {
            return NotFound(
                "La materia no existe o la carrera/profesor indicado no es válido."
            );
        }

        return Ok(materia);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var eliminado = await _materiaService.DeleteAsync(id);

        if (!eliminado)
        {
            return NotFound();
        }

        return NoContent();
    }
}