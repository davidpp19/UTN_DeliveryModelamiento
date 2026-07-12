import os
import re

controllers = [
    'ClienteCarritoController.cs',
    'ClienteCuponesController.cs',
    'ClienteDireccionesController.cs',
    'ClienteFavoritosController.cs',
    'ClienteHistorialPedidosController.cs',
    'ClientePerfilController.cs'
]
base_path = 'Delivery.MVC/Controllers'

def inject_localizer(content, class_name):
    # Add using
    if 'using Microsoft.Extensions.Localization;' not in content:
        content = content.replace('using Microsoft.AspNetCore.Mvc;', 'using Microsoft.AspNetCore.Mvc;\nusing Microsoft.Extensions.Localization;')
    
    # Check if localizer already injected
    if 'IStringLocalizer<SharedResource> _localizer' in content:
        return content
    
    # Find constructor
    constructor_regex = r'(public\s+' + class_name + r'\s*\([^)]*\))\s*\{'
    match = re.search(constructor_regex, content)
    if not match: return content
    
    constructor_sig = match.group(1)
    # Inject field before constructor
    content = content.replace(constructor_sig, 
        'private readonly IStringLocalizer<SharedResource> _localizer;\n\n        ' + constructor_sig)
    
    # Modify constructor signature
    if constructor_sig.endswith('()'):
        new_sig = constructor_sig[:-1] + 'IStringLocalizer<SharedResource> localizer)'
    else:
        new_sig = constructor_sig + ', IStringLocalizer<SharedResource> localizer)'
    
    content = content.replace(constructor_sig, new_sig)
    
    # Modify constructor body
    body_start = content.find('{', content.find(new_sig))
    if body_start != -1:
        content = content[:body_start+1] + '\n            _localizer = localizer;' + content[body_start+1:]
        
    return content

def translate_messages(content):
    # This is a bit tricky for interpolated strings ($"...") vs normal strings ("...").
    # For now, let's just do normal strings assigned to TempData or ModelState.
    # We will use simple replace for the known exact strings.
    reps = {
        '"No se pudo agregar el producto. Puede que debas vaciar el carrito si es de otro restaurante."': '_localizer["ErrorAgregarProducto"]',
        '"Producto agregado al carrito."': '_localizer["ExitoAgregarProducto"]',
        '"Tu carrito está vacío."': '_localizer["CarritoVacioError"]',
        '"Necesitas registrar una dirección antes de confirmar tu pedido."': '_localizer["RegistrarDireccionError"]',
        '"Debes proporcionar una ubicación válida en el mapa."': '_localizer["UbicacionValidaError"]',
        '"No se pudo guardar la ubicación en el sistema."': '_localizer["ErrorGuardarUbicacion"]',
        '"Debes seleccionar una dirección de entrega válida."': '_localizer["DireccionInvalidaError"]',
        '"Todos los campos de la tarjeta son obligatorios."': '_localizer["CamposTarjetaError"]',
        '"El número de tarjeta no es válido (Validación Luhn fallida)."': '_localizer["TarjetaInvalidaError"]',
        '"La tarjeta no es Visa, Mastercard, Amex ni Discover."': '_localizer["TarjetaNoSoportadaError"]',
        '"Hubo un problema al procesar tu pedido. Por favor intenta de nuevo."': '_localizer["ErrorProcesarPedido"]',
        '"Debes ingresar un código."': '_localizer["IngresarCodigoError"]',
        '"El cupón se encuentra desactivado."': '_localizer["CuponDesactivadoError"]',
        '"El cupón ha expirado."': '_localizer["CuponExpiradoError"]',
        '"Este cupón ya fue utilizado."': '_localizer["CuponUtilizadoError"]',
        '"Este cupón ya fue agregado."': '_localizer["CuponAgregadoError"]',
        '"Dirección eliminada correctamente."': '_localizer["DireccionEliminadaExito"]',
        '"No se puede eliminar la dirección porque está asociada a uno o más pedidos en el historial."': '_localizer["DireccionAsociadaPedidoError"]',
        '"Perfil actualizado exitosamente."': '_localizer["PerfilActualizadoExito"]'
    }
    
    for k, v in reps.items():
        content = content.replace(k, v)
        
    return content

for c in controllers:
    path = os.path.join(base_path, c)
    if not os.path.exists(path): continue
    with open(path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    class_name = c.replace('.cs', '')
    content = inject_localizer(content, class_name)
    content = translate_messages(content)
    
    with open(path, 'w', encoding='utf-8') as f:
        f.write(content)
