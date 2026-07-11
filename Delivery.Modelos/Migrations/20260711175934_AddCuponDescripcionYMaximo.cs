using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery.Modelos.Migrations
{
    /// <inheritdoc />
    public partial class AddCuponDescripcionYMaximo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "descripcion",
                table: "cupones",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "descuento_maximo",
                table: "cupones",
                type: "numeric(10,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "descripcion",
                table: "cupones");

            migrationBuilder.DropColumn(
                name: "descuento_maximo",
                table: "cupones");
        }
    }
}
