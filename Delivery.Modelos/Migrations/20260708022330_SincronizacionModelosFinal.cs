using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Delivery.Modelos.Migrations
{
    /// <inheritdoc />
    public partial class SincronizacionModelosFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:estado_aprobacion_enum", "pendiente,aprobado,rechazado")
                .Annotation("Npgsql:Enum:estado_pago_enum", "pendiente,completado,fallido,reembolsado")
                .Annotation("Npgsql:Enum:estado_pedido_enum", "borrador,pendiente,aceptado,en_preparacion,en_camino,entregado,cancelado")
                .Annotation("Npgsql:Enum:estado_restaurante_enum", "pendiente,aprobado,rechazado,suspendido")
                .Annotation("Npgsql:Enum:tipo_accion_auditoria_enum", "crear,editar,eliminar,cambio_estado,inicio_sesion,cierre_sesion")
                .Annotation("Npgsql:Enum:tipo_descuento_enum", "porcentaje,monto_fijo")
                .Annotation("Npgsql:Enum:tipo_metodo_pago_enum", "efectivo,tarjeta,transferencia,billetera_digital")
                .Annotation("Npgsql:Enum:tipo_usuario_enum", "administrador,cliente,repartidor,restaurante")
                .Annotation("Npgsql:Enum:tipo_vehiculo_enum", "bicicleta,motocicleta,automovil")
                .OldAnnotation("Npgsql:Enum:estado_aprobacion_enum", "pendiente,aprobado,rechazado")
                .OldAnnotation("Npgsql:Enum:estado_pago_enum", "pendiente,completado,fallido,reembolsado")
                .OldAnnotation("Npgsql:Enum:estado_pedido_enum", "pendiente,aceptado,en_preparacion,en_camino,entregado,cancelado")
                .OldAnnotation("Npgsql:Enum:estado_restaurante_enum", "pendiente,aprobado,rechazado,suspendido")
                .OldAnnotation("Npgsql:Enum:tipo_descuento_enum", "porcentaje,monto_fijo")
                .OldAnnotation("Npgsql:Enum:tipo_metodo_pago_enum", "efectivo,tarjeta,transferencia,billetera_digital")
                .OldAnnotation("Npgsql:Enum:tipo_usuario_enum", "administrador,cliente,repartidor,restaurante")
                .OldAnnotation("Npgsql:Enum:tipo_vehiculo_enum", "bicicleta,motocicleta,automovil");

            migrationBuilder.CreateTable(
                name: "registros_auditoria",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    fecha_hora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modulo_afectado = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tipo_accion = table.Column<int>(type: "integer", nullable: false),
                    detalles = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registros_auditoria", x => x.id);
                    table.ForeignKey(
                        name: "FK_registros_auditoria_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_registros_auditoria_usuario_id",
                table: "registros_auditoria",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "registros_auditoria");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:estado_aprobacion_enum", "pendiente,aprobado,rechazado")
                .Annotation("Npgsql:Enum:estado_pago_enum", "pendiente,completado,fallido,reembolsado")
                .Annotation("Npgsql:Enum:estado_pedido_enum", "pendiente,aceptado,en_preparacion,en_camino,entregado,cancelado")
                .Annotation("Npgsql:Enum:estado_restaurante_enum", "pendiente,aprobado,rechazado,suspendido")
                .Annotation("Npgsql:Enum:tipo_descuento_enum", "porcentaje,monto_fijo")
                .Annotation("Npgsql:Enum:tipo_metodo_pago_enum", "efectivo,tarjeta,transferencia,billetera_digital")
                .Annotation("Npgsql:Enum:tipo_usuario_enum", "administrador,cliente,repartidor,restaurante")
                .Annotation("Npgsql:Enum:tipo_vehiculo_enum", "bicicleta,motocicleta,automovil")
                .OldAnnotation("Npgsql:Enum:estado_aprobacion_enum", "pendiente,aprobado,rechazado")
                .OldAnnotation("Npgsql:Enum:estado_pago_enum", "pendiente,completado,fallido,reembolsado")
                .OldAnnotation("Npgsql:Enum:estado_pedido_enum", "borrador,pendiente,aceptado,en_preparacion,en_camino,entregado,cancelado")
                .OldAnnotation("Npgsql:Enum:estado_restaurante_enum", "pendiente,aprobado,rechazado,suspendido")
                .OldAnnotation("Npgsql:Enum:tipo_accion_auditoria_enum", "crear,editar,eliminar,cambio_estado,inicio_sesion,cierre_sesion")
                .OldAnnotation("Npgsql:Enum:tipo_descuento_enum", "porcentaje,monto_fijo")
                .OldAnnotation("Npgsql:Enum:tipo_metodo_pago_enum", "efectivo,tarjeta,transferencia,billetera_digital")
                .OldAnnotation("Npgsql:Enum:tipo_usuario_enum", "administrador,cliente,repartidor,restaurante")
                .OldAnnotation("Npgsql:Enum:tipo_vehiculo_enum", "bicicleta,motocicleta,automovil");
        }
    }
}
