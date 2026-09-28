using backend.DTOs.Asistencias; using backend.Security; using backend.Services; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc;
namespace backend.Controllers;
[ApiController][Route("api/[controller]")][Authorize][ScreenPermission(ScreenKeys.Asistencias)] public class AsistenciasController:ControllerBase
{private readonly AsistenciaService _s;public AsistenciasController(AsistenciaService s)=>_s=s;[HttpGet("planilla")]public async Task<IActionResult> Get([FromQuery]Guid cursadaId,[FromQuery]DateTime fecha){var(x,e)=await _s.GetPlanillaAsync(cursadaId,fecha);return e==null?Ok(x):BadRequest(e);}[HttpPost]public async Task<IActionResult> Save(GuardarAsistenciaDto dto){var(x,e)=await _s.GuardarAsync(dto);return e==null?Ok(x):BadRequest(e);}}
