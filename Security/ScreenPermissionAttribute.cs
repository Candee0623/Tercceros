using System.Security.Claims;
using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace backend.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class ScreenPermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _screen;

    public ScreenPermissionAttribute(string screen)
    {
        _screen = screen;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var userIdText = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdText, out var userId))
        {
            context.Result = new ForbidResult();
            return;
        }

        var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        var autorizado = await db.Usuarios
            .Where(u => u.Id == userId && u.Activo && u.Rol.Activo)
            .AnyAsync(u => u.Rol.EsSistema || u.Rol.Permisos.Any(p => p.Pantalla == _screen));

        if (!autorizado)
            context.Result = new ForbidResult();
    }
}
