using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo.laconcordia.Modelo.Database;



namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FichaobservacioneController : Controller
    {
        private readonly IFichaobservacione _fichaObservacione;
        public FichaobservacioneController(IFichaobservacione fichaObservacione)
        {
            _fichaObservacione = fichaObservacione;
        }

        [HttpGet("GetFichaObservacioneInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _fichaObservacione.GetFichaObservacioneInfoAll();
            return Ok(lista);
        }

        [HttpGet("GetFichaObservacioneByCedula/{cedula}")]
        public IActionResult GetByCedula(string cedula)
        {
            var items = _fichaObservacione.GetFichaObservacioneByCedula(cedula);
            if (items == null || !items.Any())
                return NotFound("No se encontraron fichas de observación para esa cédula.");
            return Ok(items);
        }

        [HttpPost("InsertFichaObservacione")]
        public IActionResult Create([FromBody] FichaobservacioneDTO nueva)
        {
            try
            {
                _fichaObservacione.InsertFichaObservacione(nueva);
                return Ok("Ficha de observación creada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear: " + ex.Message);
            }
        }

        [HttpPut("UpdateFichaObservacione")]
        public IActionResult Update([FromBody] FichaobservacioneDTO actualizada)
        {
            try
            {
                _fichaObservacione.UpdateFichaObservacione(actualizada);
                return Ok("Ficha de observación actualizada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar: " + ex.Message);
            }
        }

        [HttpDelete("DeleteFichaObservacioneByCedula/{cedula}")]
        public IActionResult DeleteByCedula(int cedula)
        {
            try
            {
                _fichaObservacione.DeleteFichaObservacioneByCedula(cedula);
                return Ok("Ficha de observación eliminada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar: " + ex.Message);
            }
        }

        [HttpGet("GetFichaObservacionePaginados")]
        public async Task<IActionResult> GetPaginados(
            int pagina = 1,
            int pageSize = 10,
            string? unidad = null,
            string? cedula = null)
        {
            var result = await _fichaObservacione.GetFichaObservacionePaginados(pagina, pageSize, unidad, cedula);
            return Ok(result);
        }

        // Paginado por ID
        [HttpGet("GetFichaObservacionePaginadosByCedula")]
        public async Task<IActionResult> GetPaginadosById(
            int pagina = 1,
            int pageSize = 10,
            string Fkcedula = null)
        {
            var result = await _fichaObservacione.GetFichaObservacionePaginadosByCedula(pagina, pageSize, Fkcedula);
            return Ok(result);
        }

    }
}
