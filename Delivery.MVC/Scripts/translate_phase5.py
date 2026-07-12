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

# Home
home_reps = {
    '"Resultados de Búsqueda"': 'Localizer["ResultadosBusquedaTitle"]',
    '>Resultados para "@ViewBag.Query"<': '>@Localizer["ResultadosPara", ViewBag.Query]<',
    '>Restaurantes Encontrados (@restaurantes?.Count)<': '>@Localizer["RestaurantesEncontrados", restaurantes?.Count ?? 0]<',
    '>Ver Menú<': '>@Localizer["VerMenu"]<',
    '>No se encontraron restaurantes con ese término.<': '>@Localizer["NoRestaurantesBusqueda"]<',
    '>Productos Encontrados (@productos?.Count)<': '>@Localizer["ProductosEncontrados", productos?.Count ?? 0]<',
    '>No se encontraron productos con ese término.<': '>@Localizer["NoProductosBusqueda"]<',
    '"Cuenta en Revisión"': 'Localizer["CuentaEnRevisionTitle"]',
    '>Tu solicitud de registro ha sido recibida y se encuentra actualmente en estado <strong>Pendiente de Aprobación</strong>.<': '>@Localizer["SolicitudRecibida"]<',
    '>Un administrador revisará tu información en breve. Recibirás una notificación cuando tu cuenta sea aprobada y puedas acceder al panel de control.<': '>@Localizer["AdminRevisaraBreve"]<',
    '>Volver al Inicio<': '>@Localizer["VolverInicioBtn"]<',
    '>Entregas Rápidas<': '>@Localizer["EntregasRapidasTitle"]<',
    '>Nuestros repartidores están siempre cerca para que tu comida llegue caliente.<': '>@Localizer["EntregasRapidasDesc"]<',
    '>Pagos Seguros<': '>@Localizer["PagosSegurosTitle"]<',
    '>Múltiples métodos de pago avalados por estrictos estándares de seguridad.<': '>@Localizer["PagosSegurosDesc"]<',
    '>Los Mejores Locales<': '>@Localizer["MejoresLocalesTitle"]<',
    '>Los mejores restaurantes de Ibarra en un solo lugar.<': '>@Localizer["MejoresLocalesDesc"]<',
    '>¿Cómo funciona RayoExpres?<': '>@Localizer["ComoFuncionaTitle"]<',
    '>Busca<': '>@Localizer["BuscaTitle"]<',
    '>Encuentra el restaurante o platillo que deseas.<': '>@Localizer["BuscaDesc"]<',
    '>Pide<': '>@Localizer["PideTitle"]<',
    '>Agrega a tu carrito y aplica tus cupones.<': '>@Localizer["PideDesc"]<',
    '>Paga<': '>@Localizer["PagaTitle"]<',
    '>Usa efectivo, tarjeta o transferencia de forma segura.<': '>@Localizer["PagaDesc"]<',
    '>Disfruta<': '>@Localizer["DisfrutaTitle"]<',
    '>Recibe tu pedido en minutos.<': '>@Localizer["DisfrutaDesc"]<',
    '>Restaurantes Aliados en Ibarra<': '>@Localizer["RestaurantesAliadosTitle"]<',
    '>Inicia sesión para ver el menú<': '>@Localizer["IniciaSesionVerMenu"]<',
    '>No hay restaurantes disponibles en este momento.<': '>@Localizer["NoRestaurantesDisponibles"]<',
    '"Solicitud Rechazada"': 'Localizer["SolicitudRechazadaTitle"]',
    '>Lamentamos informarte que tu solicitud de registro ha sido <strong>rechazada</strong> por un administrador.<': '>@Localizer["SolicitudRechazadaDesc"]<',
    '>Para más información o si crees que esto es un error, por favor comunícate con soporte.<': '>@Localizer["RechazoSoporte"]<',
    '>Menú del Restaurante<': '>@Localizer["MenuDelRestaurante"]<',
    '>Añadir al Carrito<': '>@Localizer["AgregarAlCarrito"]<',
    '>Inicia sesión para pedir<': '>@Localizer["IniciaSesionPedir"]<',
    '>Este restaurante aún no tiene productos disponibles.<': '>@Localizer["NoProductosDisponiblesRest"]<'
}
replace_in_files('Home', home_reps)

# Auth
auth_reps = {
    '"Acceso Denegado"': 'Localizer["AccesoDenegadoTitle"]',
    '>No tienes los permisos necesarios para acceder a esta página.<': '>@Localizer["SinPermisosAcceso"]<',
    '>Volver al Inicio<': '>@Localizer["VolverInicioBtn"]<',
    '"Recuperar Contraseña"': 'Localizer["RecuperarContrasenaTitle"]',
    '>Ingresa tu correo electrónico y te enviaremos las instrucciones para restablecer tu contraseña.<': '>@Localizer["RecuperarDesc"]<',
    '>Enviar Instrucciones<': '>@Localizer["EnviarInstruccionesBtn"]<',
    '>Cancelar y volver<': '>@Localizer["CancelarVolverBtn"]<'
}
replace_in_files('Auth', auth_reps)
