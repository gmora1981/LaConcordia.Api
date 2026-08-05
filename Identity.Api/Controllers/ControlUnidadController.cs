using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ControlUnidadController : Controller
    {
        private readonly IControlUnidad _controlUnidad;

        public ControlUnidadController(IControlUnidad controlUnidad)
        {
            _controlUnidad = controlUnidad;
        }

        [HttpGet("GetFichaPersonalPorServicio/{estadoServicio}")]
        public IActionResult GetFichaPersonalPorServicio(string estadoServicio)
        {
            return Ok(_controlUnidad.GetFichaPersonalPorServicio(estadoServicio));
        }

        [HttpPost("MoverUnidad")]
        public IActionResult MoverUnidad([FromBody] MoverUnidadRequestDTO request)
        {
            try
            {
                var monitora = User.Identity?.Name ?? "desconocido";
                _controlUnidad.MoverUnidad(request, monitora);
                return Ok("Movimiento registrado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al registrar el movimiento: " + ex.Message);
            }
        }
    }
}
