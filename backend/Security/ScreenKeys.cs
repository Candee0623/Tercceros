namespace backend.Security;

public static class ScreenKeys
{
    public const string Alumnos = "alumnos";
    public const string Carreras = "carreras";
    public const string Profesores = "profesores";
    public const string Materias = "materias";
    public const string Usuarios = "usuarios";
    public const string Roles = "roles";
    public const string Cursadas = "cursadas";
    public const string Matriculaciones = "matriculaciones";
    public const string PlanEstudios = "plan_estudios";
    public const string Asistencias = "asistencias";
    public const string EstadoAsistencia = "estado_asistencia";
    public const string Calendario = "calendario";

    // Mensajería
    public const string Mensajes = "mensajes";

    // Portal/autogestión del alumno
    public const string MisDatos = "mis_datos";
    public const string MiAsistencia = "mi_asistencia";
    public const string AutoMatriculacion = "auto_matriculacion";
    public const string Calificaciones = "calificaciones";
    public const string Cuotas = "cuotas";

    // Nuevas funcionalidades: Avisos / Cartelera y Trámites
    public const string Anuncios = "anuncios";
    public const string Tramites = "tramites";

    public static readonly IReadOnlyDictionary<string, string> Todas =
        new Dictionary<string, string>
        {
            [Alumnos] = "Alumnos",
            [Carreras] = "Carreras",
            [Profesores] = "Profesores",
            [Materias] = "Materias",
            [Usuarios] = "Usuarios",
            [Roles] = "Roles",
            [Cursadas] = "Cursadas",
            [Matriculaciones] = "Matriculaciones",
            [PlanEstudios] = "Plan de estudios",
            [Asistencias] = "Asistencias",
            [EstadoAsistencia] = "Estado de asistencia (administración)",
            [Calendario] = "Calendario",

            [Mensajes] = "Mensajes",
            [Anuncios] = "Avisos y Cartelera",
            [Tramites] = "Trámites y Certificados",

            [MisDatos] = "Mis datos (alumno)",
            [MiAsistencia] = "Mi asistencia (alumno)",
            [AutoMatriculacion] = "Inscripción a materias (alumno)",
            [Calificaciones] = "Calificaciones",
            [Cuotas] = "Cuotas / Estado de cuenta"
        };

    public static readonly string[] PortalAlumno =
    {
        MisDatos,
        MiAsistencia,
        AutoMatriculacion,
        Mensajes,
        Anuncios,
        Tramites
    };
}