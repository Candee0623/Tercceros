using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProfesorAUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProfesorId",
                table: "Usuarios",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_ProfesorId",
                table: "Usuarios",
                column: "ProfesorId",
                unique: true,
                filter: "[ProfesorId] IS NOT NULL");

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

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_ProfesorId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ProfesorId",
                table: "Usuarios");
        }
    }
}
