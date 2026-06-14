using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SePrise.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixTurnoAtencionRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Atenciones_Turnos_TurnoIdTurno",
                table: "Atenciones");

            migrationBuilder.DropIndex(
                name: "IX_Atenciones_TurnoIdTurno",
                table: "Atenciones");

            migrationBuilder.DropColumn(
                name: "TurnoIdTurno",
                table: "Atenciones");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TurnoIdTurno",
                table: "Atenciones",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Atenciones_TurnoIdTurno",
                table: "Atenciones",
                column: "TurnoIdTurno");

            migrationBuilder.AddForeignKey(
                name: "FK_Atenciones_Turnos_TurnoIdTurno",
                table: "Atenciones",
                column: "TurnoIdTurno",
                principalTable: "Turnos",
                principalColumn: "IdTurno");
        }
    }
}
