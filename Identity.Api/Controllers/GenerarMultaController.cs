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
    public class GenerarMultaController : Controller
    {
        private readonly IGenerarMulta _generarMulta;

        public GenerarMultaController(IGenerarMulta generarMulta)
        {
            _generarMulta = generarMulta;
        }

        [HttpGet("GetMultasPorSocio/{cidentidad}")]
        public IActionResult GetMultasPorSocio(string cidentidad)
        {
            var lista = _generarMulta.GetMultasPorSocio(cidentidad);
            return Ok(lista);
        }

        [HttpPost("InsertGenerarMulta")]
        public IActionResult Create([FromBody] GenerarMultaDTO nueva)
        {
            try
            {
                if (string.IsNullOrEmpty(nueva.Cidentidad))
                    return BadRequest("Cidentidad es obligatoria.");
                if (nueva.Valor == null || nueva.Valor <= 0)
                    return BadRequest("El valor de la multa debe ser mayor a cero.");

                var creada = _generarMulta.InsertGenerarMulta(nueva);
                return Ok(creada);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al registrar la multa: " + ex.Message);
            }
        }
    }
}
