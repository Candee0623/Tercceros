using backend.Entities;
using backend.Security;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public static class DbSeeder
{
    // Credenciales del primer usuario. Cambiá la contraseña desde la
    // pantalla de Usuarios apenas entres por primera vez.
    private const string UsuarioInicial = "admin";
    private const string PasswordInicial = "Admin123!";

    public static async Task SeedAsync(AppDbContext context)
    {
        // Si ya hay algún usuario cargado, no tocamos nada: este seeder
        // es solo para destrabar el primer acceso a un sistema vacío.
        var yaHayUsuarios = await context.Usuarios.AnyAsync();
        if (yaHayUsuarios)
        {
            return;
        }

        var rolAdmin = await context.Roles
            .FirstOrDefaultAsync(r => r.Nombre == "ADMIN");

        if (rolAdmin == null)
        {
            rolAdmin = new Rol
            {
                Id = Guid.NewGuid(),
                Nombre = "ADMIN",
                Descripcion = "Rol de sistema con acceso total a todas las pantallas.",
                Activo = true,
                // EsSistema = true hace que AuthService le otorgue TODOS los
                // permisos automáticamente (incluidos los que se agreguen
                // a futuro, como "plan_estudios"), sin tener que asignarlos
                // uno por uno.
                EsSistema = true
            };

            context.Roles.Add(rolAdmin);
        }

        var admin = new Usuario
        {
            Id = Guid.NewGuid(),
            NombreUsuario = UsuarioInicial,
            Nombre = "Administrador",
            Apellido = "Sistema",
            PasswordHash = PasswordHasher.Hash(PasswordInicial),
            RolId = rolAdmin.Id,
            Activo = true
        };

        context.Usuarios.Add(admin);

        await context.SaveChangesAsync();
    }
}
