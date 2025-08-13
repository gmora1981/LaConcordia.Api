using Identity.Api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SegurovidumController : Controller
    {
        private readonly ISegurovidum _segurovidumService;

        public SegurovidumController(ISegurovidum segurovidumService)
        {
            _segurovidumService = segurovidumService;
        }

        [HttpGet("GetSegurovidumInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _segurovidumService.GetSegurovidumInfoAll();
            return Ok(lista);
        }

        [HttpGet("GetSegurovidumByCedula/{CiAfiliado}")]
        public IActionResult GetByCedula(string CiAfiliado)
        {
            var items = _segurovidumService.GetSegurovidumByCedula(CiAfiliado);
            if (items == null || !items.Any())
                return NotFound("No se encontraron registros para esa cédula.");
            return Ok(items);
        }

        [HttpPost("InsertSegurovidum")]
        public IActionResult Create([FromBody] DTO.SegurovidumDTO nueva)
        {
            try
            {
                _segurovidumService.InsertSegurovidum(nueva);
                return Ok("Registro de seguro creado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear: " + ex.Message);
            }
        }

        [HttpPut("UpdateSegurovidum")]
        public IActionResult Update([FromBody] DTO.SegurovidumDTO actualizada)
        {
            try
            {
                _segurovidumService.UpdateSegurovidum(actualizada);
                return Ok("Registro de seguro actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar: " + ex.Message);
            }
        }

        [HttpDelete("DeleteSegurovidumByCedula/{CiBeneficiario}/{CiAfiliado}")]
        public IActionResult DeleteByCedula(string CiBeneficiario, string CiAfiliado)
        {
            try
            {
                _segurovidumService.DeleteSegurovidumByCedula(CiBeneficiario, CiAfiliado);
                return Ok("Registro de seguro eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar: " + ex.Message);
            }
        }


        [HttpGet("GetSegurovidumPaginados")]
        public async Task<IActionResult> GetPaginados(
            int pagina = 1,
            int pageSize = 10,
            string? CiBeneficiario = null,
            string? CiAfiliado = null)
        {
            var result = await _segurovidumService.GetSegurovidumPaginados(pagina, pageSize, CiBeneficiario, CiAfiliado);
            return Ok(result);
        }

        [HttpGet("GetSegurovidumPaginadosByCedulaAfiliado")]
        public async Task<IActionResult> GetPaginadosByCedulaAfiliado(
            int pagina = 1,
            int pageSize = 10,
            string CiAfiliado = null)
        {
            var result = await _segurovidumService.GetSegurovidumPaginadosByCedulaAfiliado(pagina, pageSize, CiAfiliado);
            return Ok(result);
        }

    }
}
