import os

keys_es = {
    'RegRepartidorTitulo': 'Registro de Repartidor',
    'RegRepartidorDesc': 'Completa el formulario para unirte como repartidor de RayoExpres. Tu cuenta será revisada antes de ser activada.',
    'VolverSeleccion': 'Volver a selección de tipo de cuenta',
    'DatosPersonales': 'Datos Personales',
    'TuNombrePlaceholder': 'Tu nombre',
    'TusApellidosPlaceholder': 'Tus apellidos',
    'DatosVehiculo': 'Datos del Vehículo',
    'TipoVehiculo': 'Tipo de Vehículo',
    'ConfirmarTipo': 'Confirmar Tipo',
    'ConfirmarTipoDesc': 'Selecciona y haz clic en "Confirmar Tipo" para actualizar los campos requeridos.',
    'DatosLicenciaRequerido': 'Datos de Licencia',
    'NumeroLicencia': 'Número de Licencia de Conducir',
    'FotoLicencia': 'Fotografía de la Licencia',
    'FotoLicenciaDesc': 'Adjunta una foto clara de tu licencia de conducir.',
    'MarcaVehiculo': 'Marca',
    'ModeloVehiculo': 'Modelo',
    'ColorVehiculo': 'Color',
    'PlacaVehiculo': 'Placa',
    'AnioVehiculo': 'Año del Vehículo',
    'RegRestauranteTitulo': 'Registra tu Restaurante en RayoExpres',
    'DatosPropietario': 'Datos del Propietario',
    'DatosRestaurante': 'Datos del Restaurante',
    'NombreRestaurante': 'Nombre del Restaurante',
    'Descripcion': 'Descripción',
    'RUC': 'RUC',
    'CategoriaEjemplo': 'Categoría (ej: Pizza, Sushi)',
    'HoraApertura': 'Hora de Apertura',
    'HoraCierre': 'Hora de Cierre',
    'Ubicacion': 'Ubicación',
    'BuscaDireccionMapa': 'Busca tu dirección o arrastra el marcador en el mapa.',
    'BuscarDireccion': 'Buscar Dirección',
    'DetallesUbicacion': 'Detalles de Ubicación',
    'Calle': 'Calle',
    'Ciudad': 'Ciudad',
    'CodigoPostal': 'Código Postal',
    'LogoRestaurante': 'Logo del Restaurante',
    'RegRestauranteBtn': 'Registrar Restaurante'
}

keys_en = {
    'RegRepartidorTitulo': 'Driver Registration',
    'RegRepartidorDesc': 'Complete the form to join as a RayoExpres driver. Your account will be reviewed before activation.',
    'VolverSeleccion': 'Back to account type selection',
    'DatosPersonales': 'Personal Details',
    'TuNombrePlaceholder': 'Your first name',
    'TusApellidosPlaceholder': 'Your last name',
    'DatosVehiculo': 'Vehicle Details',
    'TipoVehiculo': 'Vehicle Type',
    'ConfirmarTipo': 'Confirm Type',
    'ConfirmarTipoDesc': 'Select and click "Confirm Type" to update required fields.',
    'DatosLicenciaRequerido': 'License Details',
    'NumeroLicencia': 'Driver License Number',
    'FotoLicencia': 'License Photo',
    'FotoLicenciaDesc': 'Attach a clear photo of your driver license.',
    'MarcaVehiculo': 'Brand',
    'ModeloVehiculo': 'Model',
    'ColorVehiculo': 'Color',
    'PlacaVehiculo': 'License Plate',
    'AnioVehiculo': 'Vehicle Year',
    'RegRestauranteTitulo': 'Register your Restaurant on RayoExpres',
    'DatosPropietario': 'Owner Details',
    'DatosRestaurante': 'Restaurant Details',
    'NombreRestaurante': 'Restaurant Name',
    'Descripcion': 'Description',
    'RUC': 'Tax ID (RUC)',
    'CategoriaEjemplo': 'Category (e.g.: Pizza, Sushi)',
    'HoraApertura': 'Opening Hour',
    'HoraCierre': 'Closing Hour',
    'Ubicacion': 'Location',
    'BuscaDireccionMapa': 'Search your address or drag the marker on the map.',
    'BuscarDireccion': 'Search Address',
    'DetallesUbicacion': 'Location Details',
    'Calle': 'Street',
    'Ciudad': 'City',
    'CodigoPostal': 'Postal Code',
    'LogoRestaurante': 'Restaurant Logo',
    'RegRestauranteBtn': 'Register Restaurant'
}

def append_to_resx(filepath, keys):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    xml_data = ""
    for k, v in keys.items():
        xml_data += f'  <data name="{k}" xml:space="preserve"><value>{v}</value></data>\n'
    
    content = content.replace('</root>', xml_data + '</root>')
    
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)

append_to_resx('Delivery.MVC/Resources/SharedResource.es.resx', keys_es)
append_to_resx('Delivery.MVC/Resources/SharedResource.en.resx', keys_en)
