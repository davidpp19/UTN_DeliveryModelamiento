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

# Shared strings for all standard Spanish CRUDs
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

# CategoriasProducto
cat_reps = shared_crud_reps.copy()
cat_reps.update({
    '"Crear CategoriaProducto"': 'Localizer["CrearCategoriaProductoTitle"]',
    '>Crear CategoriaProducto<': '>@Localizer["CrearCategoriaProductoTitle"]<',
    '"Eliminar CategoriaProducto"': 'Localizer["EliminarCategoriaProductoTitle"]',
    '"Detalles CategoriaProducto"': 'Localizer["DetallesCategoriaProductoTitle"]',
    '>Detalles CategoriaProducto<': '>@Localizer["DetallesCategoriaProductoTitle"]<',
    '"Editar CategoriaProducto"': 'Localizer["EditarCategoriaProductoTitle"]',
    '>Editar CategoriaProducto<': '>@Localizer["EditarCategoriaProductoTitle"]<',
    '"CategoriasProducto"': 'Localizer["CategoriasProductoTitle"]',
    '>CategoriasProducto<': '>@Localizer["CategoriasProductoTitle"]<'
})
replace_in_files('CategoriasProducto', cat_reps)

# Vehiculos
veh_reps = shared_crud_reps.copy()
veh_reps.update({
    '"Crear Vehiculo"': 'Localizer["CrearVehiculoTitle"]',
    '>Crear Vehiculo<': '>@Localizer["CrearVehiculoTitle"]<',
    '"Eliminar Vehiculo"': 'Localizer["EliminarVehiculoTitle"]',
    '"Detalles Vehiculo"': 'Localizer["DetallesVehiculoTitle"]',
    '>Detalles Vehiculo<': '>@Localizer["DetallesVehiculoTitle"]<',
    '"Editar Vehiculo"': 'Localizer["EditarVehiculoTitle"]',
    '>Editar Vehiculo<': '>@Localizer["EditarVehiculoTitle"]<',
    '"Vehiculos"': 'Localizer["VehiculosTitle"]',
    '>Vehiculos<': '>@Localizer["VehiculosTitle"]<'
})
replace_in_files('Vehiculos', veh_reps)

# Auditorias
aud_reps = shared_crud_reps.copy()
aud_reps.update({
    '"Crear RegistroAuditoria"': 'Localizer["CrearAuditoriaTitle"]',
    '>Crear RegistroAuditoria<': '>@Localizer["CrearAuditoriaTitle"]<',
    '"Eliminar RegistroAuditoria"': 'Localizer["EliminarAuditoriaTitle"]',
    '"Detalles RegistroAuditoria"': 'Localizer["DetallesAuditoriaTitle"]',
    '>Detalles RegistroAuditoria<': '>@Localizer["DetallesAuditoriaTitle"]<',
    '"Editar RegistroAuditoria"': 'Localizer["EditarAuditoriaTitle"]',
    '>Editar RegistroAuditoria<': '>@Localizer["EditarAuditoriaTitle"]<',
    '"Auditorias"': 'Localizer["AuditoriasTitle"]',
    '>Auditorias<': '>@Localizer["AuditoriasTitle"]<'
})
replace_in_files('Auditorias', aud_reps)

# Pagos
pag_reps = shared_crud_reps.copy()
pag_reps.update({
    '"Crear Pago"': 'Localizer["CrearPagoTitle"]',
    '>Crear Pago<': '>@Localizer["CrearPagoTitle"]<',
    '"Eliminar Pago"': 'Localizer["EliminarPagoTitle"]',
    '"Detalles Pago"': 'Localizer["DetallesPagoTitle"]',
    '>Detalles Pago<': '>@Localizer["DetallesPagoTitle"]<',
    '"Editar Pago"': 'Localizer["EditarPagoTitle"]',
    '>Editar Pago<': '>@Localizer["EditarPagoTitle"]<',
    '"Pagos"': 'Localizer["PagosTitle"]',
    '>Pagos<': '>@Localizer["PagosTitle"]<'
})
replace_in_files('Pagos', pag_reps)

# Resenas
res_reps = shared_crud_reps.copy()
res_reps.update({
    '"Crear Resena"': 'Localizer["CrearResenaTitle"]',
    '>Crear Resena<': '>@Localizer["CrearResenaTitle"]<',
    '"Eliminar Resena"': 'Localizer["EliminarResenaTitle"]',
    '"Detalles Resena"': 'Localizer["DetallesResenaTitle"]',
    '>Detalles Resena<': '>@Localizer["DetallesResenaTitle"]<',
    '"Editar Resena"': 'Localizer["EditarResenaTitle"]',
    '>Editar Resena<': '>@Localizer["EditarResenaTitle"]<',
    '"Resenas"': 'Localizer["ResenasTitle"]',
    '>Resenas<': '>@Localizer["ResenasTitle"]<'
})
replace_in_files('Resenas', res_reps)

# Roles (English base strings)
rol_reps = {
    '>Create Rol<': '>@Localizer["CrearRolTitle"]<',
    '>Delete Rol<': '>@Localizer["EliminarRolTitle"]<',
    '>Are you sure you want to delete this?<': '>@Localizer["ConfirmarEliminarMsg"]<',
    '>Details Rol<': '>@Localizer["DetallesRolTitle"]<',
    '>Edit Rol<': '>@Localizer["EditarRolTitle"]<',
    '>Rol - Index<': '>@Localizer["RolesTitle"]<',
    '>Create New<': '>@Localizer["CrearNuevoBtn"]<',
    '>Id / Info<': '>@Localizer["IdInfoTag"]<',
    '>Acciones<': '>@Localizer["AccionesTag"]<',
    '>Edit<': '>@Localizer["EditarBtn"]<',
    '>Details<': '>@Localizer["DetallesBtn"]<',
    '>Delete<': '>@Localizer["EliminarBtn"]<',
    '>Back to List<': '>@Localizer["VolverListadoBtn"]<',
    'value="Create"': 'value="@Localizer["GuardarBtn"]"',
    'value="Save"': 'value="@Localizer["ActualizarBtn"]"',
    'value="Delete"': 'value="@Localizer["EliminarConfirmadoBtn"]"'
}
replace_in_files('Roles', rol_reps)
