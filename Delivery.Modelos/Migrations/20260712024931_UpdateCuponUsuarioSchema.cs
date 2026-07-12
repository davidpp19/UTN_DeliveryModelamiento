using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Delivery.Modelos.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCuponUsuarioSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_cupones_usuarios",
                table: "cupones_usuarios");

            migrationBuilder.RenameColumn(
                name: "fecha_registro",
                table: "cupones_usuarios",
                newName: "fecha_asignacion");

            migrationBuilder.AlterColumn<long>(
                name: "pedido_id",
                table: "cupones_usuarios",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "id",
                table: "cupones_usuarios",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<bool>(
                name: "activo",
                table: "cupones_usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_expiracion",
                table: "cupones_usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "usado",
                table: "cupones_usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_cupones_usuarios",
                table: "cupones_usuarios",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_cupones_usuarios_cupon_id",
                table: "cupones_usuarios",
                column: "cupon_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_cupones_usuarios",
                table: "cupones_usuarios");

            migrationBuilder.DropIndex(
                name: "IX_cupones_usuarios_cupon_id",
                table: "cupones_usuarios");

            migrationBuilder.DropColumn(
                name: "id",
                table: "cupones_usuarios");

            migrationBuilder.DropColumn(
                name: "activo",
                table: "cupones_usuarios");

            migrationBuilder.DropColumn(
                name: "fecha_expiracion",
                table: "cupones_usuarios");

            migrationBuilder.DropColumn(
                name: "usado",
                table: "cupones_usuarios");

            migrationBuilder.RenameColumn(
                name: "fecha_asignacion",
                table: "cupones_usuarios",
                newName: "fecha_registro");

            migrationBuilder.AlterColumn<long>(
                name: "pedido_id",
                table: "cupones_usuarios",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_cupones_usuarios",
                table: "cupones_usuarios",
                columns: new[] { "cupon_id", "usuario_id", "pedido_id" });
        }
    }
}
