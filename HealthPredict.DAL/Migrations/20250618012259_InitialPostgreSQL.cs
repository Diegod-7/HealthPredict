using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HealthPredict.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USUARIOS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NOMBRE = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    APELLIDO = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PASSWORD = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FECHA_NACIMIENTO = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GENERO = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ALTURA = table.Column<decimal>(type: "numeric", nullable: false),
                    PESO = table.Column<decimal>(type: "numeric", nullable: false),
                    FECHA_REGISTRO = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ULTIMO_ACCESO = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ES_PROFESIONAL_MEDICO = table.Column<bool>(type: "boolean", nullable: false),
                    ESPECIALIDAD = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NUMERO_LICENCIA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIOS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ALERTAS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    USUARIO_ID = table.Column<int>(type: "integer", nullable: false),
                    FECHA_CREACION = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TIPO_ALERTA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DESCRIPCION = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SEVERIDAD = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LEIDA = table.Column<bool>(type: "boolean", nullable: false),
                    FECHA_LECTURA = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RESUELTA = table.Column<bool>(type: "boolean", nullable: false),
                    FECHA_RESOLUCION = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NOTAS_RESOLUCION = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALERTAS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ALERTAS_USUARIOS",
                        column: x => x.USUARIO_ID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DATOS_VITALES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    USUARIO_ID = table.Column<int>(type: "integer", nullable: false),
                    FECHA_REGISTRO = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TIPO_DATO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    VALOR = table.Column<decimal>(type: "numeric", nullable: false),
                    UNIDAD = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DISPOSITIVO_ORIGEN = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NOTAS = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DATOS_VITALES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DATOS_VITALES_USUARIOS",
                        column: x => x.USUARIO_ID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ALERTAS_USUARIO_ID",
                table: "ALERTAS",
                column: "USUARIO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DATOS_VITALES_USUARIO_ID",
                table: "DATOS_VITALES",
                column: "USUARIO_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ALERTAS");

            migrationBuilder.DropTable(
                name: "DATOS_VITALES");

            migrationBuilder.DropTable(
                name: "USUARIOS");
        }
    }
}
