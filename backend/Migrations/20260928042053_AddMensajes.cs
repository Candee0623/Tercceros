using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMensajes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProfesorId",
                table: "Usuarios",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Mensajes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Asunto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRemitenteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoDestinatario = table.Column<int>(type: "int", nullable: false),
                    CarreraId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Anio = table.Column<int>(type: "int", nullable: true),
                    CursadaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mensajes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mensajes_Carreras_CarreraId",
                        column: x => x.CarreraId,
                        principalTable: "Carreras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mensajes_Cursadas_CursadaId",
                        column: x => x.CursadaId,
                        principalTable: "Cursadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mensajes_Usuarios_UsuarioRemitenteId",
                        column: x => x.UsuarioRemitenteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MensajesDestinatarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MensajeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlumnoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Leido = table.Column<bool>(type: "bit", nullable: false),
                    FechaLectura = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajesDestinatarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensajesDestinatarios_Alumnos_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "Alumnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MensajesDestinatarios_Mensajes_MensajeId",
                        column: x => x.MensajeId,
                        principalTable: "Mensajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_ProfesorId",
                table: "Usuarios",
                column: "ProfesorId",
                unique: true,
                filter: "[ProfesorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_CarreraId",
                table: "Mensajes",
                column: "CarreraId");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_CursadaId",
                table: "Mensajes",
                column: "CursadaId");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_UsuarioRemitenteId",
                table: "Mensajes",
                column: "UsuarioRemitenteId");

            migrationBuilder.CreateIndex(
                name: "IX_MensajesDestinatarios_AlumnoId",
                table: "MensajesDestinatarios",
                column: "AlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_MensajesDestinatarios_MensajeId_AlumnoId",
                table: "MensajesDestinatarios",
                columns: new[] { "MensajeId", "AlumnoId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Profesores_ProfesorId",
                table: "Usuarios",
                column: "ProfesorId",
                principalTable: "Profesores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Profesores_ProfesorId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "MensajesDestinatarios");

            migrationBuilder.DropTable(
                name: "Mensajes");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_ProfesorId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ProfesorId",
                table: "Usuarios");
        }
    }
}
