using backend.Security;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController, Route("api/[controller]"), Authorize, ScreenPermission(ScreenKeys.EstadoAsistencia)]
public class EstadoAsistenciaController : ControllerBase
{
    private readonly EstadoAsistenciaService _service; public EstadoAsistenciaController(EstadoAsistenciaService service)=>_service=service;
    [HttpGet("alumnos")] public async Task<IActionResult> Alumnos()=>Ok(await _service.GetAlumnosAsync());
    [HttpGet("alumno/{alumnoId:guid}")] public async Task<IActionResult> Alumno(Guid alumnoId){var x=await _service.GetAsync(alumnoId);return x==null?NotFound():Ok(x);}
}
