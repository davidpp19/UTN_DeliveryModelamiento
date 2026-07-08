using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Delivery.Modelos.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:estado_aprobacion_enum", "pendiente,aprobado,rechazado")
                .Annotation("Npgsql:Enum:estado_pago_enum", "pendiente,completado,fallido,reembolsado")
                .Annotation("Npgsql:Enum:estado_pedido_enum", "pendiente,aceptado,en_preparacion,en_camino,entregado,cancelado")
                .Annotation("Npgsql:Enum:estado_restaurante_enum", "pendiente,aprobado,rechazado,suspendido")
                .Annotation("Npgsql:Enum:tipo_descuento_enum", "porcentaje,monto_fijo")
                .Annotation("Npgsql:Enum:tipo_metodo_pago_enum", "efectivo,tarjeta,transferencia,billetera_digital")
                .Annotation("Npgsql:Enum:tipo_usuario_enum", "administrador,cliente,repartidor,restaurante")
                .Annotation("Npgsql:Enum:tipo_vehiculo_enum", "bicicleta,motocicleta,automovil");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rol_id = table.Column<long>(type: "bigint", nullable: true),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    apellidos = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    tipo_usuario = table.Column<int>(type: "integer", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    foto_perfil_url = table.Column<string>(type: "text", nullable: true),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuarios_roles_rol_id",
                        column: x => x.rol_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "direcciones",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<long>(type: "bigint", nullable: false),
                    alias = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    calle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ciudad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    referencia = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    latitud = table.Column<decimal>(type: "numeric(10,8)", nullable: true),
                    longitud = table.Column<decimal>(type: "numeric(11,8)", nullable: true),
                    es_principal = table.Column<bool>(type: "boolean", nullable: false),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_direcciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_direcciones_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "repartidores",
                columns: table => new
                {
                    usuario_id = table.Column<long>(type: "bigint", nullable: false),
                    licencia_conducir = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    foto_licencia_url = table.Column<string>(type: "text", nullable: true),
                    estado_aprobacion = table.Column<int>(type: "integer", nullable: false),
                    aprobado_por = table.Column<long>(type: "bigint", nullable: true),
                    fecha_aprobacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    disponible = table.Column<bool>(type: "boolean", nullable: false),
                    calificacion_promedio = table.Column<decimal>(type: "numeric(3,2)", nullable: true),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_repartidores", x => x.usuario_id);
                    table.ForeignKey(
                        name: "FK_repartidores_usuarios_aprobado_por",
                        column: x => x.aprobado_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_repartidores_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "restaurantes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    categoria = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    calle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ciudad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    latitud = table.Column<decimal>(type: "numeric(10,8)", nullable: true),
                    longitud = table.Column<decimal>(type: "numeric(11,8)", nullable: true),
                    telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    logo_url = table.Column<string>(type: "text", nullable: true),
                    hora_apertura = table.Column<TimeSpan>(type: "interval", nullable: true),
                    hora_cierre = table.Column<TimeSpan>(type: "interval", nullable: true),
                    costo_envio_base = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    aprobado_por = table.Column<long>(type: "bigint", nullable: true),
                    fecha_aprobacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    abierto = table.Column<bool>(type: "boolean", nullable: false),
                    creado_por = table.Column<long>(type: "bigint", nullable: true),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_restaurantes", x => x.id);
                    table.ForeignKey(
                        name: "FK_restaurantes_usuarios_aprobado_por",
                        column: x => x.aprobado_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_restaurantes_usuarios_creado_por",
                        column: x => x.creado_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ubicacion_actual_repartidor",
                columns: table => new
                {
                    repartidor_id = table.Column<long>(type: "bigint", nullable: false),
                    latitud = table.Column<decimal>(type: "numeric(10,8)", nullable: true),
                    longitud = table.Column<decimal>(type: "numeric(11,8)", nullable: true),
                    rumbo = table.Column<decimal>(type: "numeric(6,2)", nullable: true),
                    velocidad = table.Column<decimal>(type: "numeric(6,2)", nullable: true),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ubicacion_actual_repartidor", x => x.repartidor_id);
                    table.ForeignKey(
                        name: "FK_ubicacion_actual_repartidor_repartidores_repartidor_id",
                        column: x => x.repartidor_id,
                        principalTable: "repartidores",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vehiculos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    repartidor_id = table.Column<long>(type: "bigint", nullable: false),
                    tipo_vehiculo = table.Column<int>(type: "integer", nullable: false),
                    placa = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    marca = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    modelo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    anio = table.Column<short>(type: "smallint", nullable: true),
                    color = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    foto_vehiculo_url = table.Column<string>(type: "text", nullable: true),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehiculos", x => x.id);
                    table.ForeignKey(
                        name: "FK_vehiculos_repartidores_repartidor_id",
                        column: x => x.repartidor_id,
                        principalTable: "repartidores",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "categorias_producto",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    restaurante_id = table.Column<long>(type: "bigint", nullable: false),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias_producto", x => x.id);
                    table.ForeignKey(
                        name: "FK_categorias_producto_restaurantes_restaurante_id",
                        column: x => x.restaurante_id,
                        principalTable: "restaurantes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cupones",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tipo_descuento = table.Column<int>(type: "integer", nullable: false),
                    valor_descuento = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    pedido_minimo = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    limite_usos = table.Column<int>(type: "integer", nullable: true),
                    usos_actuales = table.Column<int>(type: "integer", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    restaurante_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cupones", x => x.id);
                    table.ForeignKey(
                        name: "FK_cupones_restaurantes_restaurante_id",
                        column: x => x.restaurante_id,
                        principalTable: "restaurantes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "favoritos",
                columns: table => new
                {
                    usuario_id = table.Column<long>(type: "bigint", nullable: false),
                    restaurante_id = table.Column<long>(type: "bigint", nullable: false),
                    fecha_agregado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_favoritos", x => new { x.usuario_id, x.restaurante_id });
                    table.ForeignKey(
                        name: "FK_favoritos_restaurantes_restaurante_id",
                        column: x => x.restaurante_id,
                        principalTable: "restaurantes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_favoritos_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pedidos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<long>(type: "bigint", nullable: false),
                    restaurante_id = table.Column<long>(type: "bigint", nullable: false),
                    repartidor_id = table.Column<long>(type: "bigint", nullable: true),
                    direccion_entrega_id = table.Column<long>(type: "bigint", nullable: false),
                    estado_pedido = table.Column<int>(type: "integer", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    costo_envio = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    total = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    tipo_metodo_pago = table.Column<int>(type: "integer", nullable: false),
                    metodo_pago_id = table.Column<long>(type: "bigint", nullable: true),
                    cupon_id = table.Column<long>(type: "bigint", nullable: true),
                    monto_descuento = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    notas = table.Column<string>(type: "text", nullable: true),
                    fecha_pedido = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_entrega_estimada = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_entrega_real = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedidos", x => x.id);
                    table.ForeignKey(
                        name: "FK_pedidos_direcciones_direccion_entrega_id",
                        column: x => x.direccion_entrega_id,
                        principalTable: "direcciones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pedidos_repartidores_repartidor_id",
                        column: x => x.repartidor_id,
                        principalTable: "repartidores",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_pedidos_restaurantes_restaurante_id",
                        column: x => x.restaurante_id,
                        principalTable: "restaurantes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pedidos_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "productos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    restaurante_id = table.Column<long>(type: "bigint", nullable: false),
                    categoria_id = table.Column<long>(type: "bigint", nullable: true),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    precio = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    imagen_url = table.Column<string>(type: "text", nullable: true),
                    disponible = table.Column<bool>(type: "boolean", nullable: false),
                    tiempo_preparacion = table.Column<int>(type: "integer", nullable: true),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productos", x => x.id);
                    table.ForeignKey(
                        name: "FK_productos_categorias_producto_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias_producto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_productos_restaurantes_restaurante_id",
                        column: x => x.restaurante_id,
                        principalTable: "restaurantes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cupones_usuarios",
                columns: table => new
                {
                    cupon_id = table.Column<long>(type: "bigint", nullable: false),
                    usuario_id = table.Column<long>(type: "bigint", nullable: false),
                    pedido_id = table.Column<long>(type: "bigint", nullable: false),
                    fecha_uso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cupones_usuarios", x => new { x.cupon_id, x.usuario_id, x.pedido_id });
                    table.ForeignKey(
                        name: "FK_cupones_usuarios_cupones_cupon_id",
                        column: x => x.cupon_id,
                        principalTable: "cupones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cupones_usuarios_pedidos_pedido_id",
                        column: x => x.pedido_id,
                        principalTable: "pedidos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_cupones_usuarios_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "historial_asignaciones_repartidor",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    repartidor_id = table.Column<long>(type: "bigint", nullable: false),
                    pedido_id = table.Column<long>(type: "bigint", nullable: false),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historial_asignaciones_repartidor", x => x.id);
                    table.ForeignKey(
                        name: "FK_historial_asignaciones_repartidor_pedidos_pedido_id",
                        column: x => x.pedido_id,
                        principalTable: "pedidos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_historial_asignaciones_repartidor_repartidores_repartidor_id",
                        column: x => x.repartidor_id,
                        principalTable: "repartidores",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pagos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pedido_id = table.Column<long>(type: "bigint", nullable: false),
                    tipo_metodo_pago = table.Column<int>(type: "integer", nullable: false),
                    monto = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    estado_pago = table.Column<int>(type: "integer", nullable: false),
                    referencia_transaccion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    fecha_pago = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagos", x => x.id);
                    table.ForeignKey(
                        name: "FK_pagos_pedidos_pedido_id",
                        column: x => x.pedido_id,
                        principalTable: "pedidos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resenas",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pedido_id = table.Column<long>(type: "bigint", nullable: false),
                    usuario_id = table.Column<long>(type: "bigint", nullable: false),
                    restaurante_id = table.Column<long>(type: "bigint", nullable: true),
                    repartidor_id = table.Column<long>(type: "bigint", nullable: true),
                    calificacion_restaurante = table.Column<short>(type: "smallint", nullable: true),
                    comentario_restaurante = table.Column<string>(type: "text", nullable: true),
                    calificacion_repartidor = table.Column<short>(type: "smallint", nullable: true),
                    comentario_repartidor = table.Column<string>(type: "text", nullable: true),
                    fecha_resena = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resenas", x => x.id);
                    table.ForeignKey(
                        name: "FK_resenas_pedidos_pedido_id",
                        column: x => x.pedido_id,
                        principalTable: "pedidos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resenas_repartidores_repartidor_id",
                        column: x => x.repartidor_id,
                        principalTable: "repartidores",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_resenas_restaurantes_restaurante_id",
                        column: x => x.restaurante_id,
                        principalTable: "restaurantes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_resenas_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "detalle_pedido",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pedido_id = table.Column<long>(type: "bigint", nullable: false),
                    producto_id = table.Column<long>(type: "bigint", nullable: false),
                    cantidad = table.Column<int>(type: "integer", nullable: false),
                    precio_unitario = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    notas_especiales = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalle_pedido", x => x.id);
                    table.ForeignKey(
                        name: "FK_detalle_pedido_pedidos_pedido_id",
                        column: x => x.pedido_id,
                        principalTable: "pedidos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_detalle_pedido_productos_producto_id",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_categorias_producto_restaurante_id",
                table: "categorias_producto",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "IX_cupones_restaurante_id",
                table: "cupones",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "IX_cupones_usuarios_pedido_id",
                table: "cupones_usuarios",
                column: "pedido_id");

            migrationBuilder.CreateIndex(
                name: "IX_cupones_usuarios_usuario_id",
                table: "cupones_usuarios",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_pedido_pedido_id",
                table: "detalle_pedido",
                column: "pedido_id");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_pedido_producto_id",
                table: "detalle_pedido",
                column: "producto_id");

            migrationBuilder.CreateIndex(
                name: "IX_direcciones_usuario_id",
                table: "direcciones",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_favoritos_restaurante_id",
                table: "favoritos",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "IX_historial_asignaciones_repartidor_pedido_id",
                table: "historial_asignaciones_repartidor",
                column: "pedido_id");

            migrationBuilder.CreateIndex(
                name: "IX_historial_asignaciones_repartidor_repartidor_id",
                table: "historial_asignaciones_repartidor",
                column: "repartidor_id");

            migrationBuilder.CreateIndex(
                name: "IX_pagos_pedido_id",
                table: "pagos",
                column: "pedido_id");

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_direccion_entrega_id",
                table: "pedidos",
                column: "direccion_entrega_id");

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_repartidor_id",
                table: "pedidos",
                column: "repartidor_id");

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_restaurante_id",
                table: "pedidos",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_usuario_id",
                table: "pedidos",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_productos_categoria_id",
                table: "productos",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "IX_productos_restaurante_id",
                table: "productos",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "IX_repartidores_aprobado_por",
                table: "repartidores",
                column: "aprobado_por");

            migrationBuilder.CreateIndex(
                name: "IX_resenas_pedido_id",
                table: "resenas",
                column: "pedido_id");

            migrationBuilder.CreateIndex(
                name: "IX_resenas_repartidor_id",
                table: "resenas",
                column: "repartidor_id");

            migrationBuilder.CreateIndex(
                name: "IX_resenas_restaurante_id",
                table: "resenas",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "IX_resenas_usuario_id",
                table: "resenas",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_restaurantes_aprobado_por",
                table: "restaurantes",
                column: "aprobado_por");

            migrationBuilder.CreateIndex(
                name: "IX_restaurantes_creado_por",
                table: "restaurantes",
                column: "creado_por");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_rol_id",
                table: "usuarios",
                column: "rol_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehiculos_repartidor_id",
                table: "vehiculos",
                column: "repartidor_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cupones_usuarios");

            migrationBuilder.DropTable(
                name: "detalle_pedido");

            migrationBuilder.DropTable(
                name: "favoritos");

            migrationBuilder.DropTable(
                name: "historial_asignaciones_repartidor");

            migrationBuilder.DropTable(
                name: "pagos");

            migrationBuilder.DropTable(
                name: "resenas");

            migrationBuilder.DropTable(
                name: "ubicacion_actual_repartidor");

            migrationBuilder.DropTable(
                name: "vehiculos");

            migrationBuilder.DropTable(
                name: "cupones");

            migrationBuilder.DropTable(
                name: "productos");

            migrationBuilder.DropTable(
                name: "pedidos");

            migrationBuilder.DropTable(
                name: "categorias_producto");

            migrationBuilder.DropTable(
                name: "direcciones");

            migrationBuilder.DropTable(
                name: "repartidores");

            migrationBuilder.DropTable(
                name: "restaurantes");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
