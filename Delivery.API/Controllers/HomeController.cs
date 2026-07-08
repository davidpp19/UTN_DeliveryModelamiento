using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.DTOs;
using Delivery.Servicios.Interfaces;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        [HttpGet]
        public async Task<ActionResult<HomeResponseDto>> GetHomeData()
        {
            var data = await _homeService.ObtenerDatosHomeAsync();
            return Ok(data);
        }
    }
}
