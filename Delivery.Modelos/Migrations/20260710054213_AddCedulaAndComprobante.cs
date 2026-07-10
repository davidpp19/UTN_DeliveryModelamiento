using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery.Modelos.Migrations
{
    /// <inheritdoc />
    public partial class AddCedulaAndComprobante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cedula",
                table: "usuarios",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "comprobante_transferencia_url",
                table: "pedidos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cedula",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "comprobante_transferencia_url",
                table: "pedidos");
        }
    }
}
