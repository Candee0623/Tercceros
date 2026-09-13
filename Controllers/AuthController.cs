using backend.DTOs.Auth;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;
[ApiController, Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;
    public AuthController(AuthService service) { _service = service; }
    [AllowAnonymous, HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var result = await _service.LoginAsync(request);
        return result == null ? Unauthorized("Usuario o contraseña incorrectos.") : Ok(result);
    }
}
