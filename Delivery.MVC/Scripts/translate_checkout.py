import os

file_path = 'Delivery.MVC/Views/ClienteCarrito/Checkout.cshtml'

with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

if '@inject IHtmlLocalizer<Delivery.MVC.SharedResource> Localizer' not in content:
    content = '@using Microsoft.AspNetCore.Mvc.Localization\n@inject IHtmlLocalizer<Delivery.MVC.SharedResource> Localizer\n' + content

reps = {
    '"Checkout"': 'Localizer["CheckoutTitulo"]',
    '>Proceso de Pago<': '>@Localizer["ProcesoPago"]<',
    '>Tu Carrito<': '>@Localizer["TuCarrito"]<',
    'Cantidad: ': '@Localizer["Cantidad"] ',
    '>Subtotal<': '>@Localizer["Subtotal"]<',
    '>Distancia y Tiempo<': '>@Localizer["DistanciaTiempo"]<',
    '>Pendiente<': '>@Localizer["Pendiente"]<',
    '>Seleccione dirección en el mapa<': '>@Localizer["SeleccioneDireccionMapa"]<',
    '>Costo de Envío<': '>@Localizer["CostoEnvio"]<',
    '>Cupón aplicado<': '>@Localizer["CuponAplicado"]<',
    '>Total (USD)<': '>@Localizer["TotalUSD"]<',
    '>-- Seleccionar cupón --<': '>@Localizer["SeleccionarCupon"]<',
    '>Aplicar cupón<': '>@Localizer["AplicarCuponBtn"]<',
    '>Tarifa de envío:<': '>@Localizer["TarifaEnvioTag"]<',
    ' $1.50 (hasta 3 km). $0.25 por km adicional.': ' @Localizer["TarifaEnvioDesc"]',
    '>Dirección de Entrega<': '>@Localizer["DireccionEntrega"]<',
    '>Tus direcciones guardadas<': '>@Localizer["TusDirecciones"]<',
    '>-- Usar otra dirección (Buscar en el mapa) --<': '>@Localizer["UsarOtraDireccion"]<',
    'placeholder="🔍 Escribe para buscar una calle o lugar..."': 'placeholder="@Localizer["BuscarCallePlaceHolder"]"',
    '📍 Puedes arrastrar el marcador rojo para afinar tu ubicación exacta.': '@Localizer["ArrastrarMarcadorDesc"]',
    '>Detalles de la Ubicación<': '>@Localizer["DetallesUbicacion"]<',
    '>Calle / Avenida principal detectada<': '>@Localizer["CalleAvenidaDetectada"]<',
    '>Guardar esta dirección para mis próximas compras<': '>@Localizer["GuardarDireccionParaCompras"]<',
    '>Alias (Ej. Casa, Trabajo)<': '>@Localizer["AliasEjemplo"]<',
    'placeholder="Casa"': 'placeholder="@Localizer["CasaPlaceholder"]"',
    '>Referencia<': '>@Localizer["Referencia"]<',
    'placeholder="Frente al parque"': 'placeholder="@Localizer["FrenteParquePlaceholder"]"',
    '>Método de Pago<': '>@Localizer["MetodoPago"]<',
    '>Efectivo (Al recibir el pedido)<': '>@Localizer["EfectivoDesc"]<',
    '>Tarjeta de Crédito / Débito<': '>@Localizer["TarjetaDesc"]<',
    '>Transferencia Bancaria<': '>@Localizer["TransferenciaDesc"]<',
    '>Detalles de la Tarjeta<': '>@Localizer["DetallesTarjeta"]<',
    '>Nombre en la tarjeta<': '>@Localizer["NombreTarjeta"]<',
    'placeholder="EJ. JUAN PEREZ"': 'placeholder="@Localizer["EjemploJuanPerez"]"',
    '>Número de tarjeta ': '>@Localizer["NumeroTarjeta"] ',
    '>Expiración<': '>@Localizer["Expiracion"]<',
    '>CVV<': '>@Localizer["CVV"]<',
    '>Datos para Transferencia<': '>@Localizer["DatosTransferencia"]<',
    '>Por favor, realiza la transferencia a la siguiente cuenta y adjunta el comprobante (JPG, PNG, PDF).<': '>@Localizer["TransferenciaInstrucciones"]<',
    '>Banco:<': '>@Localizer["BancoTag"]<',
    '>Beneficiario:<': '>@Localizer["BeneficiarioTag"]<',
    '>RUC:<': '>@Localizer["RUCTag"]<',
    '>Tipo de Cuenta:<': '>@Localizer["TipoCuentaTag"]<',
    '>Corriente<': '>@Localizer["CorrienteDesc"]<',
    '>Número de cuenta:<': '>@Localizer["NumCuentaTag"]<',
    '>Subir Comprobante<': '>@Localizer["SubirComprobante"]<',
    '>Confirmar y Pagar<': '>@Localizer["ConfirmarPagarBtn"]<'
}

for k, v in reps.items():
    content = content.replace(k, v)

with open(file_path, 'w', encoding='utf-8') as f:
    f.write(content)
