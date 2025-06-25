using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthPredict.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSistemaRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CARGO",
                table: "USUARIOS",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DEPARTAMENTO",
                table: "USUARIOS",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ES_ACTIVO",
                table: "USUARIOS",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "JEFE_ID",
                table: "USUARIOS",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ROL",
                table: "USUARIOS",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Trabajador");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIOS_JEFE_ID",
                table: "USUARIOS",
                column: "JEFE_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_USUARIOS_JEFE",
                table: "USUARIOS",
                column: "JEFE_ID",
                principalTable: "USUARIOS",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_USUARIOS_JEFE",
                table: "USUARIOS");

            migrationBuilder.DropIndex(
                name: "IX_USUARIOS_JEFE_ID",
                table: "USUARIOS");

            migrationBuilder.DropColumn(
                name: "CARGO",
                table: "USUARIOS");

            migrationBuilder.DropColumn(
                name: "DEPARTAMENTO",
                table: "USUARIOS");

            migrationBuilder.DropColumn(
                name: "ES_ACTIVO",
                table: "USUARIOS");

            migrationBuilder.DropColumn(
                name: "JEFE_ID",
                table: "USUARIOS");

            migrationBuilder.DropColumn(
                name: "ROL",
                table: "USUARIOS");
        }
    }
}
