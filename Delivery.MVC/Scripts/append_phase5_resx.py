import os

keys_es = {
    'ResultadosBusquedaTitle': 'Resultados de Búsqueda',
    'ResultadosPara': 'Resultados para "{0}"',
    'RestaurantesEncontrados': 'Restaurantes Encontrados ({0})',
    'VerMenu': 'Ver Menú',
    'NoRestaurantesBusqueda': 'No se encontraron restaurantes con ese término.',
    'ProductosEncontrados': 'Productos Encontrados ({0})',
    'NoProductosBusqueda': 'No se encontraron productos con ese término.',
    'CuentaEnRevisionTitle': 'Cuenta en Revisión',
    'SolicitudRecibida': 'Tu solicitud de registro ha sido recibida y se encuentra actualmente en estado <strong>Pendiente de Aprobación</strong>.',
    'AdminRevisaraBreve': 'Un administrador revisará tu información en breve. Recibirás una notificación cuando tu cuenta sea aprobada y puedas acceder al panel de control.',
    'VolverInicioBtn': 'Volver al Inicio',
    'EntregasRapidasTitle': 'Entregas Rápidas',
    'EntregasRapidasDesc': 'Nuestros repartidores están siempre cerca para que tu comida llegue caliente.',
    'PagosSegurosTitle': 'Pagos Seguros',
    'PagosSegurosDesc': 'Múltiples métodos de pago avalados por estrictos estándares de seguridad.',
    'MejoresLocalesTitle': 'Los Mejores Locales',
    'MejoresLocalesDesc': 'Los mejores restaurantes de Ibarra en un solo lugar.',
    'ComoFuncionaTitle': '¿Cómo funciona RayoExpres?',
    'BuscaTitle': 'Busca',
    'BuscaDesc': 'Encuentra el restaurante o platillo que deseas.',
    'PideTitle': 'Pide',
    'PideDesc': 'Agrega a tu carrito y aplica tus cupones.',
    'PagaTitle': 'Paga',
    'PagaDesc': 'Usa efectivo, tarjeta o transferencia de forma segura.',
    'DisfrutaTitle': 'Disfruta',
    'DisfrutaDesc': 'Recibe tu pedido en minutos.',
    'RestaurantesAliadosTitle': 'Restaurantes Aliados en Ibarra',
    'IniciaSesionVerMenu': 'Inicia sesión para ver el menú',
    'NoRestaurantesDisponibles': 'No hay restaurantes disponibles en este momento.',
    'SolicitudRechazadaTitle': 'Solicitud Rechazada',
    'SolicitudRechazadaDesc': 'Lamentamos informarte que tu solicitud de registro ha sido <strong>rechazada</strong> por un administrador.',
    'RechazoSoporte': 'Para más información o si crees que esto es un error, por favor comunícate con soporte.',
    'MenuDelRestaurante': 'Menú del Restaurante',
    'AgregarAlCarrito': 'Añadir al Carrito',
    'IniciaSesionPedir': 'Inicia sesión para pedir',
    'NoProductosDisponiblesRest': 'Este restaurante aún no tiene productos disponibles.',
    'AccesoDenegadoTitle': 'Acceso Denegado',
    'SinPermisosAcceso': 'No tienes los permisos necesarios para acceder a esta página.',
    'RecuperarContrasenaTitle': 'Recuperar Contraseña',
    'RecuperarDesc': 'Ingresa tu correo electrónico y te enviaremos las instrucciones para restablecer tu contraseña.',
    'EnviarInstruccionesBtn': 'Enviar Instrucciones',
    'CancelarVolverBtn': 'Cancelar y volver'
}

keys_en = {
    'ResultadosBusquedaTitle': 'Search Results',
    'ResultadosPara': 'Results for "{0}"',
    'RestaurantesEncontrados': 'Restaurants Found ({0})',
    'VerMenu': 'View Menu',
    'NoRestaurantesBusqueda': 'No restaurants found with that term.',
    'ProductosEncontrados': 'Products Found ({0})',
    'NoProductosBusqueda': 'No products found with that term.',
    'CuentaEnRevisionTitle': 'Account Under Review',
    'SolicitudRecibida': 'Your registration request has been received and is currently <strong>Pending Approval</strong>.',
    'AdminRevisaraBreve': 'An administrator will review your information shortly. You will receive a notification when your account is approved and you can access the dashboard.',
    'VolverInicioBtn': 'Back to Home',
    'EntregasRapidasTitle': 'Fast Deliveries',
    'EntregasRapidasDesc': 'Our drivers are always nearby so your food arrives hot.',
    'PagosSegurosTitle': 'Secure Payments',
    'PagosSegurosDesc': 'Multiple payment methods backed by strict security standards.',
    'MejoresLocalesTitle': 'The Best Spots',
    'MejoresLocalesDesc': 'The best restaurants in town in one place.',
    'ComoFuncionaTitle': 'How does RayoExpres work?',
    'BuscaTitle': 'Search',
    'BuscaDesc': 'Find the restaurant or dish you want.',
    'PideTitle': 'Order',
    'PideDesc': 'Add to your cart and apply your coupons.',
    'PagaTitle': 'Pay',
    'PagaDesc': 'Use cash, card or transfer safely.',
    'DisfrutaTitle': 'Enjoy',
    'DisfrutaDesc': 'Receive your order in minutes.',
    'RestaurantesAliadosTitle': 'Partner Restaurants',
    'IniciaSesionVerMenu': 'Log in to view the menu',
    'NoRestaurantesDisponibles': 'There are no restaurants available at this time.',
    'SolicitudRechazadaTitle': 'Request Rejected',
    'SolicitudRechazadaDesc': 'We regret to inform you that your registration request has been <strong>rejected</strong> by an administrator.',
    'RechazoSoporte': 'For more information or if you think this is an error, please contact support.',
    'MenuDelRestaurante': 'Restaurant Menu',
    'AgregarAlCarrito': 'Add to Cart',
    'IniciaSesionPedir': 'Log in to order',
    'NoProductosDisponiblesRest': 'This restaurant does not have products available yet.',
    'AccesoDenegadoTitle': 'Access Denied',
    'SinPermisosAcceso': 'You do not have the necessary permissions to access this page.',
    'RecuperarContrasenaTitle': 'Recover Password',
    'RecuperarDesc': 'Enter your email and we will send you instructions to reset your password.',
    'EnviarInstruccionesBtn': 'Send Instructions',
    'CancelarVolverBtn': 'Cancel and return'
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
