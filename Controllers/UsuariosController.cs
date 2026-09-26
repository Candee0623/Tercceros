using System.Security.Claims;
using backend.DTOs.Usuarios;
using backend.Security;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _service; 
    
    public UsuariosController(UsuarioService service) 
    { 
        _service = service; 
    }

    [HttpGet] 
    public async Task<ActionResult<List<UsuarioDto>>> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("crear-emergencia")]
    public async Task<IActionResult> CrearEmergencia() 
    {
        try 
        {
            var dtoCheck = new CreateUsuarioDto 
            {
                NombreUsuario = "cande",
                Nombre = "Candela",
                Apellido = "Garcete",
                Password = "123",
                RolId = Guid.Parse("74ABF9CE-3BD7-4F61-AF86-57D5D99DF6E3"),
                AlumnoId = Guid.Parse("9B997450-BD61-46C6-A862-8BD9D74DBEE8"),
                Activo = true
            };
            
            var (usuarioCreado, error) = await _service.CreateAsync(dtoCheck);
            if (error != null)
            {
                return BadRequest(new { mensaje = $"No se pudo crear: {error}" });
            }
            
            return Ok(new { mensaje = "¡Usuario 'cande' creado con éxito con contraseña '123'!" });
        } 
        catch (Exception ex) 
        { 
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("roles")] 
    public async Task<ActionResult<List<UsuarioRolOptionDto>>> GetRoles() => Ok(await _service.GetRolesAsync());

    [HttpGet("alumnos")] 
    public async Task<ActionResult<List<UsuarioAlumnoOptionDto>>> GetAlumnos([FromQuery] Guid? incluir = null) => Ok(await _service.GetAlumnosAsync(incluir));

    [HttpGet("profesores")] 
    public async Task<ActionResult<List<UsuarioProfesorOptionDto>>> GetProfesores([FromQuery] Guid? incluir = null) => Ok(await _service.GetProfesoresAsync(incluir));

    [HttpPost] 
    public async Task<IActionResult> Create(CreateUsuarioDto dto) 
    { 
        var (u, e) = await _service.CreateAsync(dto); 
        return e == null ? Ok(u) : BadRequest(e); 
    }

    [HttpPut("{id:guid}")] 
    public async Task<IActionResult> Update(Guid id, UpdateUsuarioDto dto) 
    { 
        var current = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); 
        var (u, e) = await _service.UpdateAsync(id, dto, current); 
        return e == null ? Ok(u) : BadRequest(e); 
    }
}