using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroMonitor.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Apellido = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Correo = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    ContraseñaHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Rol = table.Column<string>(type: "TEXT", nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Fincas",
                columns: table => new
                {
                    IdFinca = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdUsuario = table.Column<int>(type: "INTEGER", nullable: false),
                    NombreFinca = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    UbicacionText = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    Latitud = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    Longitud = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    AreaTareas = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fincas", x => x.IdFinca);
                    table.ForeignKey(
                        name: "FK_Fincas_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cultivos",
                columns: table => new
                {
                    IdCultivo = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdFinca = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoCultivo = table.Column<string>(type: "TEXT", nullable: false),
                    FechaSiembra = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaCosechaEstimada = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Estado = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cultivos", x => x.IdCultivo);
                    table.ForeignKey(
                        name: "FK_Cultivos_Fincas_IdFinca",
                        column: x => x.IdFinca,
                        principalTable: "Fincas",
                        principalColumn: "IdFinca",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reportes",
                columns: table => new
                {
                    IdReporte = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdUsuario = table.Column<int>(type: "INTEGER", nullable: false),
                    IdFinca = table.Column<int>(type: "INTEGER", nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Contenido = table.Column<string>(type: "TEXT", nullable: false),
                    FechaGenerado = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reportes", x => x.IdReporte);
                    table.ForeignKey(
                        name: "FK_Reportes_Fincas_IdFinca",
                        column: x => x.IdFinca,
                        principalTable: "Fincas",
                        principalColumn: "IdFinca",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reportes_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sensores",
                columns: table => new
                {
                    IdSensor = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdFinca = table.Column<int>(type: "INTEGER", nullable: false),
                    IdentificadorExterno = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    TipoSensor = table.Column<string>(type: "TEXT", nullable: false),
                    Modelo = table.Column<string>(type: "TEXT", maxLength: 40, nullable: true),
                    UbicacionEnFinca = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Estado = table.Column<string>(type: "TEXT", nullable: false),
                    InstaladoEn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UltimoCheck = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensores", x => x.IdSensor);
                    table.ForeignKey(
                        name: "FK_Sensores_Fincas_IdFinca",
                        column: x => x.IdFinca,
                        principalTable: "Fincas",
                        principalColumn: "IdFinca",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alertas",
                columns: table => new
                {
                    IdAlerta = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdCultivo = table.Column<int>(type: "INTEGER", nullable: false),
                    IdFinca = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoAlerta = table.Column<string>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    NivelPrioridad = table.Column<string>(type: "TEXT", nullable: false),
                    FechaGenerada = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alertas", x => x.IdAlerta);
                    table.ForeignKey(
                        name: "FK_Alertas_Cultivos_IdCultivo",
                        column: x => x.IdCultivo,
                        principalTable: "Cultivos",
                        principalColumn: "IdCultivo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alertas_Fincas_IdFinca",
                        column: x => x.IdFinca,
                        principalTable: "Fincas",
                        principalColumn: "IdFinca",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecomendacionesML",
                columns: table => new
                {
                    IdRec = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdCultivo = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoRecomendacion = table.Column<string>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Confianza = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    FechaGenerada = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecomendacionesML", x => x.IdRec);
                    table.ForeignKey(
                        name: "FK_RecomendacionesML_Cultivos_IdCultivo",
                        column: x => x.IdCultivo,
                        principalTable: "Cultivos",
                        principalColumn: "IdCultivo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LecturasSensor",
                columns: table => new
                {
                    IdLectura = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdSensor = table.Column<int>(type: "INTEGER", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(12,4)", nullable: false),
                    Unidad = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    FechaHora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProcesadoML = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LecturasSensor", x => x.IdLectura);
                    table.ForeignKey(
                        name: "FK_LecturasSensor_Sensores_IdSensor",
                        column: x => x.IdSensor,
                        principalTable: "Sensores",
                        principalColumn: "IdSensor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_IdCultivo",
                table: "Alertas",
                column: "IdCultivo");

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_IdFinca",
                table: "Alertas",
                column: "IdFinca");

            migrationBuilder.CreateIndex(
                name: "IX_Cultivos_IdFinca",
                table: "Cultivos",
                column: "IdFinca");

            migrationBuilder.CreateIndex(
                name: "IX_Fincas_IdUsuario",
                table: "Fincas",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_LecturasSensor_IdSensor",
                table: "LecturasSensor",
                column: "IdSensor");

            migrationBuilder.CreateIndex(
                name: "IX_RecomendacionesML_IdCultivo",
                table: "RecomendacionesML",
                column: "IdCultivo");

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_IdFinca",
                table: "Reportes",
                column: "IdFinca");

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_IdUsuario",
                table: "Reportes",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Sensores_IdFinca",
                table: "Sensores",
                column: "IdFinca");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alertas");

            migrationBuilder.DropTable(
                name: "LecturasSensor");

            migrationBuilder.DropTable(
                name: "RecomendacionesML");

            migrationBuilder.DropTable(
                name: "Reportes");

            migrationBuilder.DropTable(
                name: "Sensores");

            migrationBuilder.DropTable(
                name: "Cultivos");

            migrationBuilder.DropTable(
                name: "Fincas");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
