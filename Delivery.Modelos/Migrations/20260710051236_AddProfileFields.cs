using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery.Modelos.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_nacimiento",
                table: "usuarios",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "informacion_adicional",
                table: "usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "portada_url",
                table: "restaurantes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "redes_sociales",
                table: "restaurantes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "datos_adicionales",
                table: "repartidores",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "direccion",
                table: "repartidores",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fecha_nacimiento",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "informacion_adicional",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "portada_url",
                table: "restaurantes");

            migrationBuilder.DropColumn(
                name: "redes_sociales",
                table: "restaurantes");

            migrationBuilder.DropColumn(
                name: "datos_adicionales",
                table: "repartidores");

            migrationBuilder.DropColumn(
                name: "direccion",
                table: "repartidores");
        }
    }
}
