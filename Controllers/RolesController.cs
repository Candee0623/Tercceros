using backend.DTOs.Roles;
using backend.Security;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;
[ApiController, Route("api/[controller]"), Authorize, ScreenPermission(ScreenKeys.Roles)]
public class RolesController : ControllerBase
{
    private readonly RolService _service; public RolesController(RolService service) { _service = service; }
    [HttpGet] public async Task<ActionResult<List<RolDto>>> GetAll() => Ok(await _service.GetAllAsync());
    [HttpGet("pantallas")] public ActionResult<List<PantallaDto>> Pantallas() => Ok(_service.GetPantallas());
    [HttpPost] public async Task<IActionResult> Create(SaveRolDto dto) { var (r,e)=await _service.CreateAsync(dto); return e==null?Ok(r):BadRequest(e); }
    [HttpPut("{id:guid}")] public async Task<IActionResult> Update(Guid id, SaveRolDto dto) { var (r,e)=await _service.UpdateAsync(id,dto); return e==null?Ok(r):BadRequest(e); }
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id) { var e=await _service.DeleteAsync(id); return e==null?NoContent():BadRequest(e); }
}
