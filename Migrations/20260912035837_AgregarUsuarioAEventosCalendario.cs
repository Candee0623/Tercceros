using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AgregarUsuarioAEventosCalendario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "EventosCalendario",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_EventosCalendario_UsuarioId",
                table: "EventosCalendario",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventosCalendario_Usuarios_UsuarioId",
                table: "EventosCalendario",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventosCalendario_Usuarios_UsuarioId",
                table: "EventosCalendario");

            migrationBuilder.DropIndex(
                name: "IX_EventosCalendario_UsuarioId",
                table: "EventosCalendario");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "EventosCalendario");
        }
    }
}
