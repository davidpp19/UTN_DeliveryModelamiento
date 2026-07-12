import os

def process_file(filepath, replacements):
    if not os.path.exists(filepath): return
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    if '@inject IHtmlLocalizer<Delivery.MVC.SharedResource> Localizer' not in content:
        content = '@using Microsoft.AspNetCore.Mvc.Localization\n@inject IHtmlLocalizer<Delivery.MVC.SharedResource> Localizer\n' + content

    for k, v in replacements.items():
        content = content.replace(k, v)

    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)

# ----------------- ClienteCupones/Index.cshtml -----------------
cupones_reps = {
    '"Mis Cupones"': 'Localizer["MisCupones"]',
    '> Mis Cupones<': '> @Localizer["MisCupones"]<',
    'Gestiona tus cupones de descuento y códigos promocionales.': '@Localizer["GestionaCupones"]',
    '➕ Agregar Cupón': '➕ @Localizer["AgregarCuponBtn"]',
    'Registrar Nuevo Cupón': '@Localizer["RegistrarNuevoCupon"]',
    '>Código del cupón<': '>@Localizer["CodigoCuponTag"]<',
    'placeholder="Ingresar código"': 'placeholder="@Localizer["IngresarCodigoPlaceholder"]"',
    '>Cancelar<': '>@Localizer["CancelarBtn"]<',
    '>Agregar<': '>@Localizer["AgregarBtn"]<',
    'Aún no tienes cupones registrados.': '@Localizer["SinCupones"]',
    'Ingresa un código en la parte superior para agregarlo a tu cuenta.': '@Localizer["SinCuponesDesc"]',
    'DISPONIBLE': '@Localizer["Disponible"]',
    'UTILIZADO': '@Localizer["Utilizado"]',
    'VENCIDO': '@Localizer["Vencido"]',
    '>Descripción:<': '>@Localizer["DescripcionTag"]<',
    '>Tope máximo:<': '>@Localizer["TopeMaximoTag"]<',
    '>Vence:<': '>@Localizer["VenceTag"]<',
    '>Días restantes:<': '>@Localizer["DiasRestantesTag"]<',
    'Expira hoy': '@Localizer["ExpiraHoy"]',
    '>Registrado:<': '>@Localizer["RegistradoTag"]<',
    '>Compra mínima:<': '>@Localizer["CompraMinimaTag"]<',
    'Copiar Código': '@Localizer["CopiarCodigoBtn"]',
    'Utilizado el ': '@Localizer["UtilizadoElTag"] '
}
process_file('Delivery.MVC/Views/ClienteCupones/Index.cshtml', cupones_reps)

# ----------------- ClienteDirecciones/Create.cshtml -----------------
create_dir_reps = {
    '"Nueva Dirección"': 'Localizer["NuevaDireccion"]',
    '>Nueva Dirección<': '>@Localizer["NuevaDireccion"]<',
    'Busca en el mapa o haz clic para ubicar tu dirección exacta.': '@Localizer["BuscaMapaDireccionDesc"]',
    '>Buscar Lugar<': '>@Localizer["BuscarLugarTag"]<',
    'placeholder="Ej: Ibarra, Parque Ciudad Blanca..."': 'placeholder="@Localizer["EjemploBuscarLugar"]"',
    '>Buscar<': '>@Localizer["BuscarBtn"]<',
    'Busca tu barrio o calle y haz clic en el mapa.': '@Localizer["BuscaBarrioMapaDesc"]',
    '>Alias (Casa, Trabajo...)<': '>@Localizer["AliasTag"]<',
    'placeholder="Ej: Casa"': 'placeholder="@Localizer["EjemploCasa"]"',
    '>Calle Principal<': '>@Localizer["CallePrincipalTag"]<',
    'placeholder="Ej: Av. 17 de Julio"': 'placeholder="@Localizer["EjemploCalle"]"',
    '>Número<': '>@Localizer["NumeroTag"]<',
    'placeholder="Ej: 5-32"': 'placeholder="@Localizer["EjemploNumero"]"',
    '>Intersección<': '>@Localizer["InterseccionTag"]<',
    'placeholder="Ej: Calle 2"': 'placeholder="@Localizer["EjemploInterseccion"]"',
    '>Referencia<': '>@Localizer["ReferenciaTag"]<',
    'placeholder="Ej: Frente al parque"': 'placeholder="@Localizer["EjemploReferencia"]"',
    '>Ciudad<': '>@Localizer["CiudadTag"]<',
    'placeholder="Ej: Ibarra"': 'placeholder="@Localizer["EjemploCiudad"]"',
    '>Ubicación GPS<': '>@Localizer["UbicacionGPSTag"]<',
    'El mapa rellenará estos campos automáticamente.': '@Localizer["MapaRellenaraDesc"]',
    '>Latitud<': '>@Localizer["LatitudTag"]<',
    '>Longitud<': '>@Localizer["LongitudTag"]<',
    '>Guardar Dirección<': '>@Localizer["GuardarDireccionBtn"]<',
    'Volver a la lista': '@Localizer["VolverListaBtn"]'
}
process_file('Delivery.MVC/Views/ClienteDirecciones/Create.cshtml', create_dir_reps)

# ----------------- ClienteDirecciones/Index.cshtml -----------------
index_dir_reps = {
    '"Mis Direcciones"': 'Localizer["MisDirecciones"]',
    '>Mis Direcciones<': '>@Localizer["MisDirecciones"]<',
    'Gestiona tus direcciones de entrega para agilizar tus pedidos.': '@Localizer["GestionaDireccionesDesc"]',
    '➕ Nueva Dirección': '➕ @Localizer["NuevaDireccionBtn"]',
    'Aún no tienes direcciones registradas.': '@Localizer["SinDirecciones"]',
    'Agrega tu primera dirección para comenzar a pedir.': '@Localizer["AgregaPrimeraDireccionDesc"]',
    'Editar': '@Localizer["EditarBtn"]',
    'Eliminar': '@Localizer["EliminarBtn"]'
}
process_file('Delivery.MVC/Views/ClienteDirecciones/Index.cshtml', index_dir_reps)

# ----------------- ClienteDirecciones/Edit.cshtml -----------------
edit_dir_reps = {
    '"Editar Dirección"': 'Localizer["EditarDireccion"]',
    '>Editar Dirección<': '>@Localizer["EditarDireccion"]<',
    'Actualiza la información de tu lugar de entrega.': '@Localizer["ActualizaLugarEntregaDesc"]',
    '>Buscar Lugar<': '>@Localizer["BuscarLugarTag"]<',
    'placeholder="Ej: Ibarra, Parque Ciudad Blanca..."': 'placeholder="@Localizer["EjemploBuscarLugar"]"',
    '>Buscar<': '>@Localizer["BuscarBtn"]<',
    'Busca tu barrio o calle y haz clic en el mapa para actualizar coordenadas.': '@Localizer["BuscaBarrioMapaActDesc"]',
    '>Alias (Casa, Trabajo...)<': '>@Localizer["AliasTag"]<',
    '>Calle Principal<': '>@Localizer["CallePrincipalTag"]<',
    '>Número<': '>@Localizer["NumeroTag"]<',
    '>Intersección<': '>@Localizer["InterseccionTag"]<',
    '>Referencia<': '>@Localizer["ReferenciaTag"]<',
    '>Ciudad<': '>@Localizer["CiudadTag"]<',
    '>Ubicación GPS<': '>@Localizer["UbicacionGPSTag"]<',
    'El mapa actualizará estos campos.': '@Localizer["MapaActualizaraDesc"]',
    '>Latitud<': '>@Localizer["LatitudTag"]<',
    '>Longitud<': '>@Localizer["LongitudTag"]<',
    '>Guardar Cambios<': '>@Localizer["GuardarCambiosBtn"]<',
    'Volver a la lista': '@Localizer["VolverListaBtn"]'
}
process_file('Delivery.MVC/Views/ClienteDirecciones/Edit.cshtml', edit_dir_reps)

# ----------------- ClienteDirecciones/Delete.cshtml -----------------
del_dir_reps = {
    '"Eliminar Dirección"': 'Localizer["EliminarDireccion"]',
    '>Eliminar Dirección<': '>@Localizer["EliminarDireccion"]<',
    '¿Estás seguro de que deseas eliminar esta dirección?': '@Localizer["ConfirmarEliminarDireccion"]',
    'Esta acción no se puede deshacer.': '@Localizer["AccionNoDeshacer"]',
    '>Alias<': '>@Localizer["AliasTag"]<',
    '>Calle Principal<': '>@Localizer["CallePrincipalTag"]<',
    '>Número<': '>@Localizer["NumeroTag"]<',
    '>Intersección<': '>@Localizer["InterseccionTag"]<',
    '>Ciudad<': '>@Localizer["CiudadTag"]<',
    '>Sí, Eliminar Dirección<': '>@Localizer["SiEliminarDireccionBtn"]<',
    'Cancelar': '@Localizer["CancelarBtn"]'
}
process_file('Delivery.MVC/Views/ClienteDirecciones/Delete.cshtml', del_dir_reps)

# ----------------- ClienteDirecciones/Details.cshtml -----------------
det_dir_reps = {
    '"Detalles de Dirección"': 'Localizer["DetallesDireccion"]',
    '>Detalles de Dirección<': '>@Localizer["DetallesDireccion"]<',
    '>Alias<': '>@Localizer["AliasTag"]<',
    '>Calle Principal<': '>@Localizer["CallePrincipalTag"]<',
    '>Número<': '>@Localizer["NumeroTag"]<',
    '>Intersección<': '>@Localizer["InterseccionTag"]<',
    '>Referencia<': '>@Localizer["ReferenciaTag"]<',
    '>Ciudad<': '>@Localizer["CiudadTag"]<',
    '>Latitud<': '>@Localizer["LatitudTag"]<',
    '>Longitud<': '>@Localizer["LongitudTag"]<',
    '>Editar<': '>@Localizer["EditarBtn"]<',
    'Volver a la lista': '@Localizer["VolverListaBtn"]'
}
process_file('Delivery.MVC/Views/ClienteDirecciones/Details.cshtml', det_dir_reps)
