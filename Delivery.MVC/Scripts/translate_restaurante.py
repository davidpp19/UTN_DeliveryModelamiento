import os
import re

def process_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    if '@inject IHtmlLocalizer<Delivery.MVC.SharedResource> Localizer' not in content:
        content = '@using Microsoft.AspNetCore.Mvc.Localization\n@inject IHtmlLocalizer<Delivery.MVC.SharedResource> Localizer\n' + content

    replacements = {
        'Registra tu Restaurante en RayoExpres': '@Localizer["RegRestauranteTitulo"]',
        'Datos del Propietario': '@Localizer["DatosPropietario"]',
        '>Nombres<': '>@Localizer["Nombres"]<',
        '>Apellidos<': '>@Localizer["Apellidos"]<',
        '>Correo Electrónico<': '>@Localizer["CorreoElectronico"]<',
        '>Teléfono<': '>@Localizer["Telefono"]<',
        '>Cédula<': '>@Localizer["Cedula"]<',
        '>Contraseña<': '>@Localizer["Contrasena"]<',
        '>Confirmar Contraseña<': '>@Localizer["ConfirmarContrasena"]<',
        'Datos del Restaurante': '@Localizer["DatosRestaurante"]',
        '>Nombre del Restaurante<': '>@Localizer["NombreRestaurante"]<',
        '>Descripción<': '>@Localizer["Descripcion"]<',
        '>RUC<': '>@Localizer["RUC"]<',
        'Categoría (ej: Pizza, Sushi)': '@Localizer["CategoriaEjemplo"]',
        '>Hora de Apertura<': '>@Localizer["HoraApertura"]<',
        '>Hora de Cierre<': '>@Localizer["HoraCierre"]<',
        '>Ubicación<': '>@Localizer["Ubicacion"]<',
        'Busca tu dirección o arrastra el marcador en el mapa.': '@Localizer["BuscaDireccionMapa"]',
        'Buscar Dirección': '@Localizer["BuscarDireccion"]',
        'Buscar...': '@Localizer["BuscarPlaceholder"]',
        'Detalles de Ubicación': '@Localizer["DetallesUbicacion"]',
        '>Calle<': '>@Localizer["Calle"]<',
        '>Ciudad<': '>@Localizer["Ciudad"]<',
        '>Código Postal<': '>@Localizer["CodigoPostal"]<',
        '>Logo del Restaurante<': '>@Localizer["LogoRestaurante"]<',
        'Registrar Restaurante': '@Localizer["RegRestauranteBtn"]'
    }

    for k, v in replacements.items():
        content = content.replace(k, v)

    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)

process_file('Delivery.MVC/Views/RegistroRestaurante/Index.cshtml')
