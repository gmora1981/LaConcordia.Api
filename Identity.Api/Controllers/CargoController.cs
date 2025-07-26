using Identity.Api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CargoController : Controller
    {

        private readonly ICargo _Cargo;
        public CargoController(ICargo iCargo)
        {
            _Cargo = iCargo;
        }

        [HttpGet("GetCargoInfoAll")]
        public IActionResult Get1()
        {
            return Ok(_Cargo.CargoInfoAll);
        }


        [HttpGet("GetCargoById/{id}")]
        public IActionResult GetById(int id)
        {
            var item = _Cargo.GetCargoById(id);
            if (item == null)
                return NotFound("Cargo no encontrado.");
            return Ok(item);
        }

        [HttpPost("InsertCargo")]
        public IActionResult Create([FromBody] Cargo NewItem)
        {
            try
            {
                if (NewItem == null || !ModelState.IsValid)
                {
                    return BadRequest("Error: Envio de datos");
                }

                //continuo con el ingreso de datos
                _Cargo.InsertCargo(NewItem);

            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex.Message);
            }

            return Ok(NewItem);
        }

        [HttpPut("UpdateCargo")]
        public IActionResult Update([FromBody] Cargo UpdItem)
        {
            try
            {
                if (UpdItem == null || !ModelState.IsValid)
                {
                    return BadRequest("Error: Envio de datos");
                }

                //continuo con el ingreso de datos
                _Cargo.UpdateCargo(UpdItem);

            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex.Message);
            }

            return NoContent();
        }

        

        [HttpDelete("DeleteCargoById/{IdRegistrado}")]
        public IActionResult Delete2(int IdRegistrado)
        {
            try
            {

                //continuo con el ingreso de datos
                _Cargo.DeleteCargo2(IdRegistrado);

            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex.Message);
            }

            return NoContent();
        }


        [HttpGet("GetCargoPaginados")]
        public async Task<IActionResult> GetCargoPaginados(
            int pagina = 1,
            int pageSize = 10,
            string? cargo1 = null,
            string? estado = null)
        {
            try
            {
                var result = await _Cargo.GetCargoPaginados(pagina, pageSize, cargo1, estado);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex.Message);
            }
        }

    }
}
