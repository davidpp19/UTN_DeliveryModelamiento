using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery.Modelos.Migrations
{
    /// <inheritdoc />
    public partial class RemovePostgresEnumMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                .Annotation("Npgsql:Enum:tipo_vehiculo_enum", "bicicleta,motocicleta,automovil");
        }
    }
}
