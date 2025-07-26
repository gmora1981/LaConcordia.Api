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
    public class NacionalidadController : Controller
    {
        private readonly INacionalidad _nacionalidad;
        public NacionalidadController(INacionalidad nacionalidad)
        {
            _nacionalidad = nacionalidad;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetNacionalidadInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _nacionalidad.GetNacionalidadInfoAll();
            return Ok(lista);
        }
        [HttpGet("GetNacionalidadById/{id}")]
        public IActionResult GetById(int id)
        {
            var item = _nacionalidad.GetNacionalidadById(id);
            if (item == null)
                return NotFound("Nacionalidad no encontrada.");
            return Ok(item);
        }
        [HttpPost("InsertNacionalidad")]
        public IActionResult Create([FromBody] Nacionalidad nueva)
        {
            try
            {
                _nacionalidad.InsertNacionalidad(nueva);
                return Ok("Nacionalidad creada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear: " + ex.Message);
            }
        }
        [HttpPut("UpdateNacionalidad")]
        public IActionResult Update([FromBody] Nacionalidad actualizada)
        {
            try
            {
                _nacionalidad.UpdateNacionalidad(actualizada);
                return Ok("Nacionalidad actualizada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar: " + ex.Message);
            }
        }
        [HttpDelete("DeleteNacionalidadById/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _nacionalidad.DeleteNacionalidadById(id);
                return Ok("Nacionalidad eliminada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar: " + ex.Message);
            }
        }

        //paginado  
        [HttpGet("GetNacionalPaginados")]
        public async Task<IActionResult> GetNacionalPaginados(
            int pagina = 1,
            int pageSize = PaginadorHelper.NumeroDeDatosPorPagina,
            string? nacionalidad = null,
            string? estado = null)
        {
            try
            {
                var resultado = await _nacionalidad.GetNacionalPaginados(pagina, pageSize, nacionalidad, estado);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}