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

# ---------------- AdminAprobaciones/Repartidores.cshtml ----------------
rep_aprob_reps = {
    '"Aprobaciones de Repartidores"': 'Localizer["AprobacionesRepartidores"]',
    '>Solicitudes de Repartidores Pendientes<': '>@Localizer["SolicitudesRepartidoresPendientes"]<',
    '>No hay solicitudes pendientes en este momento.<': '>@Localizer["NoHaySolicitudesPendientes"]<',
    '>Perfil<': '>@Localizer["PerfilTag"]<',
    '>Nombre<': '>@Localizer["NombreTag"]<',
    '>Email<': '>@Localizer["EmailTag"]<',
    '>Cédula<': '>@Localizer["CedulaTag"]<',
    '>Fecha Registro<': '>@Localizer["FechaRegistroTag"]<',
    '>Acciones<': '>@Localizer["AccionesTag"]<',
    '> Mostrar información<': '> @Localizer["MostrarInformacionBtn"]<',
    '> Aprobar<': '> @Localizer["AprobarBtn"]<',
    '> Denegar<': '> @Localizer["DenegarBtn"]<',
    '>Información del Repartidor<': '>@Localizer["InfoRepartidorTitle"]<',
    '>Datos Personales<': '>@Localizer["DatosPersonalesTitle"]<',
    '>Teléfono:<': '>@Localizer["TelefonoTag"]:<',
    '>Datos del Vehículo<': '>@Localizer["DatosVehiculoTitle"]<',
    '>Tipo:<': '>@Localizer["TipoVehiculoTag"]:<',
    '>Placa:<': '>@Localizer["PlacaTag"]:<',
    '>Licencia:<': '>@Localizer["LicenciaTag"]:<',
    '>Foto de Licencia:<': '>@Localizer["FotoLicenciaTag"]:<',
    '>Cerrar<': '>@Localizer["CerrarBtn"]<',
    '>Denegar Solicitud<': '>@Localizer["DenegarSolicitudTitle"]<',
    '¿Estás seguro que deseas denegar la solicitud de': '@Localizer["SeguroDenegarSolicitud"]',
    '>Motivo del rechazo (obligatorio)<': '>@Localizer["MotivoRechazoObligatorio"]<',
    '>Cancelar<': '>@Localizer["CancelarBtn"]<',
    '>Confirmar Denegación<': '>@Localizer["ConfirmarDenegacionBtn"]<'
}
process_file('Delivery.MVC/Views/AdminAprobaciones/Repartidores.cshtml', rep_aprob_reps)

# ---------------- AdminAprobaciones/Restaurantes.cshtml ----------------
rest_aprob_reps = {
    '"Aprobaciones de Restaurantes"': 'Localizer["AprobacionesRestaurantes"]',
    '>Solicitudes de Restaurantes Pendientes<': '>@Localizer["SolicitudesRestaurantesPendientes"]<',
    '>No hay solicitudes pendientes en este momento.<': '>@Localizer["NoHaySolicitudesPendientes"]<',
    '>Logo<': '>@Localizer["LogoTag"]<',
    '>Restaurante<': '>@Localizer["RestauranteTag"]<',
    '>Categoría<': '>@Localizer["CategoriaTag"]<',
    '>Propietario<': '>@Localizer["PropietarioTag"]<',
    '>Fecha Registro<': '>@Localizer["FechaRegistroTag"]<',
    '>Acciones<': '>@Localizer["AccionesTag"]<',
    '> Mostrar información<': '> @Localizer["MostrarInformacionBtn"]<',
    '> Aprobar<': '> @Localizer["AprobarBtn"]<',
    '> Denegar<': '> @Localizer["DenegarBtn"]<',
    '>Información del Restaurante<': '>@Localizer["InfoRestauranteTitle"]<',
    '>Datos del Establecimiento<': '>@Localizer["DatosEstablecimientoTitle"]<',
    '>RUC:<': '>@Localizer["RUCTag"]:<',
    '>Dirección:<': '>@Localizer["DireccionTag"]:<',
    '>Teléfono:<': '>@Localizer["TelefonoTag"]:<',
    '>Email:<': '>@Localizer["EmailTag"]:<',
    '>Descripción:<': '>@Localizer["DescripcionTag"]:<',
    '>Datos del Propietario<': '>@Localizer["DatosPropietarioTitle"]<',
    '>Email Propietario:<': '>@Localizer["EmailPropietarioTag"]:<',
    '>Cerrar<': '>@Localizer["CerrarBtn"]<',
    '>Denegar Solicitud<': '>@Localizer["DenegarSolicitudTitle"]<',
    '¿Estás seguro que deseas denegar la solicitud del restaurante': '@Localizer["SeguroDenegarSolicitudRest"]',
    '>Motivo del rechazo (obligatorio)<': '>@Localizer["MotivoRechazoObligatorio"]<',
    '>Cancelar<': '>@Localizer["CancelarBtn"]<',
    '>Confirmar Denegación<': '>@Localizer["ConfirmarDenegacionBtn"]<'
}
process_file('Delivery.MVC/Views/AdminAprobaciones/Restaurantes.cshtml', rest_aprob_reps)

# ---------------- AdminCupones/Index.cshtml ----------------
admin_cupones_reps = {
    '"Gestión de Cupones"': 'Localizer["GestionCupones"]',
    '>Cupones del Sistema<': '>@Localizer["CuponesSistema"]<',
    '>Código<': '>@Localizer["CodigoTag"]<',
    '>Descuento<': '>@Localizer["DescuentoTag"]<',
    '>Activo<': '>@Localizer["ActivoTag"]<',
    '>Sí<': '>@Localizer["SiTag"]<',
    '>No<': '>@Localizer["NoTag"]<'
}
process_file('Delivery.MVC/Views/AdminCupones/Index.cshtml', admin_cupones_reps)

# ---------------- AdminPedidos/Index.cshtml ----------------
admin_pedidos_reps = {
    '"Gestión de Pedidos"': 'Localizer["GestionPedidos"]',
    '>Todos los Pedidos<': '>@Localizer["TodosLosPedidos"]<',
    '>Cliente ID<': '>@Localizer["ClienteIDTag"]<',
    '>Restaurante ID<': '>@Localizer["RestauranteIDTag"]<',
    '>Estado<': '>@Localizer["EstadoTag"]<',
    '>Total<': '>@Localizer["TotalTag"]<'
}
process_file('Delivery.MVC/Views/AdminPedidos/Index.cshtml', admin_pedidos_reps)

# ---------------- AdminProductos/Index.cshtml ----------------
admin_prods_reps = {
    '"Gestión de Productos"': 'Localizer["GestionProductos"]',
    '>Todos los Productos<': '>@Localizer["TodosLosProductos"]<',
    '>Nombre<': '>@Localizer["NombreTag"]<',
    '>Restaurante ID<': '>@Localizer["RestauranteIDTag"]<',
    '>Precio<': '>@Localizer["PrecioTag"]<'
}
process_file('Delivery.MVC/Views/AdminProductos/Index.cshtml', admin_prods_reps)

# ---------------- AdminRepartidores/Index.cshtml ----------------
admin_reps_reps = {
    '"Gestión de Repartidores"': 'Localizer["GestionRepartidores"]',
    '>Repartidores Registrados<': '>@Localizer["RepartidoresRegistrados"]<',
    'Vista básica para el administrador. Para aprobar o rechazar, ir a Solicitudes.': '@Localizer["VistaBasicaAdminParaAprobarRep"]',
    '>Licencia<': '>@Localizer["LicenciaTag"]<',
    '>Estado<': '>@Localizer["EstadoTag"]<'
}
process_file('Delivery.MVC/Views/AdminRepartidores/Index.cshtml', admin_reps_reps)

# ---------------- AdminRestaurantes/Index.cshtml ----------------
admin_rest_reps = {
    '"Gestión de Restaurantes"': 'Localizer["GestionRestaurantes"]',
    '>Restaurantes Registrados<': '>@Localizer["RestaurantesRegistrados"]<',
    'Vista básica para el administrador. Para aprobar o rechazar, ir a Solicitudes.': '@Localizer["VistaBasicaAdminParaAprobarRest"]',
    '>Nombre Comercial<': '>@Localizer["NombreComercialTag"]<',
    '>Dirección<': '>@Localizer["DireccionTag"]<',
    '>Estado<': '>@Localizer["EstadoTag"]<'
}
process_file('Delivery.MVC/Views/AdminRestaurantes/Index.cshtml', admin_rest_reps)

# ---------------- AdminUsuarios/Index.cshtml ----------------
admin_users_reps = {
    '"Gestión de Usuarios"': 'Localizer["GestionUsuarios"]',
    '>Usuarios del Sistema<': '>@Localizer["UsuariosSistema"]<',
    'Esta es una vista básica de solo lectura para evitar errores 404.': '@Localizer["VistaBasicaLecturaUsuarios"]',
    '>Nombre<': '>@Localizer["NombreTag"]<',
    '>Email<': '>@Localizer["EmailTag"]<'
}
process_file('Delivery.MVC/Views/AdminUsuarios/Index.cshtml', admin_users_reps)

# ---------------- DashboardAdministrador/Index.cshtml ----------------
dashboard_admin_reps = {
    '"Panel de Administrador"': 'Localizer["PanelAdministrador"]',
    '> Panel de Administración<': '> @Localizer["PanelAdministracion"]<',
    'Bienvenido al centro de control de RayoExpres. Desde aquí puedes gestionar todos los módulos del sistema.': '@Localizer["BienvenidoCentroControl"]',
    '>Usuarios<': '>@Localizer["UsuariosCardTitle"]<',
    'Gestiona los usuarios, clientes, y administradores.': '@Localizer["GestionaUsuariosDesc"]',
    '>Ir a Usuarios<': '>@Localizer["IrAUsuariosBtn"]<',
    '>Restaurantes<': '>@Localizer["RestaurantesCardTitle"]<',
    'Administra los restaurantes afiliados al sistema.': '@Localizer["AdministraRestaurantesDesc"]',
    '>Ir a Restaurantes<': '>@Localizer["IrARestaurantesBtn"]<',
    '>Productos<': '>@Localizer["ProductosCardTitle"]<',
    'Control general del catálogo de productos.': '@Localizer["ControlGeneralCatalogoDesc"]',
    '>Ir a Productos<': '>@Localizer["IrAProductosBtn"]<',
    '>Categorías<': '>@Localizer["CategoriasCardTitle"]<',
    'Gestión de categorías de productos.': '@Localizer["GestionCategoriasDesc"]',
    '>Ir a Categorías<': '>@Localizer["IrACategoriasBtn"]<',
    '>Pedidos<': '>@Localizer["PedidosCardTitle"]<',
    'Visualiza todos los pedidos generados.': '@Localizer["VisualizaPedidosDesc"]',
    '>Ir a Pedidos<': '>@Localizer["IrAPedidosBtn"]<',
    '>Repartidores<': '>@Localizer["RepartidoresCardTitle"]<',
    'Gestión de repartidores y sus estados.': '@Localizer["GestionRepartidoresEstadosDesc"]',
    '>Ir a Repartidores<': '>@Localizer["IrARepartidoresBtn"]<',
    '>Vehículos<': '>@Localizer["VehiculosCardTitle"]<',
    'Gestión de vehículos de los repartidores.': '@Localizer["GestionVehiculosDesc"]',
    '>Ir a Vehículos<': '>@Localizer["IrAVehiculosBtn"]<',
    '>Cupones<': '>@Localizer["CuponesCardTitle"]<',
    'Administración de promociones y cupones.': '@Localizer["AdministracionPromocionesDesc"]',
    '>Ir a Cupones<': '>@Localizer["IrACuponesBtn"]<',
    '>Pagos<': '>@Localizer["PagosCardTitle"]<',
    'Historial de transacciones y pagos.': '@Localizer["HistorialTransaccionesDesc"]',
    '>Ir a Pagos<': '>@Localizer["IrAPagosBtn"]<',
    '>Calificaciones<': '>@Localizer["CalificacionesCardTitle"]<',
    'Moderación de reseñas a restaurantes y repartidores.': '@Localizer["ModeracionResenasDesc"]',
    '>Ir a Reseñas<': '>@Localizer["IrAResenasBtn"]<',
    '>Auditoría<': '>@Localizer["AuditoriaCardTitle"]<',
    'Registros de acciones importantes en el sistema.': '@Localizer["RegistrosAccionesDesc"]',
    '>Ir a Auditoría<': '>@Localizer["IrAAuditoriaBtn"]<'
}
process_file('Delivery.MVC/Views/DashboardAdministrador/Index.cshtml', dashboard_admin_reps)
