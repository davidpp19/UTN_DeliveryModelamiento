using System;
using System.Threading.Tasks;
using Delivery.Modelos.DTOs;
using Delivery.Modelos.Enums;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class PasarelaPagoService : IPasarelaPagoService
    {
        // En el futuro, aquí se inyectarán los clientes de Stripe, Kushki, DataFast, etc.

        public PasarelaPagoService()
        {
        }

        public async Task<RespuestaPagoDto> ProcesarPagoAsync(SolicitudPagoDto solicitud)
        {
            // Simulación arquitectónica (Facade)
            // Lógica para derivar el pago a la pasarela correspondiente según solicitud.MetodoPago
            
            /*
            switch (solicitud.MetodoPago)
            {
                case TipoMetodoPagoEnum.Tarjeta:
                    // return await _stripeClient.ChargeAsync(...)
                case TipoMetodoPagoEnum.Transferencia:
                    // return await _bancoClient.VerifyAsync(...)
            }
            */

            return await Task.FromResult(new RespuestaPagoDto 
            {
                Exitoso = true,
                TransaccionId = Guid.NewGuid().ToString(),
                Estado = EstadoPagoEnum.Completado,
                MensajeError = string.Empty
            });
        }

        public async Task<RespuestaPagoDto> ReembolsarPagoAsync(string transaccionId, decimal monto)
        {
            // Lógica para contactar a la pasarela y hacer el refund
            return await Task.FromResult(new RespuestaPagoDto 
            {
                Exitoso = true,
                TransaccionId = transaccionId,
                Estado = EstadoPagoEnum.Reembolsado,
                MensajeError = string.Empty
            });
        }
    }
}
