import os

keys_es = {
    'CrearCuponTitle': 'Crear Cupón',
    'CrearCuponGlobalTitle': 'Crear Cupón Global',
    'EliminarCuponTitle': 'Eliminar Cupón',
    'DetallesCuponTitle': 'Detalles del Cupón',
    'EditarCuponTitle': 'Editar Cupón',
    'EditarCuponGlobalTitle': 'Editar Cupón Global',
    'CuponesTitle': 'Cupones',
    'TipoDescuentoTag': 'Tipo de Descuento',
    'PorcentajeTag': 'Porcentaje',
    'ValorFijoTag': 'Valor Fijo ($)',
    'ValorDescExplicativo': 'Valor (Ej. 10 para 10% o $10)',
    'DescuentoMaximoTag': 'Descuento Máximo ($) (Solo para Porcentaje)',
    'PedidoMinimoTag': 'Pedido Mínimo ($)',
    'DuracionCuponTag': 'Duración del Cupón',
    'LimiteUsosTag': 'Límite de Usos (0 o vacío = Sin límite)',
    'CuponActivoTag': 'Cupón Activo',
    'EsPublicoTag': 'Es Público (cualquiera puede registrarlo)',
    
    'CrearPedidoTitle': 'Crear Pedido',
    'EliminarPedidoTitle': 'Eliminar Pedido',
    'DetallesPedidoTitle': 'Detalles del Pedido',
    'EditarPedidoTitle': 'Editar Pedido',
    'PedidosTitle': 'Pedidos',
    
    'CrearProductoTitle': 'Crear Producto',
    'EliminarProductoTitle': 'Eliminar Producto',
    'DetallesProductoTitle': 'Detalles de Producto',
    'EditarProductoTitle': 'Editar Producto',
    'ProductosTitle': 'Productos',
    
    'CrearRepartidorTitle': 'Crear Repartidor',
    'EliminarRepartidorTitle': 'Eliminar Repartidor',
    'DetallesRepartidorTitle': 'Detalles de Repartidor',
    'EditarRepartidorTitle': 'Editar Repartidor',
    'RepartidoresTitle': 'Repartidores',
    
    'CrearRestauranteTitle': 'Crear Restaurante',
    'EliminarRestauranteTitle': 'Eliminar Restaurante',
    'DetallesRestauranteTitle': 'Detalles de Restaurante',
    'EditarRestauranteTitle': 'Editar Restaurante',
    'RestaurantesTitle': 'Restaurantes',
    
    'CrearUsuarioTitle': 'Crear Usuario',
    'EliminarUsuarioTitle': 'Eliminar Usuario',
    'DetallesUsuarioTitle': 'Detalles de Usuario',
    'EditarUsuarioTitle': 'Editar Usuario',
    'UsuariosTitle': 'Usuarios'
}

keys_en = {
    'CrearCuponTitle': 'Create Coupon',
    'CrearCuponGlobalTitle': 'Create Global Coupon',
    'EliminarCuponTitle': 'Delete Coupon',
    'DetallesCuponTitle': 'Coupon Details',
    'EditarCuponTitle': 'Edit Coupon',
    'EditarCuponGlobalTitle': 'Edit Global Coupon',
    'CuponesTitle': 'Coupons',
    'TipoDescuentoTag': 'Discount Type',
    'PorcentajeTag': 'Percentage',
    'ValorFijoTag': 'Fixed Value ($)',
    'ValorDescExplicativo': 'Value (E.g., 10 for 10% or $10)',
    'DescuentoMaximoTag': 'Maximum Discount ($) (Percentage only)',
    'PedidoMinimoTag': 'Minimum Order ($)',
    'DuracionCuponTag': 'Coupon Duration',
    'LimiteUsosTag': 'Usage Limit (0 or empty = No limit)',
    'CuponActivoTag': 'Active Coupon',
    'EsPublicoTag': 'Is Public (anyone can register it)',
    
    'CrearPedidoTitle': 'Create Order',
    'EliminarPedidoTitle': 'Delete Order',
    'DetallesPedidoTitle': 'Order Details',
    'EditarPedidoTitle': 'Edit Order',
    'PedidosTitle': 'Orders',
    
    'CrearProductoTitle': 'Create Product',
    'EliminarProductoTitle': 'Delete Product',
    'DetallesProductoTitle': 'Product Details',
    'EditarProductoTitle': 'Edit Product',
    'ProductosTitle': 'Products',
    
    'CrearRepartidorTitle': 'Create Driver',
    'EliminarRepartidorTitle': 'Delete Driver',
    'DetallesRepartidorTitle': 'Driver Details',
    'EditarRepartidorTitle': 'Edit Driver',
    'RepartidoresTitle': 'Drivers',
    
    'CrearRestauranteTitle': 'Create Restaurant',
    'EliminarRestauranteTitle': 'Delete Restaurant',
    'DetallesRestauranteTitle': 'Restaurant Details',
    'EditarRestauranteTitle': 'Edit Restaurant',
    'RestaurantesTitle': 'Restaurants',
    
    'CrearUsuarioTitle': 'Create User',
    'EliminarUsuarioTitle': 'Delete User',
    'DetallesUsuarioTitle': 'User Details',
    'EditarUsuarioTitle': 'Edit User',
    'UsuariosTitle': 'Users'
}

def append_to_resx(filepath, keys):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    xml_data = ""
    for k, v in keys.items():
        safe_k = k.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;").replace('"', "&quot;").replace("'", "&apos;")
        safe_v = v.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;").replace('"', "&quot;").replace("'", "&apos;")
        xml_data += f'  <data name="{safe_k}" xml:space="preserve"><value>{safe_v}</value></data>\n'
    
    content = content.replace('</root>', xml_data + '</root>')
    
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)

append_to_resx('Delivery.MVC/Resources/SharedResource.es.resx', keys_es)
append_to_resx('Delivery.MVC/Resources/SharedResource.en.resx', keys_en)
