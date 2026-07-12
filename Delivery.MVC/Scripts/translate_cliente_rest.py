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

# ----------------- ClienteFavoritos/Index.cshtml -----------------
fav_idx_reps = {
    '"ClienteFavoritos"': 'Localizer["MisFavoritos"]',
    '>ClienteFavoritos<': '>@Localizer["MisFavoritos"]<',
    '>Crear Nuevo<': '>@Localizer["CrearNuevoBtn"]<',
    '>UsuarioId<': '>@Localizer["UsuarioIdTag"]<',
    '>RestauranteId<': '>@Localizer["RestauranteIdTag"]<',
    '>FechaAgregado<': '>@Localizer["FechaAgregadoTag"]<',
    '>Usuario<': '>@Localizer["UsuarioTag"]<',
    '>Restaurante<': '>@Localizer["RestauranteTag"]<',
    '>Acciones<': '>@Localizer["AccionesTag"]<',
    '>Editar<': '>@Localizer["EditarBtn"]<',
    '>Detalles<': '>@Localizer["DetallesBtn"]<',
    '>Eliminar<': '>@Localizer["EliminarBtn"]<'
}
process_file('Delivery.MVC/Views/ClienteFavoritos/Index.cshtml', fav_idx_reps)

# ----------------- ClienteFavoritos/Delete.cshtml -----------------
fav_del_reps = {
    '"Eliminar Favorito"': 'Localizer["EliminarFavorito"]',
    '>¿Estás seguro de que quieres eliminar esto?<': '>@Localizer["SeguroEliminarFavorito"]<',
    '>Eliminar<': '>@Localizer["EliminarBtn"]<',
    '>Cancelar<': '>@Localizer["CancelarBtn"]<'
}
process_file('Delivery.MVC/Views/ClienteFavoritos/Delete.cshtml', fav_del_reps)

# ----------------- ClienteHistorialPedidos/Calificar.cshtml -----------------
hist_cal_reps = {
    '"Calificar Pedido"': 'Localizer["CalificarPedido"]',
    '> Calificar Pedido #': '> @Localizer["CalificarPedido"] #',
    '>Calificación (1 a 5 estrellas)<': '>@Localizer["CalificacionEstrellasTag"]<',
    '>5 - Excelente<': '>@Localizer["CincoExcelente"]<',
    '>4 - Muy Bueno<': '>@Localizer["CuatroMuyBueno"]<',
    '>3 - Bueno<': '>@Localizer["TresBueno"]<',
    '>2 - Regular<': '>@Localizer["DosRegular"]<',
    '>1 - Malo<': '>@Localizer["UnoMalo"]<',
    '>Comentario (Opcional)<': '>@Localizer["ComentarioOpcionalTag"]<',
    'placeholder="¿Qué te pareció la comida y el servicio?"': 'placeholder="@Localizer["QueTeParecioComidaPlaceholder"]"',
    '>Enviar Calificación<': '>@Localizer["EnviarCalificacionBtn"]<',
    '>Cancelar<': '>@Localizer["CancelarBtn"]<'
}
process_file('Delivery.MVC/Views/ClienteHistorialPedidos/Calificar.cshtml', hist_cal_reps)

# ----------------- ClienteHistorialPedidos/Details.cshtml -----------------
hist_det_reps = {
    '"Detalle del Pedido"': 'Localizer["DetallePedido"]',
    '>Detalles de tu Pedido #': '>@Localizer["DetallesTuPedido"] #',
    '>Información General<': '>@Localizer["InformacionGeneralTag"]<',
    '>Restaurante<': '>@Localizer["RestauranteTag"]<',
    '>Fecha Pedido<': '>@Localizer["FechaPedidoTag"]<',
    '>Estado<': '>@Localizer["EstadoTag"]<',
    '>Dirección<': '>@Localizer["DireccionTag"]<',
    '>Método de Pago<': '>@Localizer["MetodoPagoTag"]<',
    '>Repartidor<': '>@Localizer["RepartidorTag"]<',
    'Licencia:': '@Localizer["LicenciaTag"]:',
    '>Notas<': '>@Localizer["NotasTag"]<',
    '>Resumen del Pedido<': '>@Localizer["ResumenPedidoTag"]<',
    '>Producto<': '>@Localizer["ProductoCol"]<',
    '>Cant.<': '>@Localizer["CantCol"]<',
    '>Precio<': '>@Localizer["PrecioCol"]<',
    '>Subtotal<': '>@Localizer["SubtotalCol"]<',
    'Producto ID': '@Localizer["ProductoIDTag"]',
    '>Costo de Envío<': '>@Localizer["CostoEnvio"]<',
    '>Cupón Aplicado<': '>@Localizer["CuponAplicadoTag"]<',
    '>Total Pagado<': '>@Localizer["TotalPagadoTag"]<',
    '>Volver al Historial<': '>@Localizer["VolverHistorialBtn"]<'
}
process_file('Delivery.MVC/Views/ClienteHistorialPedidos/Details.cshtml', hist_det_reps)

# ----------------- ClienteHistorialPedidos/Index.cshtml -----------------
hist_idx_reps = {
    '"Mis Pedidos"': 'Localizer["MisPedidos"]',
    '> Mis Pedidos<': '> @Localizer["MisPedidos"]<',
    'Revisa el historial de tus compras y su estado.': '@Localizer["RevisaHistorialDesc"]',
    '>Filtro Estado<': '>@Localizer["FiltroEstadoTag"]<',
    '>Todos<': '>@Localizer["TodosTag"]<',
    'Filtrar': '@Localizer["FiltrarBtn"]',
    'No se encontraron pedidos.': '@Localizer["NoSeEncontraronPedidos"]',
    'No tienes pedidos registrados todavía.': '@Localizer["NoTienesPedidosRegistrados"]',
    'Explorar Restaurantes': '@Localizer["ExplorarRestaurantesBtn"]',
    '>Pedido #': '>@Localizer["PedidoTag"] #',
    'Artículos:': '@Localizer["ArticulosTag"]:',
    'Total:': '@Localizer["TotalTag"]:',
    'Ver Detalles': '@Localizer["VerDetallesBtn"]',
    '¡Calificar!': '@Localizer["CalificarBtn"]'
}
process_file('Delivery.MVC/Views/ClienteHistorialPedidos/Index.cshtml', hist_idx_reps)

# ----------------- ClientePerfil/CompletarPerfil.cshtml -----------------
perfil_reps = {
    '"Completar Perfil"': 'Localizer["CompletarPerfil"]',
    '>Completar Mi Perfil<': '>@Localizer["CompletarMiPerfil"]<',
    '>Subir Foto de Perfil (Opcional)<': '>@Localizer["SubirFotoPerfilTag"]<',
    '>Nombres<': '>@Localizer["NombresTag"]<',
    '>Apellidos<': '>@Localizer["ApellidosTag"]<',
    '>Teléfono<': '>@Localizer["TelefonoTag"]<',
    '>Cédula<': '>@Localizer["CedulaTag"]<',
    '>Guardar Perfil<': '>@Localizer["GuardarPerfilBtn"]<',
    'Seleccionar archivo': '@Localizer["SeleccionarArchivoBtn"]'
}
process_file('Delivery.MVC/Views/ClientePerfil/CompletarPerfil.cshtml', perfil_reps)
