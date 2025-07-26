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
    public class NiveleeducacionController : Controller
    {
        private readonly INiveleducacion _niveleeducacion;
        public NiveleeducacionController(INiveleducacion niveleeducacion)
        {
            _niveleeducacion = niveleeducacion;
        }

        [HttpGet("GetNiveleducacionInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _niveleeducacion.GetNiveleducacionInfoAll();
            return Ok(lista);
        }

        [HttpGet("GetNiveleducacionById/{id}")]
        public IActionResult GetById(int id)
        {
            var item = _niveleeducacion.GetNiveleducacionById(id);
            if (item == null)
                return NotFound("Nivel de educación no encontrado.");
            return Ok(item);
        }
        [HttpPost("InsertNiveleeducacion")]
        public IActionResult Create([FromBody] Niveleducacion nueva)
        {
            try
            {
                _niveleeducacion.InsertNiveleducacion(nueva);
                return Ok("Nivel de educación creado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear: " + ex.Message);
            }
        }
        [HttpPut("UpdateNiveleeducacion")]
        public IActionResult Update([FromBody] Niveleducacion actualizada)
        {
            try
            {
                _niveleeducacion.UpdateNiveleducacion(actualizada);
                return Ok("Nivel de educación actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar: " + ex.Message);
            }
        }
        [HttpDelete("DeleteNiveleducacionById/{id}")]
        public IActionResult DeleteById(int id)
        {
            try
            {
                _niveleeducacion.DeleteNiveleducacionById(id);
                return Ok("Nivel de educación eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar: " + ex.Message);
            }
        }

        //paginado  
        [HttpGet("GetNiveleducacionPaginados")]
        public async Task<IActionResult> GetNiveleducacionPaginados(
            int pagina = 1,
            int pageSize = PaginadorHelper.NumeroDeDatosPorPagina,
            string? descripcion = null,

            string? estado = null)
        {
            try
            {
                var resultado = await _niveleeducacion.GetNiveleducacionPaginados(pagina, pageSize, descripcion,  estado);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
