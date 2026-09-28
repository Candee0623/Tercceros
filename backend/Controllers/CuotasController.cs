using backend.DTOs.Cuotas;
using backend.Security;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ScreenPermission(ScreenKeys.Cuotas)]
public class CuotasController : ControllerBase
{
    private readonly CuotaService _service;
    public CuotasController(CuotaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPut("{alumnoId:guid}")]
    public async Task<IActionResult> Guardar(Guid alumnoId, GuardarEstadoCuotaDto dto)
    {
        var (data, error) = await _service.GuardarAsync(alumnoId, dto);
        return error == null ? Ok(data) : BadRequest(error);
    }
}
