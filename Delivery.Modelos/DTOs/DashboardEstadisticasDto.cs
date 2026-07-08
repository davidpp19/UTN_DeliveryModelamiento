using System.Collections.Generic;

namespace Delivery.Modelos.DTOs
{
    public class DashboardEstadisticasDto
    {
        public int UsuariosRegistrados { get; set; }
        public int RestaurantesActivos { get; set; }
        public int ProductosRegistrados { get; set; }
        public int ProductosDisponibles { get; set; }
        public int PedidosDelDia { get; set; }
        public int PedidosPendientes { get; set; }
        public int PedidosEntregados { get; set; }
        public int RepartidoresActivos { get; set; }
        public decimal VentasDelDia { get; set; }
        public decimal VentasDelMes { get; set; }
        public List<string> RestaurantesConMasPedidos { get; set; } = new List<string>();
        public List<string> ProductosMasVendidos { get; set; } = new List<string>();
        public List<string> ClientesConMasCompras { get; set; } = new List<string>();
    }
}
