using System.Text.Json;
using Delivery.Modelos.DTOs;
using Microsoft.AspNetCore.Http;

namespace Delivery.MVC.Helpers
{
    /// <summary>
    /// Helper para leer/escribir el carrito en la Session del navegador.
    /// El carrito NUNCA se persiste en la base de datos mientras el usuario agrega productos.
    /// Solo se crea el Pedido real cuando el usuario confirma la compra.
    /// </summary>
    public static class CarritoSessionHelper
    {
        private const string CarritoKey = "Carrito";

        public static CarritoSesionDto? ObtenerCarrito(ISession session)
        {
            var json = session.GetString(CarritoKey);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonSerializer.Deserialize<CarritoSesionDto>(json);
        }

        public static void GuardarCarrito(ISession session, CarritoSesionDto carrito)
        {
            var json = JsonSerializer.Serialize(carrito);
            session.SetString(CarritoKey, json);
        }

        public static void LimpiarCarrito(ISession session)
        {
            session.Remove(CarritoKey);
        }
    }
}
