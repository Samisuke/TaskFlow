using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HistorialModificado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Historiales_Tareas_TareaId",
                table: "Historiales");

            migrationBuilder.DropForeignKey(
                name: "FK_Historiales_Usuarios_TareaId",
                table: "Historiales");

            migrationBuilder.RenameColumn(
                name: "TareaId",
                table: "Historiales",
                newName: "ProyectoId");

            migrationBuilder.RenameIndex(
                name: "IX_Historiales_TareaId",
                table: "Historiales",
                newName: "IX_Historiales_ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_Historiales_UsuarioId",
                table: "Historiales",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Historiales_Proyectos_ProyectoId",
                table: "Historiales",
                column: "ProyectoId",
                principalTable: "Proyectos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Historiales_Usuarios_UsuarioId",
                table: "Historiales",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Historiales_Proyectos_ProyectoId",
                table: "Historiales");

            migrationBuilder.DropForeignKey(
                name: "FK_Historiales_Usuarios_UsuarioId",
                table: "Historiales");

            migrationBuilder.DropIndex(
                name: "IX_Historiales_UsuarioId",
                table: "Historiales");

            migrationBuilder.RenameColumn(
                name: "ProyectoId",
                table: "Historiales",
                newName: "TareaId");

            migrationBuilder.RenameIndex(
                name: "IX_Historiales_ProyectoId",
                table: "Historiales",
                newName: "IX_Historiales_TareaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Historiales_Tareas_TareaId",
                table: "Historiales",
                column: "TareaId",
                principalTable: "Tareas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Historiales_Usuarios_TareaId",
                table: "Historiales",
                column: "TareaId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
