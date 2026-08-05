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
    public class GenerarPlanChoqueController : Controller
    {
        private readonly IGenerarPlanChoque _plan;

        public GenerarPlanChoqueController(IGenerarPlanChoque plan)
        {
            _plan = plan;
        }

        [HttpGet("YaFueGenerado/{unidad}")]
        public IActionResult YaFueGenerado(string unidad)
        {
            return Ok(_plan.YaFueGenerado(unidad));
        }

        [HttpPost("GenerarPlanChoque")]
        public IActionResult GenerarPlanChoque([FromBody] GenerarPlanChoqueRequestDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Unidad))
                    return BadRequest("La unidad es obligatoria.");
                if (request.Valor <= 0)
                    return BadRequest("El valor debe ser mayor a cero.");

                var resultado = _plan.GenerarPlanChoque(request);
                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al generar el plan de choque: " + ex.Message);
            }
        }
    }
}
