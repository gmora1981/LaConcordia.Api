using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SolicitudCarreraController : Controller
    {
        private readonly ISolicitudCarrera _solicitud;
        private readonly UserManager<ApplicationUser> _userManager;

        public SolicitudCarreraController(ISolicitudCarrera solicitud, UserManager<ApplicationUser> userManager)
        {
            _solicitud = solicitud;
            _userManager = userManager;
        }

        private async Task<string?> ObtenerRucLogueado()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return null;

            var user = await _userManager.FindByNameAsync(username);
            return user?.Ruc;
        }

        [HttpPost("CrearSolicitud")]
        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> CrearSolicitud([FromBody] CrearSolicitudCarreraRequestDTO request)
        {
            var ruc = await ObtenerRucLogueado();
            if (string.IsNullOrEmpty(ruc))
                return BadRequest("Esta cuenta no tiene una empresa vinculada.");

            try
            {
                _solicitud.CrearSolicitud(request, ruc);
                return Ok("Solicitud enviada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al enviar la solicitud: " + ex.Message);
            }
        }

        [HttpGet("GetMisSolicitudes")]
        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> GetMisSolicitudes()
        {
            var ruc = await ObtenerRucLogueado();
            if (string.IsNullOrEmpty(ruc))
                return BadRequest("Esta cuenta no tiene una empresa vinculada.");

            return Ok(_solicitud.GetMisSolicitudes(ruc));
        }

        // Bandeja del despachador.
        [HttpGet("GetSolicitudesPendientes")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetSolicitudesPendientes()
        {
            return Ok(_solicitud.GetSolicitudesPendientes());
        }

        [HttpPost("MarcarConvertida/{idsolicitud}")]
        [Authorize(Roles = "Admin")]
        public IActionResult MarcarConvertida(int idsolicitud)
        {
            try
            {
                var usuario = User.Identity?.Name ?? "desconocido";
                _solicitud.MarcarConvertida(idsolicitud, usuario);
                return Ok("Solicitud marcada como convertida.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar la solicitud: " + ex.Message);
            }
        }
    }
}
