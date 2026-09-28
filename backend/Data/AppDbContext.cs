using backend.Entities;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Alumno> Alumnos { get; set; }
    public DbSet<Profesor> Profesores { get; set; }
    public DbSet<Materia> Materias { get; set; }
    public DbSet<Carrera> Carreras { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<RolPermiso> RolPermisos { get; set; }
    public DbSet<Cursada> Cursadas { get; set; }
    public DbSet<CursadaHorario> CursadaHorarios { get; set; }
    public DbSet<Matricula> Matriculas { get; set; }
    public DbSet<Clase> Clases { get; set; }
    public DbSet<AsistenciaDetalle> AsistenciaDetalles { get; set; }
    public DbSet<Evaluacion> Evaluaciones { get; set; }
    public DbSet<Calificacion> Calificaciones { get; set; }
    public DbSet<EstadoCuotaAlumno> EstadosCuotaAlumno { get; set; }
    public DbSet<EventoCalendario> EventosCalendario { get; set; }

    // Mensajes
    public DbSet<Mensaje> Mensajes { get; set; }
    public DbSet<MensajeDestinatario> MensajesDestinatarios { get; set; }

    // Avisos / Cartelera y Trámites
    public DbSet<Anuncio> Anuncios { get; set; }
    public DbSet<SolicitudTramite> SolicitudesTramite { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================
        // MATERIAS
        // =========================

        modelBuilder.Entity<Materia>()
            .Property(x => x.CantidadEvaluaciones)
            .HasDefaultValue(2);

        modelBuilder.Entity<Materia>()
            .Property(x => x.EsPromocionable)
            .HasDefaultValue(true);

        modelBuilder.Entity<Materia>()
            .Property(x => x.NotaPromocion)
            .HasDefaultValue(7m);

        modelBuilder.Entity<Materia>()
            .Property(x => x.NotaRegularizacion)
            .HasDefaultValue(4m);

        // =========================
        // USUARIO
        // =========================

        modelBuilder.Entity<Usuario>()
            .HasIndex(x => x.NombreUsuario)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasOne(x => x.Rol)
            .WithMany(x => x.Usuarios)
            .HasForeignKey(x => x.RolId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación Usuario -> Alumno
        modelBuilder.Entity<Usuario>()
            .HasIndex(x => x.AlumnoId)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasOne(x => x.Alumno)
            .WithMany()
            .HasForeignKey(x => x.AlumnoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación Usuario -> Profesor
        modelBuilder.Entity<Usuario>()
            .HasIndex(x => x.ProfesorId)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasOne(x => x.Profesor)
            .WithMany()
            .HasForeignKey(x => x.ProfesorId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // ROLES
        // =========================

        modelBuilder.Entity<Rol>()
            .HasIndex(x => x.Nombre)
            .IsUnique();

        modelBuilder.Entity<RolPermiso>()
            .HasKey(x => new { x.RolId, x.Pantalla });

        modelBuilder.Entity<RolPermiso>()
            .HasOne(x => x.Rol)
            .WithMany(x => x.Permisos)
            .HasForeignKey(x => x.RolId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // CURSADAS
        // =========================

        modelBuilder.Entity<CursadaHorario>()
            .HasOne(x => x.Cursada)
            .WithMany(x => x.Horarios)
            .HasForeignKey(x => x.CursadaId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // MATRICULAS
        // =========================

        modelBuilder.Entity<Matricula>()
            .HasIndex(x => new { x.AlumnoId, x.CursadaId })
            .IsUnique();

        modelBuilder.Entity<Matricula>()
            .HasOne(x => x.Cursada)
            .WithMany(x => x.Matriculas)
            .HasForeignKey(x => x.CursadaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Matricula>()
            .HasOne(x => x.Alumno)
            .WithMany()
            .HasForeignKey(x => x.AlumnoId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // CLASES
        // =========================

        modelBuilder.Entity<Clase>()
            .HasIndex(x => new { x.CursadaId, x.Fecha })
            .IsUnique();

        modelBuilder.Entity<Clase>()
            .HasOne(x => x.Cursada)
            .WithMany(x => x.Clases)
            .HasForeignKey(x => x.CursadaId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // ASISTENCIAS
        // =========================

        modelBuilder.Entity<AsistenciaDetalle>()
            .HasIndex(x => new { x.ClaseId, x.MatriculaId })
            .IsUnique();

        modelBuilder.Entity<AsistenciaDetalle>()
            .HasOne(x => x.Clase)
            .WithMany(x => x.Detalles)
            .HasForeignKey(x => x.ClaseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AsistenciaDetalle>()
            .HasOne(x => x.Matricula)
            .WithMany(x => x.Asistencias)
            .HasForeignKey(x => x.MatriculaId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // EVALUACIONES
        // =========================

        modelBuilder.Entity<Evaluacion>()
            .HasIndex(x => new { x.CursadaId, x.Numero })
            .IsUnique();

        modelBuilder.Entity<Evaluacion>()
            .HasOne(x => x.Cursada)
            .WithMany(x => x.Evaluaciones)
            .HasForeignKey(x => x.CursadaId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // CALIFICACIONES
        // =========================

        modelBuilder.Entity<Calificacion>()
            .HasIndex(x => new { x.EvaluacionId, x.MatriculaId })
            .IsUnique();

        modelBuilder.Entity<Calificacion>()
            .HasOne(x => x.Evaluacion)
            .WithMany(x => x.Calificaciones)
            .HasForeignKey(x => x.EvaluacionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Calificacion>()
            .HasOne(x => x.Matricula)
            .WithMany(x => x.Calificaciones)
            .HasForeignKey(x => x.MatriculaId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // CUOTAS
        // =========================

        modelBuilder.Entity<EstadoCuotaAlumno>()
            .HasIndex(x => x.AlumnoId)
            .IsUnique();

        modelBuilder.Entity<EstadoCuotaAlumno>()
            .HasOne(x => x.Alumno)
            .WithMany()
            .HasForeignKey(x => x.AlumnoId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // MENSAJES
        // =========================

        // Usuario que envía el mensaje
        modelBuilder.Entity<Mensaje>()
            .HasOne(x => x.UsuarioRemitente)
            .WithMany()
            .HasForeignKey(x => x.UsuarioRemitenteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Carrera utilizada como destinatario
        // para Carrera y Carrera + Año
        modelBuilder.Entity<Mensaje>()
            .HasOne(x => x.Carrera)
            .WithMany()
            .HasForeignKey(x => x.CarreraId)
            .OnDelete(DeleteBehavior.Restrict);

        // Cursada utilizada como destinatario
        modelBuilder.Entity<Mensaje>()
            .HasOne(x => x.Cursada)
            .WithMany()
            .HasForeignKey(x => x.CursadaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Un alumno no puede aparecer dos veces
        // como destinatario del mismo mensaje
        modelBuilder.Entity<MensajeDestinatario>()
            .HasIndex(x => new { x.MensajeId, x.AlumnoId })
            .IsUnique();

        // Mensaje -> Destinatarios
        modelBuilder.Entity<MensajeDestinatario>()
            .HasOne(x => x.Mensaje)
            .WithMany(x => x.Destinatarios)
            .HasForeignKey(x => x.MensajeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Alumno -> Destinatarios
        modelBuilder.Entity<MensajeDestinatario>()
            .HasOne(x => x.Alumno)
            .WithMany()
            .HasForeignKey(x => x.AlumnoId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // ANUNCIOS
        // =========================
        modelBuilder.Entity<Anuncio>()
            .HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // TRAMITES
        // =========================
        modelBuilder.Entity<SolicitudTramite>()
            .HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SolicitudTramite>()
            .HasOne(x => x.Alumno)
            .WithMany()
            .HasForeignKey(x => x.AlumnoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}