using Identity.Api.DataRepository;
using Identity.Api.Interfaces;
using Identity.Api.Model.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UnidadController : Controller
    {

        private readonly IUnidad _unidadRepository;

        public UnidadController(IUnidad unidadRepository)
        {
            _unidadRepository = unidadRepository;
        }

        [HttpGet("GetUnidadInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _unidadRepository.GetUnidadInfoAll();
            return Ok(lista);
        }

        [HttpGet("BuscarUnidad/{idUnidad}")]
        public IActionResult GetById(string idUnidad)
        {
            var item = _unidadRepository.GetUnidadById(idUnidad);
            if (item == null)
                return NotFound("Unidad no encontrada.");
            return Ok(item);
        }

        [HttpPost("InsertUnidad")]
        public IActionResult Create([FromBody] Unidad nueva)
        {
            try
            {
                _unidadRepository.InsertUnidad(nueva);
                return Ok("Unidad creada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear: " + ex.Message);
            }
        }

        [HttpPut("UpdateUnidad")]
        public IActionResult Update([FromBody] Unidad actualizada)
        {
            try
            {
                _unidadRepository.UpdateUnidad(actualizada);
                return Ok("Unidad actualizada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar: " + ex.Message);
            }
        }

        [HttpDelete("DeleteUnidad/{idUnidad}")]
        public IActionResult DeleteById(string idUnidad)
        {
            try
            {
                _unidadRepository.DeleteUnidadById(idUnidad);
                return Ok("Parentesco eliminado por ID correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar por ID: " + ex.Message);
            }
        }

    }
}
