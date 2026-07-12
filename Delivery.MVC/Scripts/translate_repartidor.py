import os
import re

def process_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Add inject if not present
    if '@inject IHtmlLocalizer<Delivery.MVC.SharedResource> Localizer' not in content:
        content = '@using Microsoft.AspNetCore.Mvc.Localization\n@inject IHtmlLocalizer<Delivery.MVC.SharedResource> Localizer\n' + content

    # Since regex is dangerous for HTML, we'll print instructions to do it safely or do basic replacements
    replacements = {
        'Registro de Repartidor': '@Localizer["RegRepartidorTitulo"]',
        'Completa el formulario para unirte como repartidor de RayoExpres. Tu cuenta será revisada antes de ser activada.': '@Localizer["RegRepartidorDesc"]',
        'Volver a selección de tipo de cuenta': '@Localizer["VolverSeleccion"]',
        'Datos Personales': '@Localizer["DatosPersonales"]',
        '>Nombre<': '>@Localizer["Nombres"]<',
        'placeholder="Tu nombre"': 'placeholder="@Localizer["TuNombrePlaceholder"]"',
        '>Apellidos<': '>@Localizer["Apellidos"]<',
        'placeholder="Tus apellidos"': 'placeholder="@Localizer["TusApellidosPlaceholder"]"',
        '>Correo Electrónico<': '>@Localizer["CorreoElectronico"]<',
        'placeholder="correo@ejemplo.com"': 'placeholder="@Localizer["EmailPlaceholder"]"',
        '>Teléfono<': '>@Localizer["Telefono"]<',
        'placeholder="0991234567"': 'placeholder="0991234567"',
        '>Cédula de Identidad<': '>@Localizer["Cedula"]<',
        '>Contraseña<': '>@Localizer["Contrasena"]<',
        '>Confirmar Contraseña<': '>@Localizer["ConfirmarContrasena"]<',
        'Datos del Vehículo': '@Localizer["DatosVehiculo"]',
        '>Tipo de Vehículo<': '>@Localizer["TipoVehiculo"]<',
        '>Confirmar Tipo<': '>@Localizer["ConfirmarTipo"]<',
        'Selecciona y haz clic en "Confirmar Tipo" para actualizar los campos requeridos.': '@Localizer["ConfirmarTipoDesc"]',
        'Datos de Licencia (Requerido para': '@Localizer["DatosLicenciaRequerido"] (Requerido para',
        '>Número de Licencia de Conducir<': '>@Localizer["NumeroLicencia"]<',
        '>Fotografía de la Licencia<': '>@Localizer["FotoLicencia"]<',
        'Adjunta una foto clara de tu licencia de conducir.': '@Localizer["FotoLicenciaDesc"]',
        '>Marca<': '>@Localizer["MarcaVehiculo"]<',
        '>Modelo<': '>@Localizer["ModeloVehiculo"]<',
        '>Color<': '>@Localizer["ColorVehiculo"]<',
        '>Placa<': '>@Localizer["PlacaVehiculo"]<',
        '>Año del Vehículo<': '>@Localizer["AnioVehiculo"]<',
        '>Registrarme como Repartidor<': '>@Localizer["RegRepartidor"]<'
    }

    for k, v in replacements.items():
        content = content.replace(k, v)

    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)

process_file('Delivery.MVC/Views/RegistroRepartidor/Index.cshtml')
