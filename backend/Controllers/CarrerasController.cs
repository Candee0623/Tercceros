using backend.DTOs.Carreras;
using backend.Services;
using backend.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ScreenPermission(ScreenKeys.Carreras)]
public class CarrerasController : ControllerBase
{
    private readonly CarreraService _carreraService;

    public CarrerasController(CarreraService carreraService)
    {
        _carreraService = carreraService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CarreraDto>>> GetAll()
    {
        var carreras = await _carreraService.GetAllAsync();

        return Ok(carreras);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CarreraDto>> GetById(Guid id)
    {
        var carrera = await _carreraService.GetByIdAsync(id);

        if (carrera == null)
        {
            return NotFound();
        }

        return Ok(carrera);
    }

    [HttpPost]
    public async Task<ActionResult<CarreraDto>> Create(
        CreateCarreraDto dto)
    {
        var carrera = await _carreraService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = carrera.Id },
            carrera
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CarreraDto>> Update(
        Guid id,
        UpdateCarreraDto dto)
    {
        var carrera = await _carreraService.UpdateAsync(id, dto);

        if (carrera == null)
        {
            return NotFound();
        }

        return Ok(carrera);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var eliminado = await _carreraService.DeleteAsync(id);

        if (!eliminado)
        {
            return NotFound();
        }

        return NoContent();
    }
}