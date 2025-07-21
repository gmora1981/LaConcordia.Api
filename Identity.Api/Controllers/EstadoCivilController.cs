using Identity.Api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EstadoCivilController : Controller
    {

        private readonly IEstadoCivil _estadoCivil;
        public EstadoCivilController(IEstadoCivil iestadoCivil)
        {
            _estadoCivil = iestadoCivil;
        }


        [HttpGet("GetEstadocivilInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _estadoCivil.GetEstadocivilInfoAll();
            return Ok(lista);
        }

        [HttpGet("BuscarEstadocivil/{idestadocivil}")]
        public IActionResult GetById(int idestadocivil)
        {
            var item = _estadoCivil.GetEstadocivilById(idestadocivil);
            if (item == null)
                return NotFound("Estado Civil no encontrado.");
            return Ok(item);
        }

        [HttpPost("InsertEstadocivil")]
        public IActionResult Create([FromBody] Estadocivil nueva)
        {
            try
            {
                _estadoCivil.InsertEstadocivil(nueva);
                return Ok("Estado Civil creado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear: " + ex.Message);
            }
        }

        [HttpPut("UpdateEstadocivil")]
        public IActionResult Update([FromBody] Estadocivil actualizada)
        {
            try
            {
                _estadoCivil.UpdateEstadocivil(actualizada);
                return Ok("Estado Civil actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar: " + ex.Message);
            }
        }

        [HttpDelete("DeleteEstadocivil/{idestadocivil}")]
        public IActionResult Delete(int idestadocivil)
        {
            try
            {
                _estadoCivil.DeleteEstadocivilById(idestadocivil);
                return Ok("Estado Civil eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar: " + ex.Message);
            }
        }
    }
}
