using System.Text;
using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddScoped<MateriaService>();
builder.Services.AddScoped<CarreraService>();
builder.Services.AddScoped<AlumnoService>();
builder.Services.AddScoped<ProfesorService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RolService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<CursadaService>();
builder.Services.AddScoped<MatriculaService>();
builder.Services.AddScoped<AsistenciaService>();
builder.Services.AddScoped<EstadoAsistenciaService>();
builder.Services.AddScoped<PortalAlumnoService>();
builder.Services.AddScoped<CalificacionService>();
builder.Services.AddScoped<CuotaService>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Debe configurarse Jwt:Key.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ClockSkew = TimeSpan.FromMinutes(1)
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("========== ERROR JWT ==========");
                Console.WriteLine(context.Exception.Message);
                Console.WriteLine("===============================");
                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                Console.WriteLine("========== JWT OK =============");
                Console.WriteLine("Usuario autenticado correctamente.");
                Console.WriteLine("===============================");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();