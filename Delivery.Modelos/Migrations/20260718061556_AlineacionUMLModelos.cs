using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery.Modelos.Migrations
{
    /// <inheritdoc />
    public partial class AlineacionUMLModelos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "branch",
                table: "usuarios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "salary",
                table: "usuarios",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "comission",
                table: "repartidores",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "stock",
                table: "productos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "branch",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "salary",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "comission",
                table: "repartidores");

            migrationBuilder.DropColumn(
                name: "stock",
                table: "productos");
        }
    }
}
