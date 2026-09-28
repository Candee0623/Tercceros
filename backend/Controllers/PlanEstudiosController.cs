using backend.DTOs.PlanEstudios;
using backend.Security;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ScreenPermission(ScreenKeys.PlanEstudios)]
public class PlanEstudiosController : ControllerBase
{
    private readonly PlanEstudioService _service;

    public PlanEstudiosController(PlanEstudioService service)
    {
        _service = service;
    }

    // GET api/PlanEstudios
    [HttpGet]
    public async Task<ActionResult<List<PlanEstudioCarreraDto>>> GetAll()
    {
        var plan = await _service.GetPlanEstudiosAsync();
        return Ok(plan);
    }
}
