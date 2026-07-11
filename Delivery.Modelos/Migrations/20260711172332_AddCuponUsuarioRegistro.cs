using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery.Modelos.Migrations
{
    /// <inheritdoc />
    public partial class AddCuponUsuarioRegistro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_uso",
                table: "cupones_usuarios",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_registro",
                table: "cupones_usuarios",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "es_publico",
                table: "cupones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "usuario_exclusivo_id",
                table: "cupones",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_cupones_usuario_exclusivo_id",
                table: "cupones",
                column: "usuario_exclusivo_id");

            migrationBuilder.AddForeignKey(
                name: "FK_cupones_usuarios_usuario_exclusivo_id",
                table: "cupones",
                column: "usuario_exclusivo_id",
                principalTable: "usuarios",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cupones_usuarios_usuario_exclusivo_id",
                table: "cupones");

            migrationBuilder.DropIndex(
                name: "IX_cupones_usuario_exclusivo_id",
                table: "cupones");

            migrationBuilder.DropColumn(
                name: "fecha_registro",
                table: "cupones_usuarios");

            migrationBuilder.DropColumn(
                name: "es_publico",
                table: "cupones");

            migrationBuilder.DropColumn(
                name: "usuario_exclusivo_id",
                table: "cupones");

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_uso",
                table: "cupones_usuarios",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
