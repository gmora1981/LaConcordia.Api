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

        [HttpGet("Buscar")]
        public async Task<IActionResult> Buscar(string query)
        {
            var resultados = await _geocoding.Buscar(query);
            return Ok(resultados);
        }
    }
}
