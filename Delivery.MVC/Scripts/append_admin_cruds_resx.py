import os

keys_es = {
    'ConfirmarEliminarMsg': '¿Estás seguro de que quieres eliminar esto?',
    'GuardarBtn': 'Guardar',
    'VolverListadoBtn': 'Volver al listado',
    'EliminarConfirmadoBtn': 'Eliminar Confirmado',
    'CancelarBtn': 'Cancelar',
    'ActualizarBtn': 'Actualizar',
    'CrearNuevoBtn': 'Crear Nuevo',
    'AccionesCol': 'Acciones',
    'EditarBtn': 'Editar',
    'DetallesBtn': 'Detalles',
    'EliminarBtn': 'Eliminar',
    
    'CrearCategoriaProductoTitle': 'Crear Categoría de Producto',
    'EliminarCategoriaProductoTitle': 'Eliminar Categoría de Producto',
    'DetallesCategoriaProductoTitle': 'Detalles de Categoría de Producto',
    'EditarCategoriaProductoTitle': 'Editar Categoría de Producto',
    'CategoriasProductoTitle': 'Categorías de Producto',
    
    'CrearVehiculoTitle': 'Crear Vehículo',
    'EliminarVehiculoTitle': 'Eliminar Vehículo',
    'DetallesVehiculoTitle': 'Detalles de Vehículo',
    'EditarVehiculoTitle': 'Editar Vehículo',
    'VehiculosTitle': 'Vehículos',
    
    'CrearAuditoriaTitle': 'Crear Registro de Auditoría',
    'EliminarAuditoriaTitle': 'Eliminar Registro de Auditoría',
    'DetallesAuditoriaTitle': 'Detalles de Registro de Auditoría',
    'EditarAuditoriaTitle': 'Editar Registro de Auditoría',
    'AuditoriasTitle': 'Registros de Auditoría',
    
    'CrearPagoTitle': 'Crear Pago',
    'EliminarPagoTitle': 'Eliminar Pago',
    'DetallesPagoTitle': 'Detalles de Pago',
    'EditarPagoTitle': 'Editar Pago',
    'PagosTitle': 'Pagos',
    
    'CrearResenaTitle': 'Crear Reseña',
    'EliminarResenaTitle': 'Eliminar Reseña',
    'DetallesResenaTitle': 'Detalles de Reseña',
    'EditarResenaTitle': 'Editar Reseña',
    'ResenasTitle': 'Reseñas',
    
    'CrearRolTitle': 'Crear Rol',
    'EliminarRolTitle': 'Eliminar Rol',
    'DetallesRolTitle': 'Detalles del Rol',
    'EditarRolTitle': 'Editar Rol',
    'RolesTitle': 'Roles',
    'IdInfoTag': 'Id / Info'
}

keys_en = {
    'ConfirmarEliminarMsg': 'Are you sure you want to delete this?',
    'GuardarBtn': 'Save',
    'VolverListadoBtn': 'Back to List',
    'EliminarConfirmadoBtn': 'Delete Confirmed',
    'CancelarBtn': 'Cancel',
    'ActualizarBtn': 'Update',
    'CrearNuevoBtn': 'Create New',
    'AccionesCol': 'Actions',
    'EditarBtn': 'Edit',
    'DetallesBtn': 'Details',
    'EliminarBtn': 'Delete',
    
    'CrearCategoriaProductoTitle': 'Create Product Category',
    'EliminarCategoriaProductoTitle': 'Delete Product Category',
    'DetallesCategoriaProductoTitle': 'Product Category Details',
    'EditarCategoriaProductoTitle': 'Edit Product Category',
    'CategoriasProductoTitle': 'Product Categories',
    
    'CrearVehiculoTitle': 'Create Vehicle',
    'EliminarVehiculoTitle': 'Delete Vehicle',
    'DetallesVehiculoTitle': 'Vehicle Details',
    'EditarVehiculoTitle': 'Edit Vehicle',
    'VehiculosTitle': 'Vehicles',
    
    'CrearAuditoriaTitle': 'Create Audit Record',
    'EliminarAuditoriaTitle': 'Delete Audit Record',
    'DetallesAuditoriaTitle': 'Audit Record Details',
    'EditarAuditoriaTitle': 'Edit Audit Record',
    'AuditoriasTitle': 'Audit Records',
    
    'CrearPagoTitle': 'Create Payment',
    'EliminarPagoTitle': 'Delete Payment',
    'DetallesPagoTitle': 'Payment Details',
    'EditarPagoTitle': 'Edit Payment',
    'PagosTitle': 'Payments',
    
    'CrearResenaTitle': 'Create Review',
    'EliminarResenaTitle': 'Delete Review',
    'DetallesResenaTitle': 'Review Details',
    'EditarResenaTitle': 'Edit Review',
    'ResenasTitle': 'Reviews',
    
    'CrearRolTitle': 'Create Role',
    'EliminarRolTitle': 'Delete Role',
    'DetallesRolTitle': 'Role Details',
    'EditarRolTitle': 'Edit Role',
    'RolesTitle': 'Roles',
    'IdInfoTag': 'Id / Info'
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
