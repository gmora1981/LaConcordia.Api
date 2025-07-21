using Identity.Api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo.laconcordia.Modelo.Database;


using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TipolicenciumController : Controller
    {
        private readonly ITipolicencium _tipolicencium;
        public TipolicenciumController(ITipolicencium tipolicencium)
        {
            _tipolicencium = tipolicencium;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("TipolicenciaInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _tipolicencium.GetTipolicenciaInfoAll();
            return Ok(lista);
        }

        [HttpGet("BuscarTipolicencia/{id}")]
        public IActionResult GetById(int id)
        {
            var item = _tipolicencium.GetTipolicenciaById(id);
            if (item == null)
                return NotFound("Tipo de licencia no encontrado.");
            return Ok(item);
        }

        [HttpPost("InsertTipolicencia")]
        public IActionResult Create([FromBody] Tipolicencium nueva)
        {
            try
            {
                _tipolicencium.InsertTipolicencia(nueva);
                return Ok("Tipo de licencia creado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear: " + ex.Message);
            }
        }

        [HttpPut("UpdateTipolicencia")]
        public IActionResult Update([FromBody] Tipolicencium actualizada)
        {
            try
            {
                _tipolicencium.UpdateTipolicencia(actualizada);
                return Ok("Tipo de licencia actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar: " + ex.Message);
            }
        }

        [HttpDelete("DeleteTipolicencia/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _tipolicencium.DeleteTipolicenciaById(id);
                return Ok("Tipo de licencia eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar: " + ex.Message);
            }

        }
    }
}
