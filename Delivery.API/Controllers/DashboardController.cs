using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.DTOs;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Authorize(Roles = "Admin,Gerente")]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardEstadisticasDto>> GetEstadisticas()
        {
            var estadisticas = await _dashboardService.ObtenerEstadisticasAsync();
            return Ok(estadisticas);
        }
    }
}
