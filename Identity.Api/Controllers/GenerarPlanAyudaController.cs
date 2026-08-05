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
    public class GenerarPlanAyudaController : Controller
    {
        private readonly IGenerarPlanAyuda _plan;

        public GenerarPlanAyudaController(IGenerarPlanAyuda plan)
        {
            _plan = plan;
        }

        [HttpGet("GetBeneficiariosPorAfiliado/{ciAfiliado}")]
        public IActionResult GetBeneficiariosPorAfiliado(string ciAfiliado)
        {
            var lista = _plan.GetBeneficiariosPorAfiliado(ciAfiliado);
            return Ok(lista);
        }

        [HttpGet("YaFueGenerado/{beneficiario}")]
        public IActionResult YaFueGenerado(string beneficiario)
        {
            return Ok(_plan.YaFueGenerado(beneficiario));
        }

        [HttpPost("GenerarPlanAyuda")]
        public IActionResult GenerarPlanAyuda([FromBody] GenerarPlanAyudaRequestDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Beneficiario) || string.IsNullOrEmpty(request.CiAfiliado))
                    return BadRequest("Beneficiario y afiliado son obligatorios.");
                if (request.Valor <= 0)
                    return BadRequest("El valor debe ser mayor a cero.");

                var resultado = _plan.GenerarPlanAyuda(request);
                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al generar el plan de ayuda: " + ex.Message);
            }
        }
    }
}
