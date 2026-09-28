using backend.Data;
using backend.DTOs.Mensajes;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class MensajeService
{
private readonly AppDbContext _context;

public MensajeService(AppDbContext context)
{
    _context = context;
}

public async Task<(MensajeEnviadoDto? Data, string? Error)> EnviarAsync(
    Guid usuarioId,
    CrearMensajeDto dto)
{
    var usuario = await _context.Usuarios
        .Include(x => x.Rol)
        .FirstOrDefaultAsync(x =>
            x.Id == usuarioId &&
            x.Activo);

    if (usuario == null)
        return (null, "El usuario no existe o está inactivo.");

    if (!Enum.IsDefined(typeof(TipoDestinatarioMensaje), dto.TipoDestinatario))
        return (null, "El tipo de destinatario no es válido.");

    if (string.IsNullOrWhiteSpace(dto.Asunto))
        return (null, "El asunto es obligatorio.");

    if (string.IsNullOrWhiteSpace(dto.Contenido))
        return (null, "El contenido es obligatorio.");

    var validacion = await ValidarDestinatarioAsync(usuario, dto);

    if (validacion != null)
        return (null, validacion);

    var alumnoIds = await ObtenerAlumnoIdsAsync(dto);

    if (alumnoIds.Count == 0)
        return (null, "No se encontraron alumnos destinatarios.");

    var mensaje = new Mensaje
    {
        Id = Guid.NewGuid(),
        Asunto = dto.Asunto.Trim(),
        Contenido = dto.Contenido.Trim(),
        FechaEnvio = DateTime.UtcNow,
        UsuarioRemitenteId = usuarioId,
        TipoDestinatario = dto.TipoDestinatario,
        CarreraId = dto.CarreraId,
        Anio = dto.Anio,
        CursadaId = dto.CursadaId
    };

    foreach (var alumnoId in alumnoIds.Distinct())
    {
        mensaje.Destinatarios.Add(new MensajeDestinatario
        {
            Id = Guid.NewGuid(),
            AlumnoId = alumnoId,
            Leido = false
        });
    }

    _context.Mensajes.Add(mensaje);

    await _context.SaveChangesAsync();

    return (
        await ObtenerEnviadoAsync(mensaje.Id),
        null
    );
}

public async Task<List<MensajeBandejaDto>> ObtenerBandejaAlumnoAsync(
    Guid alumnoId)
{
    return await _context.MensajesDestinatarios
        .AsNoTracking()
        .Where(x => x.AlumnoId == alumnoId)
        .OrderByDescending(x => x.Mensaje.FechaEnvio)
        .Select(x => new MensajeBandejaDto
        {
            Id = x.MensajeId,
            Asunto = x.Mensaje.Asunto,
            Contenido = x.Mensaje.Contenido,
            FechaEnvio = x.Mensaje.FechaEnvio,
            Remitente =
                x.Mensaje.UsuarioRemitente.Nombre
                + " "
                + x.Mensaje.UsuarioRemitente.Apellido,
            Leido = x.Leido
        })
        .ToListAsync();
}

public async Task<int> ContarNoLeidosAsync(Guid alumnoId)
{
    return await _context.MensajesDestinatarios
        .CountAsync(x =>
            x.AlumnoId == alumnoId &&
            !x.Leido);
}

public async Task<bool> MarcarLeidoAsync(
    Guid alumnoId,
    Guid mensajeId)
{
    var destinatario =
        await _context.MensajesDestinatarios
            .FirstOrDefaultAsync(x =>
                x.AlumnoId == alumnoId &&
                x.MensajeId == mensajeId);

    if (destinatario == null)
        return false;

    if (!destinatario.Leido)
    {
        destinatario.Leido = true;
        destinatario.FechaLectura = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    return true;
}

public async Task<List<MensajeEnviadoDto>> ObtenerEnviadosAsync(
    Guid usuarioId)
{
    return await _context.Mensajes
        .AsNoTracking()
        .Where(x => x.UsuarioRemitenteId == usuarioId)
        .OrderByDescending(x => x.FechaEnvio)
        .Select(x => new MensajeEnviadoDto
        {
            Id = x.Id,
            Asunto = x.Asunto,
            Contenido = x.Contenido,
            FechaEnvio = x.FechaEnvio,
            TipoDestinatario = x.TipoDestinatario.ToString(),
            DestinatarioDescripcion =
                x.TipoDestinatario == TipoDestinatarioMensaje.Todos
                    ? "Todos los alumnos"
                    : x.TipoDestinatario == TipoDestinatarioMensaje.Carrera
                        ? x.Carrera == null
                            ? "Carrera"
                            : x.Carrera.Nombre
                        : x.TipoDestinatario == TipoDestinatarioMensaje.CarreraAnio
                            ? x.Carrera == null
                                ? $"Carrera - Año {x.Anio}"
                                : $"{x.Carrera.Nombre} - Año {x.Anio}"
                            : x.Cursada == null
                                ? "Cursada"
                                : x.Cursada.Materia.Nombre,
            CantidadDestinatarios = x.Destinatarios.Count
        })
        .ToListAsync();
}

public async Task<MensajeDetalleDto?> ObtenerDetalleAlumnoAsync(
    Guid alumnoId,
    Guid mensajeId)
{
    return await _context.MensajesDestinatarios
        .AsNoTracking()
        .Where(x =>
            x.AlumnoId == alumnoId &&
            x.MensajeId == mensajeId)
        .Select(x => new MensajeDetalleDto
        {
            Id = x.MensajeId,
            Asunto = x.Mensaje.Asunto,
            Contenido = x.Mensaje.Contenido,
            FechaEnvio = x.Mensaje.FechaEnvio,
            Remitente =
                x.Mensaje.UsuarioRemitente.Nombre
                + " "
                + x.Mensaje.UsuarioRemitente.Apellido,
            Leido = x.Leido
        })
        .FirstOrDefaultAsync();
}

public async Task<List<MensajeCursadaDto>> ObtenerCursadasProfesorAsync(
    Guid profesorId)
{
    return await _context.Cursadas
        .AsNoTracking()
        .Where(x =>
            x.ProfesorId == profesorId &&
            x.Activa)
        .OrderByDescending(x => x.CicloLectivo)
        .ThenBy(x => x.Materia.Nombre)
        .Select(x => new MensajeCursadaDto
        {
            Id = x.Id,
            MateriaNombre = x.Materia.Nombre,
            CicloLectivo = x.CicloLectivo,
            Periodo = x.Periodo
        })
        .ToListAsync();
}

public async Task<List<MensajeCursadaDto>> ObtenerCursadasDisponiblesAsync()
{
    return await _context.Cursadas
        .AsNoTracking()
        .Where(x => x.Activa)
        .OrderByDescending(x => x.CicloLectivo)
        .ThenBy(x => x.Materia.Nombre)
        .Select(x => new MensajeCursadaDto
        {
            Id = x.Id,
            MateriaNombre = x.Materia.Nombre,
            CicloLectivo = x.CicloLectivo,
            Periodo = x.Periodo
        })
        .ToListAsync();
}

public async Task<bool> UsuarioEsSistemaAsync(Guid usuarioId)
{
    return await _context.Usuarios
        .AsNoTracking()
        .Where(x =>
            x.Id == usuarioId &&
            x.Activo)
        .Select(x => x.Rol.EsSistema)
        .FirstOrDefaultAsync();
}

private async Task<string?> ValidarDestinatarioAsync(
    Usuario usuario,
    CrearMensajeDto dto)
{
    var esSistema = usuario.Rol.EsSistema;

    var esProfesor =
        usuario.ProfesorId.HasValue &&
        usuario.Rol.Nombre.Equals(
            "PROFESOR",
            StringComparison.OrdinalIgnoreCase);

    if (esProfesor)
    {
        if (dto.TipoDestinatario != TipoDestinatarioMensaje.Cursada)
            return "Los profesores solamente pueden enviar mensajes a sus propias cursadas.";

        if (!usuario.ProfesorId.HasValue)
            return "El usuario profesor no tiene un profesor asociado.";

        if (!dto.CursadaId.HasValue)
            return "Debés seleccionar una cursada.";

        var cursadaId = dto.CursadaId.Value;
        var profesorId = usuario.ProfesorId.Value;

        var cursadaProfesor =
            await _context.Cursadas.AnyAsync(x =>
                x.Id == cursadaId &&
                x.ProfesorId == profesorId &&
                x.Activa);

        if (!cursadaProfesor)
            return "La cursada seleccionada no pertenece al profesor.";
    }

    if (!esProfesor && !esSistema)
        return "No tenés autorización para enviar mensajes.";

    switch (dto.TipoDestinatario)
    {
        case TipoDestinatarioMensaje.Todos:
            if (
                dto.CarreraId.HasValue ||
                dto.Anio.HasValue ||
                dto.CursadaId.HasValue)
            {
                return "Para enviar a todos no debés indicar carrera, año ni cursada.";
            }

            break;

        case TipoDestinatarioMensaje.Carrera:
            if (!dto.CarreraId.HasValue)
                return "Debés seleccionar una carrera.";

            var carreraId = dto.CarreraId.Value;

            if (!await _context.Carreras.AnyAsync(x =>
                    x.Id == carreraId &&
                    x.Activa))
            {
                return "La carrera seleccionada no existe o está inactiva.";
            }

            if (
                dto.Anio.HasValue ||
                dto.CursadaId.HasValue)
            {
                return "Para enviar a una carrera no debés indicar año ni cursada.";
            }

            break;

        case TipoDestinatarioMensaje.CarreraAnio:
            if (!dto.CarreraId.HasValue)
                return "Debés seleccionar una carrera.";

            if (!dto.Anio.HasValue)
                return "Debés indicar el año.";

            var carreraIdAnio = dto.CarreraId.Value;
            var anio = dto.Anio.Value;

            if (!await _context.Carreras.AnyAsync(x =>
                    x.Id == carreraIdAnio &&
                    x.Activa))
            {
                return "La carrera seleccionada no existe o está inactiva.";
            }

            if (anio <= 0)
                return "El año indicado no es válido.";

            if (dto.CursadaId.HasValue)
                return "Para carrera y año no debés indicar una cursada.";

            break;

        case TipoDestinatarioMensaje.Cursada:
            if (!dto.CursadaId.HasValue)
                return "Debés seleccionar una cursada.";

            var cursadaIdValidar = dto.CursadaId.Value;

            var cursada =
                await _context.Cursadas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id == cursadaIdValidar &&
                        x.Activa);

            if (cursada == null)
                return "La cursada seleccionada no existe o está inactiva.";

            if (
                dto.CarreraId.HasValue ||
                dto.Anio.HasValue)
            {
                return "Para enviar a una cursada no debés indicar carrera ni año.";
            }

            break;

        default:
            return "El tipo de destinatario no es válido.";
    }

    return null;
}

private async Task<List<Guid>> ObtenerAlumnoIdsAsync(
    CrearMensajeDto dto)
{
    IQueryable<Matricula> matriculas =
        _context.Matriculas
            .AsNoTracking()
            .Where(x =>
                x.Activa &&
                x.Alumno.Activo &&
                x.Cursada.Activa);

    switch (dto.TipoDestinatario)
    {
        case TipoDestinatarioMensaje.Todos:
            return await matriculas
                .Select(x => x.AlumnoId)
                .Distinct()
                .ToListAsync();

        case TipoDestinatarioMensaje.Carrera:
            if (!dto.CarreraId.HasValue)
                return new List<Guid>();

            var carreraId = dto.CarreraId.Value;

            return await matriculas
                .Where(x =>
                    x.Cursada.Materia.CarreraId ==
                    carreraId)
                .Select(x => x.AlumnoId)
                .Distinct()
                .ToListAsync();

        case TipoDestinatarioMensaje.CarreraAnio:
            if (
                !dto.CarreraId.HasValue ||
                !dto.Anio.HasValue)
            {
                return new List<Guid>();
            }

            var carreraIdAnio = dto.CarreraId.Value;
            var anio = dto.Anio.Value;

            return await matriculas
                .Where(x =>
                    x.Cursada.Materia.CarreraId ==
                        carreraIdAnio &&
                    x.Cursada.Materia.Anio ==
                        anio)
                .Select(x => x.AlumnoId)
                .Distinct()
                .ToListAsync();

        case TipoDestinatarioMensaje.Cursada:
            if (!dto.CursadaId.HasValue)
                return new List<Guid>();

            var cursadaId = dto.CursadaId.Value;

            return await matriculas
                .Where(x =>
                    x.CursadaId == cursadaId)
                .Select(x => x.AlumnoId)
                .Distinct()
                .ToListAsync();

        default:
            return new List<Guid>();
    }
}

private async Task<MensajeEnviadoDto?> ObtenerEnviadoAsync(
    Guid mensajeId)
{
    return await _context.Mensajes
        .AsNoTracking()
        .Where(x => x.Id == mensajeId)
        .Select(x => new MensajeEnviadoDto
        {
            Id = x.Id,
            Asunto = x.Asunto,
            Contenido = x.Contenido,
            FechaEnvio = x.FechaEnvio,
            TipoDestinatario = x.TipoDestinatario.ToString(),
            DestinatarioDescripcion =
                x.TipoDestinatario == TipoDestinatarioMensaje.Todos
                    ? "Todos los alumnos"
                    : x.TipoDestinatario == TipoDestinatarioMensaje.Carrera
                        ? x.Carrera == null
                            ? "Carrera"
                            : x.Carrera.Nombre
                        : x.TipoDestinatario == TipoDestinatarioMensaje.CarreraAnio
                            ? x.Carrera == null
                                ? $"Carrera - Año {x.Anio}"
                                : $"{x.Carrera.Nombre} - Año {x.Anio}"
                            : x.Cursada == null
                                ? "Cursada"
                                : x.Cursada.Materia.Nombre,
            CantidadDestinatarios = x.Destinatarios.Count
        })
        .FirstOrDefaultAsync();
}

public async Task<Guid?> ObtenerProfesorIdUsuarioAsync(
    Guid usuarioId)
{
    return await _context.Usuarios
        .AsNoTracking()
        .Where(x =>
            x.Id == usuarioId &&
            x.Activo)
        .Select(x => x.ProfesorId)
        .FirstOrDefaultAsync();
}

public async Task<Guid?> ObtenerAlumnoIdUsuarioAsync(
    Guid usuarioId)
{
    return await _context.Usuarios
        .AsNoTracking()
        .Where(x =>
            x.Id == usuarioId &&
            x.Activo)
        .Select(x => x.AlumnoId)
        .FirstOrDefaultAsync();
}

}
