using Identity.Api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GeocodingController : Controller
    {
        private readonly IGeocoding _geocoding;

        public GeocodingController(IGeocoding geocoding)
        {
            _geocoding = geocoding;
        }

        [HttpGet("BuscarPredicciones")]
        public async Task<IActionResult> BuscarPredicciones(string query)
        {
            try
            {
                var resultados = await _geocoding.BuscarPredicciones(query);
                return Ok(resultados);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("ObtenerCoordenadasPorPlaceId")]
        public async Task<IActionResult> ObtenerCoordenadasPorPlaceId(string placeId)
        {
            try
            {
                var resultado = await _geocoding.ObtenerCoordenadasPorPlaceId(placeId);
                if (resultado == null)
                    return NotFound();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("Reverse")]
        public async Task<IActionResult> Reverse(decimal lat, decimal lon)
        {
            try
            {
                var direccion = await _geocoding.BuscarDireccionPorCoordenadas(lat, lon);
                return Ok(direccion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
