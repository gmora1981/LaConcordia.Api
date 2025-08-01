using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Identity.Api.Reporteria;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo.laconcordia.Modelo.Database;
using QuestPDF.Infrastructure;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FichapersonaController : Controller
    {
        private readonly IFichapersona _fichapersona;
        public FichapersonaController(IFichapersona fichapersona)
        {
            _fichapersona = fichapersona;
        }
        [HttpGet("GetFichaPersonalInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _fichapersona.GetFichaPersonalInfoAll();
            return Ok(lista);
        }
        [HttpGet("GetFichaPersonalById/{cedula}")]
        public IActionResult GetById(string cedula)
        {
            var item = _fichapersona.GetFichaPersonalById(cedula);
            if (item == null)
                return NotFound("Ficha personal no encontrada.");
            return Ok(item);
        }

        [HttpPost("InsertFichaPersonal")]
        public IActionResult Create([FromBody] FichapersonalDTO nueva)
        {
            try
            {
                _fichapersona.InsertFichaPersonal(nueva);
                return Ok("Ficha personal creada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear: " + ex.Message);
            }
        }

        [HttpPut("UpdateFichaPersonal")]
        public IActionResult Update([FromBody] FichapersonalDTO actualizada)
        {
            try
            {
                _fichapersona.UpdateFichaPersonal(actualizada);
                return Ok("Ficha personal actualizada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar: " + ex.Message);
            }
        }

        [HttpDelete("DeleteFichaPersonalById/{cedula}")]
        public IActionResult Delete(string cedula)
        {
            try
            {
                _fichapersona.DeleteFichaPersonalById(cedula);
                return Ok("Ficha personal eliminada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar: " + ex.Message);
            }
        }

        [HttpGet("GetFichaPersonalPaginados")]


    }
}
