using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CuponesController : Controller
    {
        private readonly ICuponConsumer _consumer;

        public CuponesController(ICuponConsumer consumer)
        {
            _consumer = consumer;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _consumer.GetAllAsync();
            return View(data);
        }

        public async Task<IActionResult> Details(long id)
        {
            var data = await _consumer.GetByIdAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cupon entity, int duracionDias)
        {
            if (duracionDias != 7 && duracionDias != 14) duracionDias = 7;
            entity.FechaInicio = DateTime.UtcNow;
            entity.FechaFin = DateTime.UtcNow.AddDays(duracionDias);
            entity.Codigo = entity.Codigo?.Trim().ToUpper() ?? string.Empty;
            
            // Regla de negocio: Si no hay un usuario exclusivo asignado, el cupón DEBE ser público
            // para evitar que se vuelva un "cupón fantasma" que nadie pueda usar.
            if (!entity.UsuarioExclusivoId.HasValue)
            {
                entity.EsPublico = true;
            }
            
            await _consumer.CreateAsync(entity);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var data = await _consumer.GetByIdAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(long id, Cupon entity, int duracionDias)
        {
            var data = await _consumer.GetByIdAsync(id);
            if (data == null) return NotFound();

            data.Codigo = entity.Codigo?.Trim().ToUpper() ?? string.Empty;
            data.Descripcion = entity.Descripcion;
            data.TipoDescuento = entity.TipoDescuento;
            data.ValorDescuento = entity.ValorDescuento;
            data.DescuentoMaximo = entity.DescuentoMaximo;
            data.PedidoMinimo = entity.PedidoMinimo;
            data.LimiteUsos = entity.LimiteUsos;
            data.Activo = entity.Activo;
            data.EsPublico = entity.EsPublico;
            
            if (duracionDias == 7 || duracionDias == 14) 
            {
                data.FechaFin = data.FechaInicio.AddDays(duracionDias);
            }

            if (!data.UsuarioExclusivoId.HasValue)
            {
                data.EsPublico = true;
            }

            await _consumer.UpdateAsync(id, data);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(long id)
        {
            var data = await _consumer.GetByIdAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await _consumer.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
