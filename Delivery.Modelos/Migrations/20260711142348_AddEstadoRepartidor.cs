using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery.Modelos.Migrations
{
    /// <inheritdoc />
    public partial class AddEstadoRepartidor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "disponible",
                table: "repartidores");

            migrationBuilder.AddColumn<int>(
                name: "estado",
                table: "repartidores",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estado",
                table: "repartidores");

            migrationBuilder.AddColumn<bool>(
                name: "disponible",
                table: "repartidores",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
