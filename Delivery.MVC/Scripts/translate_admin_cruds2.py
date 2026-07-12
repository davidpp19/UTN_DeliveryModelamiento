import os
import glob

def replace_in_files(folder, replacements):
    files = glob.glob(f'Delivery.MVC/Views/{folder}/*.cshtml')
    for filepath in files:
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
        
        if '@inject IHtmlLocalizer<Delivery.MVC.SharedResource> Localizer' not in content:
            content = '@using Microsoft.AspNetCore.Mvc.Localization\n@inject IHtmlLocalizer<Delivery.MVC.SharedResource> Localizer\n' + content

        for k, v in replacements.items():
            content = content.replace(k, v)

        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(content)

shared_crud_reps = {
    '>¿Estás seguro de que quieres eliminar esto?<': '>@Localizer["ConfirmarEliminarMsg"]<',
    '>Guardar<': '>@Localizer["GuardarBtn"]<',
    '>Volver al listado<': '>@Localizer["VolverListadoBtn"]<',
    '>Eliminar Confirmado<': '>@Localizer["EliminarConfirmadoBtn"]<',
    '>Cancelar<': '>@Localizer["CancelarBtn"]<',
    '>Actualizar<': '>@Localizer["ActualizarBtn"]<',
    '>Crear Nuevo<': '>@Localizer["CrearNuevoBtn"]<',
    '>Acciones<': '>@Localizer["AccionesTag"]<',
    '>Editar<': '>@Localizer["EditarBtn"]<',
    '>Detalles<': '>@Localizer["DetallesBtn"]<',
    '>Eliminar<': '>@Localizer["EliminarBtn"]<'
}

# Cupones
cup_reps = shared_crud_reps.copy()
cup_reps.update({
    '"Crear Cupón"': 'Localizer["CrearCuponTitle"]',
    '>Crear Cupón Global<': '>@Localizer["CrearCuponGlobalTitle"]<',
    '"Eliminar Cupon"': 'Localizer["EliminarCuponTitle"]',
    '"Detalles Cupon"': 'Localizer["DetallesCuponTitle"]',
    '>Detalles Cupon<': '>@Localizer["DetallesCuponTitle"]<',
    '"Editar Cupón"': 'Localizer["EditarCuponTitle"]',
    '>Editar Cupón Global<': '>@Localizer["EditarCuponGlobalTitle"]<',
    '"Cupones"': 'Localizer["CuponesTitle"]',
    '>Cupones<': '>@Localizer["CuponesTitle"]<',
    '>Descripción<': '>@Localizer["DescripcionTag"]<',
    '>Tipo de Descuento<': '>@Localizer["TipoDescuentoTag"]<',
    '>Porcentaje<': '>@Localizer["PorcentajeTag"]<',
    '>Valor Fijo ($)<': '>@Localizer["ValorFijoTag"]<',
    '>Valor (Ej. 10 para 10% o $10)<': '>@Localizer["ValorDescExplicativo"]<',
    '>Descuento Máximo ($) (Solo para Porcentaje)<': '>@Localizer["DescuentoMaximoTag"]<',
    '>Pedido Mínimo ($)<': '>@Localizer["PedidoMinimoTag"]<',
    '>Duración del Cupón<': '>@Localizer["DuracionCuponTag"]<',
    '>Límite de Usos (0 o vacío = Sin límite)<': '>@Localizer["LimiteUsosTag"]<',
    '>Cupón Activo<': '>@Localizer["CuponActivoTag"]<',
    '>Es Público (cualquiera puede registrarlo)<': '>@Localizer["EsPublicoTag"]<'
})
replace_in_files('Cupones', cup_reps)

# Pedidos
ped_reps = shared_crud_reps.copy()
ped_reps.update({
    '"Crear Pedido"': 'Localizer["CrearPedidoTitle"]',
    '>Crear Pedido<': '>@Localizer["CrearPedidoTitle"]<',
    '"Eliminar Pedido"': 'Localizer["EliminarPedidoTitle"]',
    '"Detalles Pedido"': 'Localizer["DetallesPedidoTitle"]',
    '>Detalles Pedido<': '>@Localizer["DetallesPedidoTitle"]<',
    '"Editar Pedido"': 'Localizer["EditarPedidoTitle"]',
    '>Editar Pedido<': '>@Localizer["EditarPedidoTitle"]<',
    '"Pedidos"': 'Localizer["PedidosTitle"]',
    '>Pedidos<': '>@Localizer["PedidosTitle"]<'
})
replace_in_files('Pedidos', ped_reps)

# Productos
prod_reps = shared_crud_reps.copy()
prod_reps.update({
    '"Crear Producto"': 'Localizer["CrearProductoTitle"]',
    '>Crear Producto<': '>@Localizer["CrearProductoTitle"]<',
    '"Eliminar Producto"': 'Localizer["EliminarProductoTitle"]',
    '"Detalles Producto"': 'Localizer["DetallesProductoTitle"]',
    '>Detalles Producto<': '>@Localizer["DetallesProductoTitle"]<',
    '"Editar Producto"': 'Localizer["EditarProductoTitle"]',
    '>Editar Producto<': '>@Localizer["EditarProductoTitle"]<',
    '"Productos"': 'Localizer["ProductosTitle"]',
    '>Productos<': '>@Localizer["ProductosTitle"]<'
})
replace_in_files('Productos', prod_reps)

# Repartidores
rep_reps = shared_crud_reps.copy()
rep_reps.update({
    '"Crear Repartidor"': 'Localizer["CrearRepartidorTitle"]',
    '>Crear Repartidor<': '>@Localizer["CrearRepartidorTitle"]<',
    '"Eliminar Repartidor"': 'Localizer["EliminarRepartidorTitle"]',
    '"Detalles Repartidor"': 'Localizer["DetallesRepartidorTitle"]',
    '>Detalles Repartidor<': '>@Localizer["DetallesRepartidorTitle"]<',
    '"Editar Repartidor"': 'Localizer["EditarRepartidorTitle"]',
    '>Editar Repartidor<': '>@Localizer["EditarRepartidorTitle"]<',
    '"Repartidores"': 'Localizer["RepartidoresTitle"]',
    '>Repartidores<': '>@Localizer["RepartidoresTitle"]<'
})
replace_in_files('Repartidores', rep_reps)

# Restaurantes
rest_reps = shared_crud_reps.copy()
rest_reps.update({
    '"Crear Restaurante"': 'Localizer["CrearRestauranteTitle"]',
    '>Crear Restaurante<': '>@Localizer["CrearRestauranteTitle"]<',
    '"Eliminar Restaurante"': 'Localizer["EliminarRestauranteTitle"]',
    '"Detalles Restaurante"': 'Localizer["DetallesRestauranteTitle"]',
    '>Detalles Restaurante<': '>@Localizer["DetallesRestauranteTitle"]<',
    '"Editar Restaurante"': 'Localizer["EditarRestauranteTitle"]',
    '>Editar Restaurante<': '>@Localizer["EditarRestauranteTitle"]<',
    '"Restaurantes"': 'Localizer["RestaurantesTitle"]',
    '>Restaurantes<': '>@Localizer["RestaurantesTitle"]<'
})
replace_in_files('Restaurantes', rest_reps)

# Usuarios
usr_reps = shared_crud_reps.copy()
usr_reps.update({
    '"Crear Usuario"': 'Localizer["CrearUsuarioTitle"]',
    '>Crear Usuario<': '>@Localizer["CrearUsuarioTitle"]<',
    '"Eliminar Usuario"': 'Localizer["EliminarUsuarioTitle"]',
    '"Detalles Usuario"': 'Localizer["DetallesUsuarioTitle"]',
    '>Detalles Usuario<': '>@Localizer["DetallesUsuarioTitle"]<',
    '"Editar Usuario"': 'Localizer["EditarUsuarioTitle"]',
    '>Editar Usuario<': '>@Localizer["EditarUsuarioTitle"]<',
    '"Usuarios"': 'Localizer["UsuariosTitle"]',
    '>Usuarios<': '>@Localizer["UsuariosTitle"]<'
})
replace_in_files('Usuarios', usr_reps)
